import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { IMessageReceiver } from './i-message-receiver';
import { RawMessage } from './raw-message';
import { LogBus } from '../log/log.bus';

/**
 * Handles low-level WebView2 communication.
 *
 * Responsibilities (single):
 *   - Establish / tear down the `chrome.webview` event listener.
 *   - Emit raw parsed messages on `messages$`.
 *   - Forward outgoing messages via `postMessage`.
 *
 * `connect()` polls for `chrome.webview` readiness (max 10 s, 200 ms interval)
 * so the caller never needs to know when the WebView2 host is ready.
 *
 * Must NOT contain application or domain logic.
 */
@Injectable({ providedIn: 'root' })
export class ConnectionManager implements IMessageReceiver {
  private readonly src = 'ConnectionManager';
  private webview: any | null = null;

  private readonly POLL_INTERVAL_MS = 200;
  private readonly POLL_MAX_ATTEMPTS = 50; // 200 ms × 50 = 10 s max wait

  private readonly messageSubject = new Subject<RawMessage>();

  /** Emits every inbound message from the WebView2 host. */
  readonly messages$: Observable<RawMessage> = this.messageSubject.asObservable();

  private readonly messageHandler = (event: any): void => {
    this.handleMessage(event?.data);
  };

  constructor(private readonly bus: LogBus) {}

  /**
   * Establish the WebView2 connection.
   * Polls for `chrome.webview` availability — safe to call immediately at bootstrap
   * before the WebView2 host has finished initialising.
   */
  connect(): void {
    if (this.webview) {
      this.bus.push({ level: 'warn', source: this.src, message: 'connect: already connected — ignoring duplicate call.' });
      return;
    }

    this.bus.push({ level: 'debug', source: this.src, message: 'connect: polling for chrome.webview...' });
    this.pollForWebView(0);
  }

  /** Send a JSON message to the WebView2 host. */
  send(message: RawMessage): void {
    if (!this.webview) {
      return;
    }
    // WebView2 TryGetWebMessageAsString() requires a string, not an object.
    this.webview.postMessage(JSON.stringify(message));
  }

  /** Remove the event listener and release the webview reference. */
  disconnect(): void {
    if (this.webview) {
      this.webview.removeEventListener('message', this.messageHandler);
      this.webview = null;
      this.bus.push({ level: 'debug', source: this.src, message: 'disconnect: WebView2 listener removed.' });
    }
  }

  // ---- private ----

  /**
   * Polls for `chrome.webview` every {@link POLL_INTERVAL_MS} ms.
   * Registers the message listener as soon as it becomes available.
   * Gives up after {@link POLL_MAX_ATTEMPTS} attempts and logs an error.
   */
  private pollForWebView(attempt: number): void {
    const chromeRef = (window as any)?.chrome;

    if (chromeRef?.webview) {
      this.webview = chromeRef.webview;
      this.webview.addEventListener('message', this.messageHandler);
      this.bus.push({ level: 'debug', source: this.src, message: `connect: WebView2 listener registered after ${attempt * this.POLL_INTERVAL_MS} ms.` });
      return;
    }

    if (attempt >= this.POLL_MAX_ATTEMPTS) {
      this.bus.push({ level: 'error', source: this.src, message: `connect: chrome.webview not available after ${this.POLL_MAX_ATTEMPTS * this.POLL_INTERVAL_MS} ms — giving up.` });
      return;
    }

    setTimeout(() => this.pollForWebView(attempt + 1), this.POLL_INTERVAL_MS);
  }

  private handleMessage(data: any): void {
    if (data === null || data === undefined) {
      this.bus.push({ level: 'warn', source: this.src, message: 'handleMessage: received null/undefined data — discarding.' });
      return;
    }

    try {
      const parsedMessage: RawMessage =
        typeof data === 'string' && data !== ''
          ? (JSON.parse(data) as RawMessage)
          : (data as RawMessage);

      this.messageSubject.next(parsedMessage);
    } catch (err) {
      // Malformed JSON — do not propagate into the Subject as that would
      // permanently terminate the stream.
      this.bus.push({ level: 'warn', source: this.src, message: `handleMessage: failed to parse message — discarding. ${err}` });
    }
  }
}

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ConnectionManager } from './connection.manager';
import { RawMessage } from './raw-message';
import { LogBus } from '../log/log.bus';

/**
 * Application-level communication facade.
 *
 * Converts high-level intents (action + optional payload) into
 * Action-keyed JSON messages and delegates to ConnectionManager.
 */
@Injectable({ providedIn: 'root' })
export class CommunicationService {
  private readonly src = 'CommunicationService';

  constructor(
    private readonly connection: ConnectionManager,
    private readonly bus:        LogBus
  ) {}

  /** Establish the WebView2 connection. Call once at bootstrap. */
  connect(): void {
    this.bus.push({ level: 'debug', source: this.src, message: 'connect: establishing WebView2 connection.' });
    this.connection.connect();
  }

  /**
   * Send an action-keyed message to the backend.
   * Wraps the action and payload into the `CallContext`/`Payload` envelope
   * that the C# `ConnectionManager` expects.
   * @param action  - The `Action` field value (e.g. `'VerifyInstallationPrerequisite'`).
   * @param payload - Optional payload fields sent as the `Payload` property.
   */
  send(action: string, payload?: Record<string, unknown>): void {
    const message = {
      CallContext: { Action: action },
      Payload: payload ?? {}
    };
    this.connection.send(message as RawMessage);
  }
  
  /** Graceful shutdown — removes the WebView2 event listener. */
  shutdown(): void {
    this.bus.push({ level: 'debug', source: this.src, message: 'shutdown: disconnecting.' });
    this.connection.disconnect();
  }
}


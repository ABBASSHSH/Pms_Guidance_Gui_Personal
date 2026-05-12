import { inject, Injectable } from '@angular/core';
import { filter, map } from 'rxjs';
import { MESSAGE_RECEIVER } from '../communication/i-message-receiver.token';
import { UiStateManagementService } from './ui-state-management-service';
import { BackendMessage } from './message.interfaces';
import { HANDLERS, HandlerContext } from './message-handlers';
import { LogService } from '../log/log.service';
import { I18nService } from '../i18n';

/**
 * Routes incoming backend messages to the appropriate registered handler.
 *
 * Flow:
 *   IMessageReceiver.messages$ → cast to BackendMessage → look up HANDLERS[Action] → call handler
 *
 * Depends on the IMessageReceiver token (DIP) — not on the concrete ConnectionManager.
 * Must be started exactly once via `start()` during application bootstrap.
 */
@Injectable({ providedIn: 'root' })
export class Converter {
  private readonly receiver = inject(MESSAGE_RECEIVER);
  private readonly state    = inject(UiStateManagementService);
  private readonly log      = inject(LogService);
  private readonly i18n     = inject(I18nService);

  private readonly context: HandlerContext = {
    state: this.state,
    log:   this.log,
    i18n:  this.i18n,
  };

  /** Subscribe to the message stream. Call once at bootstrap. */
  start(): void {
    this.receiver.messages$
      .pipe(
        map(m => m as unknown as BackendMessage),
        filter(m => !!m?.Action)
      )
      .subscribe({
        next: message => this.mapToHandler(message),
        error: (err: unknown) =>
          this.log.error('Converter', 'connect', `Message stream error: ${err}`),
      });
  }

  private mapToHandler(message: BackendMessage): void {
    const handler = HANDLERS[message.Action];

    if (!handler) {
      this.log.warn(
        'Converter',
        'route',
        `No handler registered for action: "${message.Action}". Full message payload: ${JSON.stringify(message)}`
      );
      return;
    }

    try {
      handler(message, this.context);
    } catch (error) {
      this.log.error('Converter', 'route', `Handler for '${message.Action}' threw an error: ${error}`);
    }
  }
}

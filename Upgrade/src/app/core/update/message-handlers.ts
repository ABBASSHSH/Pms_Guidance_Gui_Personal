import { UiStateManagementService } from './ui-state-management-service';
import { BackendMessage } from './message.interfaces';
import { LogService } from '../log/log.service';
import { I18nService } from '../i18n';
import { ShowInstallationPrereqHandler } from './handlers/show-installation-prereq.handler';
import { ShowSystemLanguageHandler } from './handlers/show-system-language.handler';
/**
 * Services available to every message handler.
 * Adding a dependency here keeps individual handler signatures stable.
 */
export interface HandlerContext {
  readonly state: UiStateManagementService;
  readonly log: LogService;
  readonly i18n: I18nService;
}

/** A pure function that processes one backend message and updates state. */
export type MessageHandler = (message: BackendMessage, context: HandlerContext) => void;

// ─────────────────────────────────────────────────────────────────────────────
// Handler registry
// ─────────────────────────────────────────────────────────────────────────────

/**
 * Maps each Action string to its handler.
 *
 * To add a new handler:
 *   1. Define the message interface in `message.interfaces.ts`.
 *   2. Add a handler class above with a static `isValid` guard and `handle` method.
 *   3. Register the `handle` method below.
 */
export const HANDLERS: Readonly<Record<string, MessageHandler>> = {
  ShowInstallationPrerequisite: ShowInstallationPrereqHandler.handle,
  ShowSystemLanguage:           ShowSystemLanguageHandler.handle,
};

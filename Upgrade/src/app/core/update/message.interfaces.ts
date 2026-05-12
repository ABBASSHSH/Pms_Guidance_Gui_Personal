import { PrereqStatus } from './update.models';

/**
 * Base interface for all backend messages.
 * All messages must have an Action field.
 */
export interface BackendMessage {
  Action: string;
}

/**
 * Message sent when installation prerequisites are checked
 */
export interface ShowInstallationPrereqMsg extends BackendMessage {
  Action: 'ShowInstallationPrerequisite';
  Status: PrereqStatus.OK | PrereqStatus.NotOk;
}

/**
 * Message sent by backend after connection is established
 * Contains the system language for the FE to use
 */
export interface ShowSystemLanguageMsg extends BackendMessage {
  Action: 'ShowSystemLanguage';
  /** System language code (e.g., 'en-US', 'de-DE') */
  Language: string;
}

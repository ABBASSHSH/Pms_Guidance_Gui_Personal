import { BackendMessage, ShowSystemLanguageMsg } from '../message.interfaces';
import { HandlerContext } from '../message-handlers';

export class ShowSystemLanguageHandler {
  static isValid(message: BackendMessage): message is ShowSystemLanguageMsg {
    return (
      message.Action === 'ShowSystemLanguage' &&
      'Language' in message &&
      typeof (message as ShowSystemLanguageMsg).Language === 'string'
    );
  }

  static handle(message: BackendMessage, context: HandlerContext): void {
    const { log, i18n } = context;

    if (!ShowSystemLanguageHandler.isValid(message)) {
      log.error('Handler', 'ShowSystemLanguage', `Invalid message structure: ${JSON.stringify(message)}`);
      return;
    }

    log.info('Handler', 'ShowSystemLanguage', `System language received: ${message.Language}`);
    i18n.setLanguage(message.Language);
    log.debug('Handler', 'ShowSystemLanguage', `Language set to ${message.Language}`);
  }
}
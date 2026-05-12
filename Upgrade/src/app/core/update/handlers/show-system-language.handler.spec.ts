import { ShowSystemLanguageHandler } from './show-system-language.handler';
import { ShowSystemLanguageMsg } from '../message.interfaces';
import { HandlerContext } from '../message-handlers';

describe('ShowSystemLanguageHandler', () => {
  let mockState: any;
  let mockLog: any;
  let mockI18n: any;
  let context: HandlerContext;

  beforeEach(() => {
    mockState = {} as any;
    mockLog   = jasmine.createSpyObj('LogService',  ['info', 'debug', 'warn', 'error']);
    mockI18n  = jasmine.createSpyObj('I18nService', ['setLanguage']);
    context   = { state: mockState, log: mockLog, i18n: mockI18n };
  });

  describe('isValid', () => {
    it('accepts a correct message', () => {
      const msg: ShowSystemLanguageMsg = { Action: 'ShowSystemLanguage', Language: 'English' };
      expect(ShowSystemLanguageHandler.isValid(msg)).toBeTrue();
    });

    it('rejects a missing Language field', () => {
      const msg = { Action: 'ShowSystemLanguage' } as any;
      expect(ShowSystemLanguageHandler.isValid(msg)).toBeFalse();
    });

    it('rejects a non-string Language field', () => {
      const msg = { Action: 'ShowSystemLanguage', Language: 123 } as any;
      expect(ShowSystemLanguageHandler.isValid(msg)).toBeFalse();
    });
  });

  describe('handle', () => {
    it('passes the Language string directly to i18n.setLanguage', () => {
      const msg: ShowSystemLanguageMsg = { Action: 'ShowSystemLanguage', Language: 'English' };
      ShowSystemLanguageHandler.handle(msg, context);
      expect(mockI18n.setLanguage).toHaveBeenCalledWith('English');
    });

    it('logs info with the received language', () => {
      const msg: ShowSystemLanguageMsg = { Action: 'ShowSystemLanguage', Language: 'German' };
      ShowSystemLanguageHandler.handle(msg, context);
      expect(mockLog.info).toHaveBeenCalledWith('Handler', 'ShowSystemLanguage', jasmine.stringContaining('German'));
    });

    it('logs debug after setting language', () => {
      const msg: ShowSystemLanguageMsg = { Action: 'ShowSystemLanguage', Language: 'English' };
      ShowSystemLanguageHandler.handle(msg, context);
      expect(mockLog.debug).toHaveBeenCalledWith('Handler', 'ShowSystemLanguage', 'Language set to English');
    });

    it('logs error and does not call setLanguage when Language field is missing', () => {
      const msg = { Action: 'ShowSystemLanguage' } as any;
      ShowSystemLanguageHandler.handle(msg, context);
      expect(mockLog.error).toHaveBeenCalledWith('Handler', 'ShowSystemLanguage', jasmine.stringContaining('Invalid message structure'));
      expect(mockI18n.setLanguage).not.toHaveBeenCalled();
    });

    it('logs error and does not call setLanguage when Language is not a string', () => {
      const msg = { Action: 'ShowSystemLanguage', Language: 123 } as any;
      ShowSystemLanguageHandler.handle(msg, context);
      expect(mockLog.error).toHaveBeenCalledWith('Handler', 'ShowSystemLanguage', jasmine.stringContaining('Invalid message structure'));
      expect(mockI18n.setLanguage).not.toHaveBeenCalled();
    });
  });
});
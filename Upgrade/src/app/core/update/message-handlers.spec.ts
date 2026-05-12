import { HANDLERS, MessageHandler } from './message-handlers';

describe('HANDLERS Registry', () => {
  it('should have ShowInstallationPrerequisite handler registered', () => {
    expect(HANDLERS['ShowInstallationPrerequisite']).toBeDefined();
    expect(typeof HANDLERS['ShowInstallationPrerequisite']).toBe('function');
  });

  it('should have ShowSystemLanguage handler registered', () => {
    expect(HANDLERS['ShowSystemLanguage']).toBeDefined();
    expect(typeof HANDLERS['ShowSystemLanguage']).toBe('function');
  });

  it('should not have handlers for unregistered actions', () => {
    expect(HANDLERS['UnknownAction']).toBeUndefined();
    expect(HANDLERS['NonExistentAction']).toBeUndefined();
  });

  it('should have exactly two handlers registered', () => {
    const handlerKeys = Object.keys(HANDLERS);
    expect(handlerKeys.length).toBe(2);
    expect(handlerKeys).toContain('ShowInstallationPrerequisite');
    expect(handlerKeys).toContain('ShowSystemLanguage');
  });

  it('ShowInstallationPrerequisite should be a function', () => {
    const handler: MessageHandler = HANDLERS['ShowInstallationPrerequisite'];
    expect(handler).toEqual(jasmine.any(Function));
  });

  it('ShowSystemLanguage should be a function', () => {
    const handler: MessageHandler = HANDLERS['ShowSystemLanguage'];
    expect(handler).toEqual(jasmine.any(Function));
  });
});

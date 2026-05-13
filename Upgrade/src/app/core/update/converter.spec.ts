import { TestBed } from '@angular/core/testing';
import { Subject } from 'rxjs';

import { Converter } from './converter';
import { UiStateManagementService } from './ui-state-management-service';
import { LogService } from '../log/log.service';
import { I18nService } from '../i18n';
import { MESSAGE_RECEIVER } from '../communication/i-message-receiver.token';
import { HANDLERS } from './message-handlers';
import { BackendMessage } from './message.interfaces';
import { RawMessage } from '../communication/raw-message';

describe('Converter', () => {
  let converter: Converter;
  let messageSubject: Subject<RawMessage>;
  let mockState: jasmine.SpyObj<UiStateManagementService>;
  let mockLog: jasmine.SpyObj<LogService>;
  let mockI18n: jasmine.SpyObj<I18nService>;

  beforeEach(() => {
    messageSubject = new Subject<RawMessage>();

    mockState = jasmine.createSpyObj('UiStateManagementService', ['setActive', 'setStepStatus', 'next']);
    mockLog   = jasmine.createSpyObj('LogService', ['debug', 'info', 'warn', 'error']);
    mockI18n  = jasmine.createSpyObj('I18nService', ['translate', 'setLanguage']);

    TestBed.configureTestingModule({
      providers: [
        Converter,
        { provide: UiStateManagementService, useValue: mockState },
        { provide: LogService,               useValue: mockLog   },
        { provide: I18nService,              useValue: mockI18n  },
        { provide: MESSAGE_RECEIVER,         useValue: { messages$: messageSubject.asObservable() } },
      ],
    });

    converter = TestBed.inject(Converter);
  });

  // ── Creation ─────────────────────────────────────────────────────────────────

  describe('Creation', () => {
    it('should create', () => {
      expect(converter).toBeTruthy();
    });
  });

  // ── start() ──────────────────────────────────────────────────────────────────

  describe('start()', () => {
    it('subscribes to the message stream and processes a known action', () => {
      const handler = spyOn(HANDLERS, 'ShowInstallationPrerequisite' as any);
      converter.start();

      const msg = { Action: 'ShowInstallationPrerequisite', Result: 'success' } as unknown as BackendMessage;
      messageSubject.next(msg as unknown as RawMessage);

      expect(HANDLERS['ShowInstallationPrerequisite']).toHaveBeenCalledWith(
        msg,
        jasmine.objectContaining({ state: mockState, log: mockLog, i18n: mockI18n })
      );
    });

    it('logs a warning when no handler is registered for the action', () => {
      converter.start();

      const msg = { Action: 'UnknownAction' } as unknown as RawMessage;
      messageSubject.next(msg);

      expect(mockLog.warn).toHaveBeenCalledWith(
        'Converter', 'route',
        jasmine.stringContaining('UnknownAction')
      );
    });

    it('does not call any handler for an unregistered action', () => {
      converter.start();
      messageSubject.next({ Action: 'GhostAction' } as unknown as RawMessage);
      // No registered handler should have been touched
      expect(mockState.setActive).not.toHaveBeenCalled();
      expect(mockState.setStepStatus).not.toHaveBeenCalled();
    });

    it('filters out messages with no Action field', () => {
      converter.start();
      messageSubject.next({} as unknown as RawMessage);
      messageSubject.next(null as unknown as RawMessage);
      expect(mockLog.warn).not.toHaveBeenCalled();
      expect(mockLog.error).not.toHaveBeenCalled();
    });

    it('logs an error when a handler throws', () => {
      spyOn(HANDLERS, 'ShowInstallationPrerequisite' as any).and.throwError('handler boom');
      converter.start();

      messageSubject.next({ Action: 'ShowInstallationPrerequisite' } as unknown as RawMessage);

      expect(mockLog.error).toHaveBeenCalledWith(
        'Converter', 'route',
        jasmine.stringContaining('ShowInstallationPrerequisite')
      );
    });

    it('continues processing subsequent messages after a handler throws', () => {
      const handlerSpy = spyOn(HANDLERS, 'ShowInstallationPrerequisite' as any)
        .and.throwError('boom');
      converter.start();

      messageSubject.next({ Action: 'ShowInstallationPrerequisite' } as unknown as RawMessage);
      // Second message — unknown action — should still hit the warn path
      messageSubject.next({ Action: 'AnotherUnknown' } as unknown as RawMessage);

      expect(mockLog.warn).toHaveBeenCalledWith(
        'Converter', 'route',
        jasmine.stringContaining('AnotherUnknown')
      );
    });

    it('handles stream error via the error callback and logs it', () => {
      converter.start();
      const streamError = new Error('stream failed');
      messageSubject.error(streamError);

      expect(mockLog.error).toHaveBeenCalledWith(
        'Converter', 'connect',
        jasmine.stringContaining('stream failed')
      );
    });

    it('passes the correct HandlerContext to the handler', () => {
      const handlerSpy = spyOn(HANDLERS, 'ShowSystemLanguage' as any);
      converter.start();

      messageSubject.next({ Action: 'ShowSystemLanguage' } as unknown as RawMessage);

      const ctx = (handlerSpy as jasmine.Spy).calls.mostRecent().args[1];
      expect(ctx.state).toBe(mockState);
      expect(ctx.log).toBe(mockLog);
      expect(ctx.i18n).toBe(mockI18n);
    });

    it('processes multiple consecutive messages correctly', () => {
      const installSpy = spyOn(HANDLERS, 'ShowInstallationPrerequisite' as any);
      const langSpy    = spyOn(HANDLERS, 'ShowSystemLanguage' as any);
      converter.start();

      messageSubject.next({ Action: 'ShowInstallationPrerequisite' } as unknown as RawMessage);
      messageSubject.next({ Action: 'ShowSystemLanguage' } as unknown as RawMessage);
      messageSubject.next({ Action: 'ShowInstallationPrerequisite' } as unknown as RawMessage);

      expect(installSpy).toHaveBeenCalledTimes(2);
      expect(langSpy).toHaveBeenCalledTimes(1);
    });

    it('does not process messages before start() is called', () => {
      const handlerSpy = spyOn(HANDLERS, 'ShowInstallationPrerequisite' as any);

      // Emit before subscribing
      messageSubject.next({ Action: 'ShowInstallationPrerequisite' } as unknown as RawMessage);
      converter.start();

      expect(handlerSpy).not.toHaveBeenCalled();
    });
  });

  // ── Cleanup (takeUntilDestroyed) ─────────────────────────────────────────────

  describe('cleanup', () => {
    it('subscription can be unsubscribed mid-stream without affecting other subscribers', () => {
      const handlerSpy = spyOn(HANDLERS, 'ShowInstallationPrerequisite' as any);
      converter.start();

      // Verify messages are processed before unsubscribing
      messageSubject.next({ Action: 'ShowInstallationPrerequisite' } as unknown as RawMessage);
      expect(handlerSpy).toHaveBeenCalledTimes(1);
    });

    it('a second start() call adds another subscription that processes messages independently', () => {
      const handlerSpy = spyOn(HANDLERS, 'ShowInstallationPrerequisite' as any);
      converter.start();
      converter.start();

      messageSubject.next({ Action: 'ShowInstallationPrerequisite' } as unknown as RawMessage);
      // Two subscriptions → handler called twice
      expect(handlerSpy).toHaveBeenCalledTimes(2);
    });
  });
});

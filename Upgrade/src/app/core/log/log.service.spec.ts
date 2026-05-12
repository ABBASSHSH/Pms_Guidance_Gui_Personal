import { TestBed } from '@angular/core/testing';
import { LogService } from './log.service';
import { LogManager } from './log.manager';

describe('LogService', () => {
  let service: LogService;
  let mockManager: jasmine.SpyObj<LogManager>;

  beforeEach(() => {
    mockManager = jasmine.createSpyObj('LogManager', ['debug', 'info', 'warn', 'error']);

    TestBed.configureTestingModule({
      providers: [
        LogService,
        { provide: LogManager, useValue: mockManager },
      ],
    });

    service = TestBed.inject(LogService);
  });

  // ── Initialization ──────────────────────────────────────────────────────────

  describe('Initialization', () => {
    it('should create the service', () => {
      expect(service).toBeTruthy();
    });

    it('should inject LogManager', () => {
      expect(service['manager']).toBe(mockManager);
    });
  });

  // ── debug() ─────────────────────────────────────────────────────────────────

  describe('debug()', () => {
    it('delegates to manager.debug with formatted message', () => {
      service.debug('Src', 'Event', 'msg');
      expect(mockManager.debug).toHaveBeenCalledWith('Src', 'Event: msg');
    });

    it('called exactly once per invocation', () => {
      service.debug('S', 'E', 'M');
      expect(mockManager.debug).toHaveBeenCalledTimes(1);
    });

    it('handles empty message', () => {
      service.debug('S', 'E', '');
      expect(mockManager.debug).toHaveBeenCalledWith('S', 'E: ');
    });

    it('handles empty event', () => {
      service.debug('S', '', 'M');
      expect(mockManager.debug).toHaveBeenCalledWith('S', ': M');
    });

    it('propagates manager errors', () => {
      mockManager.debug.and.throwError('err');
      expect(() => service.debug('S', 'E', 'M')).toThrow();
    });
  });

  // ── info() ──────────────────────────────────────────────────────────────────

  describe('info()', () => {
    it('delegates to manager.info with formatted message', () => {
      service.info('Src', 'Event', 'msg');
      expect(mockManager.info).toHaveBeenCalledWith('Src', 'Event: msg');
    });

    it('called exactly once per invocation', () => {
      service.info('S', 'E', 'M');
      expect(mockManager.info).toHaveBeenCalledTimes(1);
    });

    it('handles empty message', () => {
      service.info('S', 'E', '');
      expect(mockManager.info).toHaveBeenCalledWith('S', 'E: ');
    });

    it('handles special characters', () => {
      const msg = '"quotes" & <tags>';
      service.info('S', 'E', msg);
      expect(mockManager.info).toHaveBeenCalledWith('S', `E: ${msg}`);
    });

    it('propagates manager errors', () => {
      mockManager.info.and.throwError('err');
      expect(() => service.info('S', 'E', 'M')).toThrow();
    });
  });

  // ── warn() ──────────────────────────────────────────────────────────────────

  describe('warn()', () => {
    it('delegates to manager.warn with formatted message', () => {
      service.warn('Src', 'Event', 'msg');
      expect(mockManager.warn).toHaveBeenCalledWith('Src', 'Event: msg');
    });

    it('called exactly once per invocation', () => {
      service.warn('S', 'E', 'M');
      expect(mockManager.warn).toHaveBeenCalledTimes(1);
    });

    it('handles empty message', () => {
      service.warn('S', 'E', '');
      expect(mockManager.warn).toHaveBeenCalledWith('S', 'E: ');
    });

    it('propagates manager errors', () => {
      mockManager.warn.and.throwError('err');
      expect(() => service.warn('S', 'E', 'M')).toThrow();
    });
  });

  // ── error() ─────────────────────────────────────────────────────────────────

  describe('error()', () => {
    it('delegates to manager.error with formatted message', () => {
      service.error('Src', 'Event', 'msg');
      expect(mockManager.error).toHaveBeenCalledWith('Src', 'Event: msg');
    });

    it('called exactly once per invocation', () => {
      service.error('S', 'E', 'M');
      expect(mockManager.error).toHaveBeenCalledTimes(1);
    });

    it('handles stack-trace style messages', () => {
      const trace = 'Error\n  at foo.ts:1:2';
      service.error('S', 'E', trace);
      expect(mockManager.error).toHaveBeenCalledWith('S', `E: ${trace}`);
    });

    it('propagates manager errors', () => {
      mockManager.error.and.throwError('err');
      expect(() => service.error('S', 'E', 'M')).toThrow();
    });
  });

  // ── Message formatting ───────────────────────────────────────────────────────

  describe('Message formatting', () => {
    it('prefixes message with "<event>: "', () => {
      service.info('Src', 'MyEvent', 'data loaded');
      expect(mockManager.info).toHaveBeenCalledWith('Src', 'MyEvent: data loaded');
    });

    it('passes source unchanged', () => {
      service.info('My:Component', 'E', 'M');
      expect(mockManager.info).toHaveBeenCalledWith('My:Component', 'E: M');
    });

    it('does not alter the source parameter', () => {
      const src = 'OriginalSource';
      service.warn(src, 'E', 'M');
      expect(mockManager.warn.calls.mostRecent().args[0]).toBe(src);
    });
  });

  // ── Multiple calls ───────────────────────────────────────────────────────────

  describe('Multiple calls', () => {
    it('handles sequential calls to different methods', () => {
      service.debug('S', 'E', 'D');
      service.info('S', 'E', 'I');
      service.warn('S', 'E', 'W');
      service.error('S', 'E', 'Err');

      expect(mockManager.debug).toHaveBeenCalledTimes(1);
      expect(mockManager.info).toHaveBeenCalledTimes(1);
      expect(mockManager.warn).toHaveBeenCalledTimes(1);
      expect(mockManager.error).toHaveBeenCalledTimes(1);
    });

    it('handles 100 rapid info() calls', () => {
      for (let i = 0; i < 100; i++) service.info('S', 'E', `M${i}`);
      expect(mockManager.info).toHaveBeenCalledTimes(100);
    });

    it('preserves call order', () => {
      const received: string[] = [];
      mockManager.info.and.callFake((_src: string, msg: string) => received.push(msg));

      for (let i = 0; i < 5; i++) service.info('S', 'E', `M${i}`);

      expect(received).toEqual(['E: M0', 'E: M1', 'E: M2', 'E: M3', 'E: M4']);
    });
  });

  // ── No flush on service ──────────────────────────────────────────────────────

  describe('No flush method', () => {
    it('does not expose a flush() method', () => {
      expect((service as any).flush).toBeUndefined();
    });
  });
});

import { TestBed } from '@angular/core/testing';
import { LogManager } from './log.manager';
import { LogBus } from './log.bus';
import { CommunicationService } from '../communication/communication.service';

/** Fixed ISO timestamp returned by the timestamp() spy in every test. */
const FIXED_TS = '2026-01-01T00:00:00.000Z';

/** Regex that matches a real ISO-8601 UTC timestamp. */
const ISO_PATTERN = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z$/;

describe('LogManager', () => {
  let service: LogManager;
  let mockComm: jasmine.SpyObj<CommunicationService>;

  beforeEach(() => {
    mockComm = jasmine.createSpyObj('CommunicationService', ['send']);

    TestBed.configureTestingModule({
      providers: [
        LogManager,
        { provide: CommunicationService, useValue: mockComm },
      ],
    });

    service = TestBed.inject(LogManager);

    // Pin the timestamp so every format assertion is deterministic.
    spyOn(service as any, 'timestamp').and.returnValue(FIXED_TS);
  });

  // -- Initialization --

  describe('Initialization', () => {
    it('should create the service', () => {
      expect(service).toBeTruthy();
    });

    it('should inject CommunicationService', () => {
      expect(service['comm']).toBe(mockComm);
    });
  });

  // -- timestamp() --

  describe('timestamp()', () => {
    it('returns a valid ISO-8601 UTC string when not spied upon', () => {
      const freshComm = jasmine.createSpyObj('CommunicationService', ['send']);
      TestBed.resetTestingModule();
      TestBed.configureTestingModule({
        providers: [
          LogManager,
          { provide: CommunicationService, useValue: freshComm },
        ],
      });
      const fresh = TestBed.inject(LogManager);
      const ts = (fresh as any).timestamp() as string;
      expect(ts).toMatch(ISO_PATTERN);
    });
  });

  // -- Logging methods (console output) --

  describe('Logging methods', () => {
    it('debug() sends to backend with DEBUG level', () => {
      service.debug('Src', 'msg');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[DEBUG] [Src] msg', Timestamp: FIXED_TS }
      );
    });

    it('info() sends to backend with INFO level', () => {
      service.info('Src', 'msg');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[INFO] [Src] msg', Timestamp: FIXED_TS }
      );
    });

    it('warn() sends to backend with WARN level', () => {
      service.warn('Src', 'msg');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[WARN] [Src] msg', Timestamp: FIXED_TS }
      );
    });

    it('error() sends to backend with ERROR level', () => {
      service.error('Src', 'msg');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[ERROR] [Src] msg', Timestamp: FIXED_TS }
      );
    });

    it('handles undefined message (defaults to empty string)', () => {
      service.info('Src');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[INFO] [Src] ', Timestamp: FIXED_TS }
      );
    });
  });

  // -- Immediate send to backend --

  describe('Immediate send to CommunicationService', () => {
    it('sends immediately on debug()', () => {
      service.debug('Src', 'msg');
      expect(mockComm.send).toHaveBeenCalledTimes(1);
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[DEBUG] [Src] msg', Timestamp: FIXED_TS }
      );
    });

    it('sends immediately on info()', () => {
      service.info('Src', 'msg');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[INFO] [Src] msg', Timestamp: FIXED_TS }
      );
    });

    it('sends immediately on warn()', () => {
      service.warn('Src', 'msg');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[WARN] [Src] msg', Timestamp: FIXED_TS }
      );
    });

    it('sends immediately on error()', () => {
      service.error('Src', 'msg');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[ERROR] [Src] msg', Timestamp: FIXED_TS }
      );
    });

    it('sends one message per log call', () => {
      service.debug('S1', 'M1');
      service.info('S2', 'M2');
      service.warn('S3', 'M3');
      expect(mockComm.send).toHaveBeenCalledTimes(3);
    });

    it('sends with correct action name "LogMessage"', () => {
      service.info('Src', 'msg');
      expect(mockComm.send.calls.mostRecent().args[0]).toBe('LogMessage');
    });

    it('formats the payload Message correctly', () => {
      service.warn('MyComp', 'Something happened');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[WARN] [MyComp] Something happened', Timestamp: FIXED_TS }
      );
    });

    it('handles undefined message in payload', () => {
      service.error('Src');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[ERROR] [Src] ', Timestamp: FIXED_TS }
      );
    });
  });

  // -- Log-level filtering --

  describe('Log-level filtering', () => {
    beforeEach(() => {
      spyOn(console, 'debug');
      spyOn(console, 'info');
      spyOn(console, 'warn');
      spyOn(console, 'error');
    });

    it('logs all four levels when minLevel is debug (default)', () => {
      service.debug('T', 'debug');
      service.info('T', 'info');
      service.warn('T', 'warn');
      service.error('T', 'error');

      expect(mockComm.send).toHaveBeenCalledTimes(4);
    });

    it('suppresses levels below minLevel (warn)', () => {
      (service as any).minLevel = 'warn';

      service.debug('T', 'should be suppressed');
      service.info('T', 'should be suppressed');
      service.warn('T', 'should pass');
      service.error('T', 'should pass');

      expect(mockComm.send).toHaveBeenCalledTimes(2);
    });

    it('suppresses all levels below error when minLevel is error', () => {
      (service as any).minLevel = 'error';

      service.debug('T', 'suppressed');
      service.info('T', 'suppressed');
      service.warn('T', 'suppressed');

      expect(mockComm.send).not.toHaveBeenCalled();
      expect(console.debug).not.toHaveBeenCalled();

      service.error('T', 'passes');
      expect(mockComm.send).toHaveBeenCalledTimes(1);
    });
  });

  // -- Message content handling --

  describe('Message content handling', () => {
    beforeEach(() => spyOn(console, 'info'));

    it('preserves special characters', () => {
      service.info('Src', 'msg with "quotes" & <tags>');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[INFO] [Src] msg with "quotes" & <tags>', Timestamp: FIXED_TS }
      );
    });

    it('preserves newlines', () => {
      service.info('Src', 'line1\nline2');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[INFO] [Src] line1\nline2', Timestamp: FIXED_TS }
      );
    });

    it('handles very long messages without throwing', () => {
      const long = 'A'.repeat(10000);
      expect(() => service.info('Src', long)).not.toThrow();
      expect(mockComm.send).toHaveBeenCalledTimes(1);
    });

    it('handles empty source', () => {
      service.info('', 'msg');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[INFO] [] msg', Timestamp: FIXED_TS }
      );
    });

    it('handles unicode characters', () => {
      service.info('Src', '\u4e2d\u6587 \u0627\u0644\u0639\u0631\u0628\u064a\u0629 \uD83C\uDF89');
      expect(mockComm.send).toHaveBeenCalledWith(
        'LogMessage',
        { Message: '[INFO] [Src] \u4e2d\u6587 \u0627\u0644\u0639\u0631\u0628\u064a\u0629 \uD83C\uDF89', Timestamp: FIXED_TS }
      );
    });
  });

  // -- Performance --

  describe('Performance', () => {
    it('handles 1000 rapid log calls within 1 second', () => {
      spyOn(console, 'info');
      const start = Date.now();
      for (let i = 0; i < 1000; i++) service.info(`S${i}`, `M${i}`);
      expect(Date.now() - start).toBeLessThan(1000);
      expect(mockComm.send).toHaveBeenCalledTimes(1000);
    });
  });

  // -- Error propagation --

  describe('Error propagation', () => {
    it('propagates CommunicationService errors', () => {
      spyOn(console, 'info');
      mockComm.send.and.throwError('Network error');
      expect(() => service.info('Src', 'msg')).toThrowError('Network error');
    });
  });

  // -- ngOnDestroy --

  describe('ngOnDestroy', () => {
    it('unsubscribes from LogBus entries$ on destroy', () => {
      spyOn(console, 'info');
      const logBus = TestBed.inject(LogBus);

      logBus.push({ level: 'info', source: 'Test', message: 'before destroy' });
      expect(console.info).toHaveBeenCalledTimes(1);
      (console.info as jasmine.Spy).calls.reset();

      service.ngOnDestroy();

      logBus.push({ level: 'info', source: 'Test', message: 'after destroy' });
      expect(console.info).not.toHaveBeenCalled();
    });

    it('does not forward any log level from LogBus after destruction', () => {
      spyOn(console, 'debug');
      spyOn(console, 'info');
      spyOn(console, 'warn');
      spyOn(console, 'error');
      const logBus = TestBed.inject(LogBus);

      service.ngOnDestroy();

      logBus.push({ level: 'debug', source: 'T', message: 'd' });
      logBus.push({ level: 'info',  source: 'T', message: 'i' });
      logBus.push({ level: 'warn',  source: 'T', message: 'w' });
      logBus.push({ level: 'error', source: 'T', message: 'e' });

      expect(console.debug).not.toHaveBeenCalled();
      expect(console.info).not.toHaveBeenCalled();
      expect(console.warn).not.toHaveBeenCalled();
      expect(console.error).not.toHaveBeenCalled();
    });

    it('does not affect the ability to call write() methods directly after destroy', () => {
      service.ngOnDestroy();
      expect(() => service.info('Src', 'msg')).not.toThrow();
      expect(mockComm.send).toHaveBeenCalledTimes(1);
    });
  });
});

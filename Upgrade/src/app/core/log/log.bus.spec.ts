import { TestBed } from '@angular/core/testing';
import { LogBus } from './log.bus';
import { LogEntry } from './log.models';

describe('LogBus', () => {
  let bus: LogBus;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LogBus],
    });
    bus = TestBed.inject(LogBus);
  });

  // ── Creation ─────────────────────────────────────────────────────────────────

  describe('Creation', () => {
    it('should create', () => {
      expect(bus).toBeTruthy();
    });

    it('exposes entries$ observable', () => {
      expect(bus.entries$).toBeDefined();
      expect(typeof bus.entries$.subscribe).toBe('function');
    });
  });

  // ── push() ───────────────────────────────────────────────────────────────────

  describe('push()', () => {
    it('emits the pushed entry on entries$', () => {
      const received: LogEntry[] = [];
      bus.entries$.subscribe(e => received.push(e));

      const entry: LogEntry = { level: 'info', source: 'Test', message: 'hello' };
      bus.push(entry);

      expect(received).toEqual([entry]);
    });

    it('emits multiple entries in order', () => {
      const received: LogEntry[] = [];
      bus.entries$.subscribe(e => received.push(e));

      const entries: LogEntry[] = [
        { level: 'debug', source: 'A', message: '1' },
        { level: 'warn',  source: 'B', message: '2' },
        { level: 'error', source: 'C', message: '3' },
      ];
      entries.forEach(e => bus.push(e));

      expect(received).toEqual(entries);
    });

    it('delivers to all active subscribers', () => {
      const r1: LogEntry[] = [];
      const r2: LogEntry[] = [];
      bus.entries$.subscribe(e => r1.push(e));
      bus.entries$.subscribe(e => r2.push(e));

      const entry: LogEntry = { level: 'warn', source: 'X', message: 'broadcast' };
      bus.push(entry);

      expect(r1).toEqual([entry]);
      expect(r2).toEqual([entry]);
    });

    it('does not emit to unsubscribed observers', () => {
      const received: LogEntry[] = [];
      const sub = bus.entries$.subscribe(e => received.push(e));
      sub.unsubscribe();

      bus.push({ level: 'info', source: 'T', message: 'late' });

      expect(received).toEqual([]);
    });
  });

  // ── ngOnDestroy ─────────────────────────────────────────────────────────────

  describe('ngOnDestroy', () => {
    it('completes entries$ for existing subscribers', () => {
      let completed = false;
      bus.entries$.subscribe({ complete: () => { completed = true; } });

      bus.ngOnDestroy();

      expect(completed).toBeTrue();
    });

    it('does not emit entries pushed after destruction', () => {
      const received: LogEntry[] = [];
      bus.entries$.subscribe(e => received.push(e));

      bus.ngOnDestroy();
      bus.push({ level: 'info', source: 'T', message: 'after destroy' });

      expect(received).toEqual([]);
    });

    it('new subscribers after destruction receive completion immediately', () => {
      bus.ngOnDestroy();

      let completed = false;
      bus.entries$.subscribe({ complete: () => { completed = true; } });

      expect(completed).toBeTrue();
    });

    it('completes all subscribers when destroyed', () => {
      let s1Done = false;
      let s2Done = false;
      bus.entries$.subscribe({ complete: () => { s1Done = true; } });
      bus.entries$.subscribe({ complete: () => { s2Done = true; } });

      bus.ngOnDestroy();

      expect(s1Done).toBeTrue();
      expect(s2Done).toBeTrue();
    });
  });
});

import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { UiStateManagementService } from './ui-state-management-service';
import { LogService } from '../log/log.service';
import { StepId, StepStatus } from './update.models';

describe('UiStateManagementService', () => {
  let service: UiStateManagementService;
  let mockLog: jasmine.SpyObj<LogService>;

  beforeEach(() => {
    mockLog = jasmine.createSpyObj('LogService', ['debug', 'info', 'warn', 'error']);

    TestBed.configureTestingModule({
      providers: [
        UiStateManagementService,
        { provide: LogService, useValue: mockLog },
      ],
    });

    service = TestBed.inject(UiStateManagementService);
  });

  // ── Creation ─────────────────────────────────────────────────────────────────

  describe('Initialization', () => {
    it('should create', () => {
      expect(service).toBeTruthy();
    });

    it('logs init message on construction', () => {
      expect(mockLog.info).toHaveBeenCalledWith(
        'UiStateManagementService', 'init', 'State service initialized'
      );
    });

    it('initial active step is Introduction (0)', fakeAsync(() => {
      let step: StepId | undefined;
      service.activeStepId$.subscribe(s => (step = s));
      tick();
      expect(step).toBe(StepId.Introduction);
    }));

    it('initial step status has Introduction as Active', fakeAsync(() => {
      let statuses: Record<StepId, StepStatus> | undefined;
      service.stepStatuses$.subscribe(s => (statuses = s));
      tick();
      expect(statuses![StepId.Introduction]).toBe(StepStatus.Active);
    }));
  });

  // ── next() ───────────────────────────────────────────────────────────────────

  describe('next()', () => {
    it('advances activeStepId from Introduction to VerifyPrereq', fakeAsync(() => {
      service.next();
      let step: StepId | undefined;
      service.activeStepId$.subscribe(s => (step = s));
      tick();
      expect(step).toBe(StepId.VerifyPrereq);
    }));

    it('marks previous step as Success', fakeAsync(() => {
      service.next();
      let statuses: Record<StepId, StepStatus> | undefined;
      service.stepStatuses$.subscribe(s => (statuses = s));
      tick();
      expect(statuses![StepId.Introduction]).toBe(StepStatus.Success);
    }));

    it('marks new step as Active', fakeAsync(() => {
      service.next();
      let statuses: Record<StepId, StepStatus> | undefined;
      service.stepStatuses$.subscribe(s => (statuses = s));
      tick();
      expect(statuses![StepId.VerifyPrereq]).toBe(StepStatus.Active);
    }));

    it('logs info when advancing', () => {
      mockLog.info.calls.reset();
      service.next();
      expect(mockLog.info).toHaveBeenCalledWith(
        'UiStateManagementService', 'next',
        jasmine.stringContaining('Introduction')
      );
    });

    it('does not advance past Installation (last step)', fakeAsync(async () => {
      // Advance to last step
      for (let i = 0; i < StepId.Installation; i++) service.next();

      const stepsBefore = await new Promise<StepId>(r =>
        service.activeStepId$.subscribe(s => r(s))
      );

      service.next(); // should be blocked

      let stepAfter: StepId | undefined;
      service.activeStepId$.subscribe(s => (stepAfter = s));
      tick();

      expect(stepAfter).toBe(StepId.Installation);
    }));

    it('logs warn when already on last step', fakeAsync(() => {
      for (let i = 0; i < StepId.Installation; i++) service.next();
      mockLog.warn.calls.reset();
      service.next();
      expect(mockLog.warn).toHaveBeenCalledWith(
        'UiStateManagementService', 'next', 'Already on last step'
      );
    }));

    it('does not call flush', () => {
      expect((mockLog as any).flush).toBeUndefined();
    });

    it('advances from VerifyPrereq to VerificationResult when next() is called', fakeAsync(() => {
      service.next(); // Introduction -> VerifyPrereq
      service.next(); // VerifyPrereq -> VerificationResult
      let step: StepId | undefined;
      service.activeStepId$.subscribe(s => (step = s));
      tick();
      expect(step).toBe(StepId.VerificationResult);
    }));
  });

  // ── setActive() ──────────────────────────────────────────────────────────────

  describe('setActive()', () => {
    it('changes active step', fakeAsync(() => {
      service.setActive(StepId.SaveImages);
      let step: StepId | undefined;
      service.activeStepId$.subscribe(s => (step = s));
      tick();
      expect(step).toBe(StepId.SaveImages);
    }));

    it('marks step as Active in statuses', fakeAsync(() => {
      service.setActive(StepId.DriveToPark);
      let statuses: Record<StepId, StepStatus> | undefined;
      service.stepStatuses$.subscribe(s => (statuses = s));
      tick();
      expect(statuses![StepId.DriveToPark]).toBe(StepStatus.Active);
    }));

    it('logs info', () => {
      mockLog.info.calls.reset();
      service.setActive(StepId.Installation);
      expect(mockLog.info).toHaveBeenCalledWith(
        'UiStateManagementService', 'setActive',
        jasmine.stringContaining('Installation')
      );
    });
  });

  // ── setStepStatus() ──────────────────────────────────────────────────────────

  describe('setStepStatus()', () => {
    it('updates the status of a specific step', fakeAsync(() => {
      service.setStepStatus(StepId.VerifyPrereq, StepStatus.Success);
      let statuses: Record<StepId, StepStatus> | undefined;
      service.stepStatuses$.subscribe(s => (statuses = s));
      tick();
      expect(statuses![StepId.VerifyPrereq]).toBe(StepStatus.Success);
    }));

    it('does not modify other step statuses', fakeAsync(() => {
      service.setStepStatus(StepId.VerifyPrereq, StepStatus.Error);
      let statuses: Record<StepId, StepStatus> | undefined;
      service.stepStatuses$.subscribe(s => (statuses = s));
      tick();
      expect(statuses![StepId.Introduction]).toBe(StepStatus.Active);
    }));

    it('logs info', () => {
      mockLog.info.calls.reset();
      service.setStepStatus(StepId.SaveImages, StepStatus.Warning);
      expect(mockLog.info).toHaveBeenCalledWith(
        'UiStateManagementService', 'setStepStatus',
        jasmine.stringContaining('SaveImages')
      );
    });
  });

  // ── Dependency injection ────────────────────────────────────────────────────

  describe('Dependency injection', () => {
    it('injects LogService', () => {
      expect((service as any).log).toBe(mockLog);
    });
  });

  // ── ngOnDestroy ─────────────────────────────────────────────────────────────

  describe('ngOnDestroy', () => {
    it('completes activeStepId$ when destroyed', () => {
      let completed = false;
      service.activeStepId$.subscribe({ complete: () => { completed = true; } });
      service.ngOnDestroy();
      expect(completed).toBeTrue();
    });

    it('completes stepStatuses$ when destroyed', () => {
      let completed = false;
      service.stepStatuses$.subscribe({ complete: () => { completed = true; } });
      service.ngOnDestroy();
      expect(completed).toBeTrue();
    });

    it('does not emit new activeStepId values after destruction', fakeAsync(() => {
      const emitted: StepId[] = [];
      service.activeStepId$.subscribe(s => emitted.push(s));
      emitted.length = 0; // discard the initial BehaviorSubject replay

      service.ngOnDestroy();
      service.setActive(StepId.SaveImages);
      tick();

      expect(emitted).toEqual([]);
    }));

    it('does not emit new stepStatuses values after destruction', fakeAsync(() => {
      const emitted: Array<Record<StepId, StepStatus>> = [];
      service.stepStatuses$.subscribe(s => emitted.push(s));
      emitted.length = 0;

      service.ngOnDestroy();
      service.setStepStatus(StepId.DriveToPark, StepStatus.Success);
      tick();

      expect(emitted).toEqual([]);
    }));
  });
});

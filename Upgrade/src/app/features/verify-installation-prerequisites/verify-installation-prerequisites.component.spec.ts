import { ComponentFixture, TestBed, fakeAsync, tick, discardPeriodicTasks } from '@angular/core/testing';
import { BehaviorSubject } from 'rxjs';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

import { VerifyInstallationPrerequisitesComponent } from './verify-installation-prerequisites.component';
import { UiStateManagementService } from '../../core/update/ui-state-management-service';
import { CommunicationService } from '../../core/communication/communication.service';
import { LogService } from '../../core/log/log.service';
import { I18nService } from '../../core/i18n';
import { StepId, StepStatus } from '../../core/update/update.models';

describe('VerifyInstallationPrerequisitesComponent', () => {
  let component: VerifyInstallationPrerequisitesComponent;
  let fixture: ComponentFixture<VerifyInstallationPrerequisitesComponent>;
  let mockState: jasmine.SpyObj<UiStateManagementService>;
  let mockComm: jasmine.SpyObj<CommunicationService>;
  let mockLog: jasmine.SpyObj<LogService>;
  let mockI18n: jasmine.SpyObj<I18nService>;
  let stepStatusSubject: BehaviorSubject<Record<StepId, StepStatus>>;

  const defaultStatuses = (): Record<StepId, StepStatus> =>
    ({
      [StepId.Introduction]:      StepStatus.Pending,
      [StepId.VerifyPrereq]:      StepStatus.Active,
      [StepId.VerificationResult]:StepStatus.Pending,
      [StepId.SaveImages]:        StepStatus.Pending,
      [StepId.DriveToPark]:       StepStatus.Pending,
      [StepId.Installation]:      StepStatus.Pending,
    } as unknown as Record<StepId, StepStatus>);

  beforeEach(async () => {
    stepStatusSubject = new BehaviorSubject<Record<StepId, StepStatus>>(defaultStatuses());

    mockState = jasmine.createSpyObj('UiStateManagementService', ['next', 'setActive', 'setStepStatus'], {
      activeStepId$:  new BehaviorSubject<StepId>(StepId.VerifyPrereq).asObservable(),
      stepStatuses$:  stepStatusSubject.asObservable(),
    });
    mockComm  = jasmine.createSpyObj('CommunicationService', ['send', 'connect', 'shutdown']);
    mockLog   = jasmine.createSpyObj('LogService', ['debug', 'info', 'warn', 'error']);
    mockI18n  = jasmine.createSpyObj('I18nService', ['translate', 'setLanguage']);
    mockI18n.translate.and.callFake((key: string) => key);

    await TestBed.configureTestingModule({
      imports: [VerifyInstallationPrerequisitesComponent],
      providers: [
        { provide: UiStateManagementService, useValue: mockState },
        { provide: CommunicationService,     useValue: mockComm  },
        { provide: LogService,               useValue: mockLog   },
        { provide: I18nService,              useValue: mockI18n  },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
    }).compileComponents();

    fixture   = TestBed.createComponent(VerifyInstallationPrerequisitesComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => { if (fixture) fixture.destroy(); });

  // ── Creation ────────────────────────────────────────────────────────────────

  describe('Component creation', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('starts with progress 0', () => {
      expect(component.progress).toBe(0);
    });

    it('starts with statusTextKey "verification.status.inProgress"', () => {
      expect(component.statusTextKey).toBe('verification.status.inProgress');
    });

    it('starts with isError false', () => {
      expect(component.isError).toBeFalse();
    });

    it('starts with showAbortModal false', () => {
      expect(component.showAbortModal).toBeFalse();
    });
  });

  // ── ngOnInit ────────────────────────────────────────────────────────────────

  describe('ngOnInit', () => {
    beforeEach(() => fixture.detectChanges());

    it('logs info on entry', () => {
      expect(mockLog.info).toHaveBeenCalledWith(
        'VerifyPrerequisites', 'step.entered',
        'Entered Verify Installation Prerequisites step.'
      );
    });

    it('sends VerifyInstallationPrerequisite to backend', () => {
      expect(mockComm.send).toHaveBeenCalledWith('VerifyInstallationPrerequisite');
    });

    it('logs debug after sending backend request', () => {
      expect(mockLog.debug).toHaveBeenCalledWith(
        'VerifyPrerequisites', 'be.request',
        'Sent VerifyInstallationPrerequisite to backend.'
      );
    });

    it('does not expose or call flush', () => {
      expect((mockLog as any).flush).toBeUndefined();
    });
  });

  // ── stepStatuses$ subscription — success ─────────────────────────────────────

  describe('stepStatuses$ subscription', () => {
    beforeEach(() => fixture.detectChanges());

    it('sets progress to 100 and statusTextKey to success on "success" status', fakeAsync(() => {
      const updated = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'success' as unknown as StepStatus };
      stepStatusSubject.next(updated);
      tick();
      expect(component.progress).toBe(100);
      expect(component.statusTextKey).toBe('verification.status.success');
      expect(component.isError).toBeFalse();
    }));

    it('sets progress to 75, isError true and statusTextKey to error on "error" status', fakeAsync(() => {
      const updated = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'error' as unknown as StepStatus };
      stepStatusSubject.next(updated);
      tick();
      expect(component.progress).toBe(75);
      expect(component.statusTextKey).toBe('verification.status.error');
      expect(component.isError).toBeTrue();
    }));

    it('logs info when status changes to success', fakeAsync(() => {
      mockLog.info.calls.reset();
      const updated = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'success' as unknown as StepStatus };
      stepStatusSubject.next(updated);
      tick();
      expect(mockLog.info).toHaveBeenCalledWith(
        'VerifyPrerequisites', 'state.update', 'VerifyPrereq status: success'
      );
    }));

    it('logs info when status changes to error', fakeAsync(() => {
      mockLog.info.calls.reset();
      const updated = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'error' as unknown as StepStatus };
      stepStatusSubject.next(updated);
      tick();
      expect(mockLog.info).toHaveBeenCalledWith(
        'VerifyPrerequisites', 'state.update', 'VerifyPrereq status: error'
      );
    }));

    it('ignores "pending" and "active" statuses', fakeAsync(() => {
      mockLog.info.calls.reset();
      const updated = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'pending' as unknown as StepStatus };
      stepStatusSubject.next(updated);
      tick();
      expect(component.progress).toBe(0);
    }));

    it('does not navigate before the 2 s delay elapses', fakeAsync(() => {
      const updated = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'success' as unknown as StepStatus };
      stepStatusSubject.next(updated);
      tick(1_999);
      expect(mockState.setActive).not.toHaveBeenCalled();
      tick(1);
    }));

    it('calls setActive(VerificationResult) after 2 s delay on success', fakeAsync(() => {
      const updated = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'success' as unknown as StepStatus };
      stepStatusSubject.next(updated);
      tick(2_000);
      expect(mockState.setActive).toHaveBeenCalledWith(StepId.VerificationResult);
    }));

    it('calls setActive(VerificationResult) after 2 s delay on error', fakeAsync(() => {
      const updated = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'error' as unknown as StepStatus };
      stepStatusSubject.next(updated);
      tick(2_000);
      expect(mockState.setActive).toHaveBeenCalledWith(StepId.VerificationResult);
    }));

    it('logs navigate info when calling setActive', fakeAsync(() => {
      mockLog.info.calls.reset();
      const updated = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'success' as unknown as StepStatus };
      stepStatusSubject.next(updated);
      tick(2_000);
      expect(mockLog.info).toHaveBeenCalledWith(
        'VerifyPrerequisites', 'navigate',
        'Navigating to VerificationResult after delay'
      );
    }));
  });

  // ── progress animation ────────────────────────────────────────────────────────

  describe('progress animation', () => {
    it('increments progress by 1 each 250 ms tick', fakeAsync(() => {
      fixture.detectChanges();
      tick(250);
      expect(component.progress).toBe(1);
      tick(250);
      expect(component.progress).toBe(2);
      discardPeriodicTasks();
    }));

    it('caps progress at 75 while waiting', fakeAsync(() => {
      fixture.detectChanges();
      tick(250 * 80); // 80 ticks >> 75 cap
      expect(component.progress).toBe(75);
      discardPeriodicTasks();
    }));

    it('snaps to 100 on success regardless of animated value', fakeAsync(() => {
      fixture.detectChanges();
      tick(250 * 50); // animate to 50
      const updated = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'success' as unknown as StepStatus };
      stepStatusSubject.next(updated);
      tick();
      expect(component.progress).toBe(100);
      tick(2_000);
    }));

    it('snaps to 75 on error regardless of animated value', fakeAsync(() => {
      fixture.detectChanges();
      tick(250 * 30); // animate to 30
      const updated = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'error' as unknown as StepStatus };
      stepStatusSubject.next(updated);
      tick();
      expect(component.progress).toBe(75);
      tick(2_000);
    }));
  });

  // ── onAbort ──────────────────────────────────────────────────────────────────

  describe('onAbort', () => {
    beforeEach(() => fixture.detectChanges());

    it('sets showAbortModal to true', () => {
      component.onAbort();
      expect(component.showAbortModal).toBeTrue();
    });

    it('logs warn about abort modal', () => {
      component.onAbort();
      expect(mockLog.warn).toHaveBeenCalledWith(
        'VerifyPrerequisites', 'ui.abort',
        'User pressed Abort - showing confirmation modal.'
      );
    });
  });

  // ── onAbortConfirmed ──────────────────────────────────────────────────────────

  describe('onAbortConfirmed', () => {
    beforeEach(() => {
      fixture.detectChanges();
      component.showAbortModal = true;
    });

    it('sets showAbortModal to false', () => {
      component.onAbortConfirmed();
      expect(component.showAbortModal).toBeFalse();
    });

    it('logs warn about confirmed abort', () => {
      component.onAbortConfirmed();
      expect(mockLog.warn).toHaveBeenCalledWith(
        'VerifyPrerequisites', 'ui.abort.confirmed',
        'User confirmed abort - sending CloseApp to backend.'
      );
    });

    it('sends CloseApp to backend', () => {
      component.onAbortConfirmed();
      expect(mockComm.send).toHaveBeenCalledWith('CloseApp');
    });

    it('logs debug after sending CloseApp', () => {
      component.onAbortConfirmed();
      expect(mockLog.debug).toHaveBeenCalledWith(
        'VerifyPrerequisites', 'be.request',
        'Sent CloseApp to backend.'
      );
    });
  });

  // ── onAbortCancelled ──────────────────────────────────────────────────────────

  describe('onAbortCancelled', () => {
    beforeEach(() => {
      fixture.detectChanges();
      component.showAbortModal = true;
    });

    it('sets showAbortModal to false', () => {
      component.onAbortCancelled();
      expect(component.showAbortModal).toBeFalse();
    });

    it('logs info about cancelled abort', () => {
      component.onAbortCancelled();
      expect(mockLog.info).toHaveBeenCalledWith(
        'VerifyPrerequisites', 'ui.abort.cancelled',
        'User cancelled abort - verification continues.'
      );
    });

    it('does not send any backend message', () => {
      mockComm.send.calls.reset();
      component.onAbortCancelled();
      expect(mockComm.send).not.toHaveBeenCalled();
    });
  });
});

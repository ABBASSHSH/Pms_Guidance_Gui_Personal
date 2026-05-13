import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BehaviorSubject } from 'rxjs';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

import { VerificationResultComponent } from './verification-result.component';
import { UiStateManagementService } from '../../core/update/ui-state-management-service';
import { CommunicationService } from '../../core/communication/communication.service';
import { LogService } from '../../core/log/log.service';
import { I18nService } from '../../core/i18n';
import { StepId, StepStatus } from '../../core/update/update.models';

describe('VerificationResultComponent', () => {
  let component: VerificationResultComponent;
  let fixture: ComponentFixture<VerificationResultComponent>;
  let mockState: jasmine.SpyObj<UiStateManagementService>;
  let mockComm: jasmine.SpyObj<CommunicationService>;
  let mockLog: jasmine.SpyObj<LogService>;
  let mockI18n: jasmine.SpyObj<I18nService>;
  let stepStatusSubject: BehaviorSubject<Record<StepId, StepStatus>>;

  const defaultStatuses = (): Record<StepId, StepStatus> =>
    ({
      [StepId.Introduction]:       StepStatus.Pending,
      [StepId.VerifyPrereq]:       StepStatus.Success,
      [StepId.VerificationResult]: StepStatus.Active,
      [StepId.SaveImages]:         StepStatus.Pending,
      [StepId.DriveToPark]:        StepStatus.Pending,
      [StepId.Installation]:       StepStatus.Pending,
    } as unknown as Record<StepId, StepStatus>);

  beforeEach(async () => {
    stepStatusSubject = new BehaviorSubject<Record<StepId, StepStatus>>(defaultStatuses());

    mockState = jasmine.createSpyObj('UiStateManagementService', ['next'], {
      stepStatuses$: stepStatusSubject.asObservable(),
    });
    mockComm  = jasmine.createSpyObj('CommunicationService', ['send', 'connect', 'shutdown']);
    mockLog   = jasmine.createSpyObj('LogService', ['debug', 'info', 'warn', 'error']);
    mockI18n  = jasmine.createSpyObj('I18nService', ['translate', 'setLanguage']);
    mockI18n.translate.and.callFake((key: string) => key);

    await TestBed.configureTestingModule({
      imports: [VerificationResultComponent],
      providers: [
        { provide: UiStateManagementService, useValue: mockState },
        { provide: CommunicationService,     useValue: mockComm  },
        { provide: LogService,               useValue: mockLog   },
        { provide: I18nService,              useValue: mockI18n  },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
    }).compileComponents();

    fixture   = TestBed.createComponent(VerificationResultComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => fixture.destroy());

  // ── Creation ────────────────────────────────────────────────────────────────

  describe('Component creation', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('prereqOk defaults to false before detectChanges', () => {
      expect(component.prereqOk).toBeFalse();
    });
  });

  // ── ngOnInit — reads VerifyPrereq status ─────────────────────────────────────

  describe('ngOnInit prereqOk', () => {
    it('sets prereqOk to true when VerifyPrereq status is success', () => {
      const statuses = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'success' as unknown as StepStatus };
      stepStatusSubject.next(statuses);
      fixture.detectChanges();
      expect(component.prereqOk).toBeTrue();
    });

    it('sets prereqOk to false when VerifyPrereq status is error', () => {
      const statuses = { ...defaultStatuses(), [StepId.VerifyPrereq]: 'error' as unknown as StepStatus };
      stepStatusSubject.next(statuses);
      fixture.detectChanges();
      expect(component.prereqOk).toBeFalse();
    });

    it('updates prereqOk reactively when status changes', () => {
      fixture.detectChanges();
      stepStatusSubject.next({ ...defaultStatuses(), [StepId.VerifyPrereq]: 'success' as unknown as StepStatus });
      expect(component.prereqOk).toBeTrue();
      stepStatusSubject.next({ ...defaultStatuses(), [StepId.VerifyPrereq]: 'error' as unknown as StepStatus });
      expect(component.prereqOk).toBeFalse();
    });
  });

  // ── onProceedInstall ────────────────────────────────────────────────────────

  describe('onProceedInstall()', () => {
    it('logs info', () => {
      component.onProceedInstall();
      expect(mockLog.info).toHaveBeenCalledWith(
        'VerificationResultComponent', 'proceed', 'Proceed with installation clicked'
      );
    });

    it('calls state.next()', () => {
      component.onProceedInstall();
      expect(mockState.next).toHaveBeenCalledTimes(1);
    });

    it('logs before advancing state', () => {
      const order: string[] = [];
      mockLog.info.and.callFake(() => order.push('log'));
      mockState.next.and.callFake(() => order.push('next'));
      component.onProceedInstall();
      expect(order).toEqual(['log', 'next']);
    });

    it('does not send to backend', () => {
      component.onProceedInstall();
      expect(mockComm.send).not.toHaveBeenCalled();
    });
  });

  // ── onCancel ────────────────────────────────────────────────────────────────

  describe('onCancel()', () => {
    it('logs info', () => {
      component.onCancel();
      expect(mockLog.info).toHaveBeenCalledWith(
        'VerificationResultComponent', 'cancel', 'Cancel clicked'
      );
    });

    it('sends CloseApp', () => {
      component.onCancel();
      expect(mockComm.send).toHaveBeenCalledWith('CloseApp');
    });

    it('does not call state.next()', () => {
      component.onCancel();
      expect(mockState.next).not.toHaveBeenCalled();
    });
  });

  // ── onOk ────────────────────────────────────────────────────────────────────

  describe('onOk()', () => {
    it('logs info', () => {
      component.onOk();
      expect(mockLog.info).toHaveBeenCalledWith(
        'VerificationResultComponent', 'ok', 'OK clicked'
      );
    });

    it('does not call state.next()', () => {
      component.onOk();
      expect(mockState.next).not.toHaveBeenCalled();
    });

    it('does not send to backend', () => {
      component.onOk();
      expect(mockComm.send).not.toHaveBeenCalled();
    });
  });

  // ── onShowReport ─────────────────────────────────────────────────────────────

  describe('onShowReport()', () => {
    it('logs info', () => {
      component.onShowReport();
      expect(mockLog.info).toHaveBeenCalledWith(
        'VerificationResultComponent', 'showReport', 'Show report clicked'
      );
    });

    it('does not call state.next()', () => {
      component.onShowReport();
      expect(mockState.next).not.toHaveBeenCalled();
    });

    it('does not send to backend', () => {
      component.onShowReport();
      expect(mockComm.send).not.toHaveBeenCalled();
    });
  });

  // ── Logging consistency ─────────────────────────────────────────────────────

  describe('Logging consistency', () => {
    it('always uses "VerificationResultComponent" as source', () => {
      component.onProceedInstall();
      component.onCancel();
      component.onOk();
      component.onShowReport();
      mockLog.info.calls.all().forEach(c => expect(c.args[0]).toBe('VerificationResultComponent'));
    });

    it('uses distinct event keys per action', () => {
      component.onProceedInstall();
      component.onCancel();
      component.onOk();
      component.onShowReport();
      const events = mockLog.info.calls.all().map(c => c.args[1]);
      expect(events).toEqual(['proceed', 'cancel', 'ok', 'showReport']);
    });

    it('does not expose flush', () => {
      expect((mockLog as any).flush).toBeUndefined();
    });
  });

  // ── Multiple calls ──────────────────────────────────────────────────────────

  describe('Multiple calls', () => {
    it('handles multiple onProceedInstall calls', () => {
      component.onProceedInstall();
      component.onProceedInstall();
      expect(mockState.next).toHaveBeenCalledTimes(2);
      expect(mockLog.info).toHaveBeenCalledTimes(2);
    });

    it('handles multiple onCancel calls', () => {
      component.onCancel();
      component.onCancel();
      expect(mockComm.send).toHaveBeenCalledTimes(2);
    });
  });

  // ── Dependency injection ────────────────────────────────────────────────────

  describe('Dependency injection', () => {
    it('injects UiStateManagementService', () => {
      expect((component as any).uiStateUpdate).toBe(mockState);
    });

    it('injects CommunicationService', () => {
      expect((component as any).comm).toBe(mockComm);
    });

    it('injects LogService', () => {
      expect((component as any).log).toBe(mockLog);
    });
  });

  // ── ngOnDestroy ─────────────────────────────────────────────────────────────

  describe('ngOnDestroy', () => {
    it('stops updating prereqOk after component is destroyed', () => {
      stepStatusSubject.next({ ...defaultStatuses(), [StepId.VerifyPrereq]: 'success' as unknown as StepStatus });
      fixture.detectChanges();
      expect(component.prereqOk).toBeTrue();

      fixture.destroy();

      stepStatusSubject.next({ ...defaultStatuses(), [StepId.VerifyPrereq]: 'error' as unknown as StepStatus });
      expect(component.prereqOk).toBeTrue(); // subscription gone — value unchanged
    });

    it('does not react to new stepStatuses$ emissions after destruction', () => {
      fixture.detectChanges();
      fixture.destroy();

      const valueBefore = component.prereqOk;
      stepStatusSubject.next({ ...defaultStatuses(), [StepId.VerifyPrereq]: 'success' as unknown as StepStatus });
      expect(component.prereqOk).toBe(valueBefore);
    });
  });
});

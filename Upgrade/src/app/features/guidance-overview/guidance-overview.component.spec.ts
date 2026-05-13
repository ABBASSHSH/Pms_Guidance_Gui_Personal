import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { BehaviorSubject } from 'rxjs';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { GuidanceOverviewComponent } from './guidance-overview.component';
import { UiStateManagementService } from '../../core/update/ui-state-management-service';
import { I18nService } from '../../core/i18n';
import { StepId, StepStatus } from '../../core/update/update.models';

describe('GuidanceOverviewComponent', () => {
  let component: GuidanceOverviewComponent;
  let fixture: ComponentFixture<GuidanceOverviewComponent>;
  let mockState: jasmine.SpyObj<UiStateManagementService>;
  let mockI18n: jasmine.SpyObj<I18nService>;
  let statusSubject: BehaviorSubject<Record<StepId, StepStatus>>;

  const allPending = (): Record<StepId, StepStatus> => ({
    [StepId.Introduction]:       StepStatus.Pending,
    [StepId.VerifyPrereq]:       StepStatus.Pending,
    [StepId.VerificationResult]: StepStatus.Pending,
    [StepId.SaveImages]:         StepStatus.Pending,
    [StepId.DriveToPark]:        StepStatus.Pending,
    [StepId.Installation]:       StepStatus.Pending,
  } as Record<StepId, StepStatus>);

  beforeEach(async () => {
    statusSubject = new BehaviorSubject<Record<StepId, StepStatus>>(allPending());

    mockState = jasmine.createSpyObj('UiStateManagementService', ['next'], {
      stepStatuses$: statusSubject.asObservable(),
    });

    mockI18n = jasmine.createSpyObj('I18nService', ['translate', 'setLanguage']);
    mockI18n.translate.and.callFake((key: string) => key);

    await TestBed.configureTestingModule({
      imports: [GuidanceOverviewComponent],
      providers: [
        { provide: UiStateManagementService, useValue: mockState },
        { provide: I18nService,              useValue: mockI18n  },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
    }).compileComponents();

    fixture   = TestBed.createComponent(GuidanceOverviewComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => fixture.destroy());

  // ── Creation ────────────────────────────────────────────────────────────────

  describe('Component creation', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('has 6 steps defined', () => {
      expect(component.steps.length).toBe(6);
    });

    it('initial step order is correct', () => {
      const ids = component.steps.map(s => s.id);
      expect(ids).toEqual([
        StepId.Introduction,
        StepId.VerifyPrereq,
        StepId.VerificationResult,
        StepId.SaveImages,
        StepId.DriveToPark,
        StepId.Installation,
      ]);
    });
  });

  // ── ngOnInit ────────────────────────────────────────────────────────────────

  describe('ngOnInit', () => {
    it('subscribes to stepStatuses$', fakeAsync(() => {
      fixture.detectChanges();
      const statuses: Record<StepId, StepStatus> = {
        ...allPending(),
        [StepId.Introduction]: StepStatus.Active,
      };
      statusSubject.next(statuses);
      tick();
      const introStep = component.steps.find(s => s.id === StepId.Introduction)!;
      expect(introStep.status).toBe(StepStatus.Active);
    }));

    it('sets active=true when status is "active"', fakeAsync(() => {
      fixture.detectChanges();
      statusSubject.next({ ...allPending(), [StepId.VerifyPrereq]: StepStatus.Active } as Record<StepId, StepStatus>);
      tick();
      const step = component.steps.find(s => s.id === StepId.VerifyPrereq)!;
      expect(step.active).toBeTrue();
    }));

    it('sets active=false when status is not "active"', fakeAsync(() => {
      fixture.detectChanges();
      statusSubject.next({ ...allPending(), [StepId.SaveImages]: StepStatus.Success } as Record<StepId, StepStatus>);
      tick();
      const step = component.steps.find(s => s.id === StepId.SaveImages)!;
      expect(step.active).toBeFalse();
    }));

    it('updates all steps on each emission', fakeAsync(() => {
      fixture.detectChanges();
      const statuses: Record<StepId, StepStatus> = {
        [StepId.Introduction]:       StepStatus.Success,
        [StepId.VerifyPrereq]:       StepStatus.Success,
        [StepId.VerificationResult]: StepStatus.Active,
        [StepId.SaveImages]:         StepStatus.Pending,
        [StepId.DriveToPark]:        StepStatus.Pending,
        [StepId.Installation]:       StepStatus.Pending,
      } as Record<StepId, StepStatus>;
      statusSubject.next(statuses);
      tick();
      expect(component.steps.find(s => s.id === StepId.Introduction)!.status).toBe(StepStatus.Success);
      expect(component.steps.find(s => s.id === StepId.VerificationResult)!.active).toBeTrue();
      expect(component.steps.find(s => s.id === StepId.SaveImages)!.status).toBe(StepStatus.Pending);
    }));
  });

  // ── Step definitions ─────────────────────────────────────────────────────────

  describe('Step definitions', () => {
    it('Introduction step has label steps.introduction', () => {
      const step = component.steps.find(s => s.id === StepId.Introduction)!;
      expect(step.label).toBe('steps.introduction');
    });

    it('VerifyPrereq step has label steps.verifyPrereq', () => {
      const step = component.steps.find(s => s.id === StepId.VerifyPrereq)!;
      expect(step.label).toBe('steps.verifyPrereq');
    });

    it('all steps have labels defined', () => {
      component.steps.forEach(s => expect(s.label).toBeTruthy());
    });
  });

  // ── Dependency injection ────────────────────────────────────────────────────

  describe('Dependency injection', () => {
    it('injects UiStateManagementService', () => {
      expect((component as any).uiState).toBe(mockState);
    });
  });

  // ── ngOnDestroy ─────────────────────────────────────────────────────────────

  describe('ngOnDestroy', () => {
    it('stops updating steps after component is destroyed', fakeAsync(() => {
      fixture.detectChanges();
      fixture.destroy();

      const statusBefore = component.steps.find(s => s.id === StepId.VerifyPrereq)!.status;
      statusSubject.next({ ...allPending(), [StepId.VerifyPrereq]: StepStatus.Active } as Record<StepId, StepStatus>);
      tick();

      expect(component.steps.find(s => s.id === StepId.VerifyPrereq)!.status).toBe(statusBefore);
    }));

    it('does not set active=true for new emissions after destruction', fakeAsync(() => {
      fixture.detectChanges();
      fixture.destroy();

      statusSubject.next({ ...allPending(), [StepId.Installation]: StepStatus.Active } as Record<StepId, StepStatus>);
      tick();

      expect(component.steps.find(s => s.id === StepId.Installation)!.active).toBeFalse();
    }));
  });
});

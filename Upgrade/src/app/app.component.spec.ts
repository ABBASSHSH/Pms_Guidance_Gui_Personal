import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { BehaviorSubject } from 'rxjs';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

import { AppComponent } from './app.component';
import { UiStateManagementService } from './core/update/ui-state-management-service';
import { LogService } from './core/log/log.service';
import { CommunicationService } from './core/communication/communication.service';
import { Converter } from './core/update/converter';
import { I18nService } from './core/i18n';
import { StepId, StepStatus } from './core/update/update.models';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let mockState: jasmine.SpyObj<UiStateManagementService>;
  let mockLog: jasmine.SpyObj<LogService>;
  let mockComm: jasmine.SpyObj<CommunicationService>;
  let mockConverter: jasmine.SpyObj<Converter>;
  let mockI18n: jasmine.SpyObj<I18nService>;
  let activeStepSubject: BehaviorSubject<StepId>;

  beforeEach(async () => {
    activeStepSubject = new BehaviorSubject<StepId>(StepId.Introduction);

    mockState     = jasmine.createSpyObj('UiStateManagementService', ['next'], {
      activeStepId$: activeStepSubject.asObservable(),
      stepStatuses$: new BehaviorSubject<Record<StepId, StepStatus>>({
        [StepId.Introduction]: StepStatus.Active,
      } as Record<StepId, StepStatus>).asObservable(),
    });
    mockLog       = jasmine.createSpyObj('LogService', ['debug', 'info', 'warn', 'error']);
    mockComm      = jasmine.createSpyObj('CommunicationService', ['send', 'connect', 'shutdown']);
    mockConverter = jasmine.createSpyObj('Converter', ['start']);
    mockI18n      = jasmine.createSpyObj('I18nService', ['translate', 'setLanguage']);
    mockI18n.translate.and.callFake((key: string) => key);

    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [
        { provide: UiStateManagementService, useValue: mockState     },
        { provide: LogService,               useValue: mockLog       },
        { provide: CommunicationService,     useValue: mockComm      },
        { provide: Converter,                useValue: mockConverter  },
        { provide: I18nService,              useValue: mockI18n       },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
    }).compileComponents();

    fixture   = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => fixture.destroy());

  // ── Creation ────────────────────────────────────────────────────────────────

  describe('Component creation', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('exposes activeStepId$', () => {
      expect(component.activeStepId$).toBeDefined();
    });
  });

  // ── ngOnInit ────────────────────────────────────────────────────────────────

  describe('ngOnInit', () => {
    beforeEach(() => fixture.detectChanges());

    it('logs info "App started"', () => {
      expect(mockLog.info).toHaveBeenCalledWith('AppComponent', 'init', 'App started');
    });

    it('calls comm.connect()', () => {
      expect(mockComm.connect).toHaveBeenCalledTimes(1);
    });

    it('logs debug after comm.connect', () => {
      expect(mockLog.debug).toHaveBeenCalledWith('AppComponent', 'init', 'Communication connected');
    });

    it('calls converter.start()', () => {
      expect(mockConverter.start).toHaveBeenCalledTimes(1);
    });

    it('logs debug after converter.start', () => {
      expect(mockLog.debug).toHaveBeenCalledWith('AppComponent', 'init', 'Converter started');
    });

    it('does not expose or call flush', () => {
      expect((mockLog as any).flush).toBeUndefined();
    });

    it('executes in correct order: info, connect, debug, start, debug', () => {
      const order: string[] = [];
      // Re-run with fresh stubs to capture order
      mockLog.info.calls.reset();
      mockLog.debug.calls.reset();
      mockComm.connect.calls.reset();
      mockConverter.start.calls.reset();

      mockLog.info.and.callFake(() => order.push('info'));
      mockLog.debug.and.callFake(() => order.push('debug'));
      mockComm.connect.and.callFake(() => order.push('connect'));
      mockConverter.start.and.callFake(() => order.push('start'));

      component.ngOnInit();
      expect(order).toEqual(['info', 'connect', 'debug', 'start', 'debug']);
    });
  });

  // ── activeStepId$ ────────────────────────────────────────────────────────────

  describe('activeStepId$', () => {
    it('emits current step from UiStateManagementService', fakeAsync(() => {
      fixture.detectChanges();
      let step: StepId | undefined;
      component.activeStepId$.subscribe(s => (step = s));
      tick();
      expect(step).toBe(StepId.Introduction);
    }));

    it('emits updated step when state changes', fakeAsync(() => {
      fixture.detectChanges();
      activeStepSubject.next(StepId.VerifyPrereq);
      let step: StepId | undefined;
      component.activeStepId$.subscribe(s => (step = s));
      tick();
      expect(step).toBe(StepId.VerifyPrereq);
    }));
  });

  // ── ngOnDestroy ─────────────────────────────────────────────────────────────

  describe('ngOnDestroy', () => {
    it('calls comm.shutdown() when the component is destroyed', () => {
      fixture.detectChanges();
      fixture.destroy();
      expect(mockComm.shutdown).toHaveBeenCalledTimes(1);
    });

    it('does not call comm.shutdown() before the component is destroyed', () => {
      fixture.detectChanges();
      expect(mockComm.shutdown).not.toHaveBeenCalled();
    });

    it('calls comm.shutdown() exactly once even if ngOnDestroy is invoked again', () => {
      fixture.detectChanges();
      component.ngOnDestroy();
      component.ngOnDestroy();
      expect(mockComm.shutdown).toHaveBeenCalledTimes(2);
    });
  });

  // ── Dependency injection ────────────────────────────────────────────────────

  describe('Dependency injection', () => {
    it('injects UiStateManagementService', () => {
      expect((component as any).stepUpdate).toBe(mockState);
    });

    it('injects LogService', () => {
      expect((component as any).log).toBe(mockLog);
    });

    it('injects CommunicationService', () => {
      expect((component as any).comm).toBe(mockComm);
    });

    it('injects Converter', () => {
      expect((component as any).converter).toBe(mockConverter);
    });
  });
});

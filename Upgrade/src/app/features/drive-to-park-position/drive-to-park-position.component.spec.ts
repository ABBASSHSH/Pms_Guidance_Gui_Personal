import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { DriveToParkPositionComponent } from './drive-to-park-position.component';
import { UiStateManagementService } from '../../core/update/ui-state-management-service';
import { CommunicationService } from '../../core/communication/communication.service';
import { LogService } from '../../core/log/log.service';
import { I18nService } from '../../core/i18n';

describe('DriveToParkPositionComponent', () => {
  let component: DriveToParkPositionComponent;
  let fixture: ComponentFixture<DriveToParkPositionComponent>;
  let mockState: jasmine.SpyObj<UiStateManagementService>;
  let mockComm: jasmine.SpyObj<CommunicationService>;
  let mockLog: jasmine.SpyObj<LogService>;
  let mockI18n: jasmine.SpyObj<I18nService>;

  beforeEach(async () => {
    mockState = jasmine.createSpyObj('UiStateManagementService', ['next', 'setActive']);
    mockComm  = jasmine.createSpyObj('CommunicationService', ['send', 'connect', 'shutdown']);
    mockLog   = jasmine.createSpyObj('LogService', ['debug', 'info', 'warn', 'error']);
    mockI18n  = jasmine.createSpyObj('I18nService', ['translate', 'setLanguage']);
    mockI18n.translate.and.callFake((key: string) => key);

    await TestBed.configureTestingModule({
      imports: [DriveToParkPositionComponent],
      providers: [
        { provide: UiStateManagementService, useValue: mockState },
        { provide: CommunicationService,     useValue: mockComm  },
        { provide: LogService,               useValue: mockLog   },
        { provide: I18nService,              useValue: mockI18n  },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
    }).compileComponents();

    fixture   = TestBed.createComponent(DriveToParkPositionComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => fixture.destroy());

  // ── Creation ────────────────────────────────────────────────────────────────

  describe('Component creation', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('has source identifier "DriveToParkPositionComponent"', () => {
      expect((component as any).source).toBe('DriveToParkPositionComponent');
    });
  });

  // ── onProceed ───────────────────────────────────────────────────────────────

  describe('onProceed()', () => {
    beforeEach(() => {
      mockLog.info.calls.reset();
      mockState.next.calls.reset();
    });

    it('logs info', () => {
      component.onProceed();
      expect(mockLog.info).toHaveBeenCalledWith('DriveToParkPositionComponent', 'proceed', 'Proceed clicked');
    });

    it('calls state.next()', () => {
      component.onProceed();
      expect(mockState.next).toHaveBeenCalledTimes(1);
    });

    it('logs before advancing state', () => {
      const order: string[] = [];
      mockLog.info.and.callFake(() => order.push('log'));
      mockState.next.and.callFake(() => order.push('next'));
      component.onProceed();
      expect(order).toEqual(['log', 'next']);
    });

    it('does not call comm.send()', () => {
      component.onProceed();
      expect(mockComm.send).not.toHaveBeenCalled();
    });

    it('uses info level (not warn)', () => {
      component.onProceed();
      expect(mockLog.info).toHaveBeenCalled();
      expect(mockLog.warn).not.toHaveBeenCalled();
    });

    it('handles multiple clicks', () => {
      component.onProceed();
      component.onProceed();
      component.onProceed();
      expect(mockState.next).toHaveBeenCalledTimes(3);
    });
  });

  // ── onCancel ────────────────────────────────────────────────────────────────

  describe('onCancel()', () => {
    beforeEach(() => {
      mockLog.debug.calls.reset();
      mockLog.info.calls.reset();
      mockLog.warn.calls.reset();
      mockLog.error.calls.reset();
      mockComm.send.calls.reset();
      mockState.next.calls.reset();
    });

    it('logs warn', () => {
      component.onCancel();
      expect(mockLog.warn).toHaveBeenCalledWith('DriveToParkPositionComponent', 'cancel', 'Cancel clicked');
    });

    it('sends CloseApp', () => {
      component.onCancel();
      expect(mockComm.send).toHaveBeenCalledWith('CloseApp');
    });

    it('does not call state.next()', () => {
      component.onCancel();
      expect(mockState.next).not.toHaveBeenCalled();
    });

    it('uses warn level (not info)', () => {
      component.onCancel();
      expect(mockLog.warn).toHaveBeenCalled();
      expect(mockLog.info).not.toHaveBeenCalled();
    });

    it('handles multiple clicks', () => {
      component.onCancel();
      component.onCancel();
      expect(mockLog.warn).toHaveBeenCalledTimes(2);
      expect(mockComm.send).toHaveBeenCalledTimes(2);
    });
  });

  // ── Logging consistency ─────────────────────────────────────────────────────

  describe('Logging consistency', () => {
    it('always uses "DriveToParkPositionComponent" as source', () => {
      component.onProceed();
      component.onCancel();

      mockLog.info.calls.all().forEach(c => expect(c.args[0]).toBe('DriveToParkPositionComponent'));
      mockLog.warn.calls.all().forEach(c => expect(c.args[0]).toBe('DriveToParkPositionComponent'));
    });

    it('does not expose flush', () => {
      expect((mockLog as any).flush).toBeUndefined();
    });
  });

  // ── Template ────────────────────────────────────────────────────────────────

  describe('Template', () => {
    beforeEach(() => fixture.detectChanges());

    it('renders without errors', () => {
      expect(fixture.nativeElement).toBeTruthy();
    });

    it('has a content panel', () => {
      expect(fixture.nativeElement.querySelector('.content-panel')).toBeTruthy();
    });

    it('shows title', () => {
      expect(fixture.nativeElement.querySelector('.title')).toBeTruthy();
    });

    it('has proceed button', () => {
      expect(fixture.nativeElement.querySelector('sh-button[color="primary"]')).toBeTruthy();
    });

    it('has cancel button', () => {
      expect(fixture.nativeElement.querySelector('sh-button[color="secondary"]')).toBeTruthy();
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
});

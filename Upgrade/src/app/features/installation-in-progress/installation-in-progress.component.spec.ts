import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { InstallationInProgressComponent } from './installation-in-progress.component';
import { CommunicationService } from '../../core/communication/communication.service';
import { LogService } from '../../core/log/log.service';
import { I18nService } from '../../core/i18n';

describe('InstallationInProgressComponent', () => {
  let component: InstallationInProgressComponent;
  let fixture: ComponentFixture<InstallationInProgressComponent>;
  let mockComm: jasmine.SpyObj<CommunicationService>;
  let mockLog: jasmine.SpyObj<LogService>;
  let mockI18n: jasmine.SpyObj<I18nService>;

  beforeEach(async () => {
    mockComm = jasmine.createSpyObj('CommunicationService', ['send', 'connect', 'shutdown']);
    mockLog  = jasmine.createSpyObj('LogService', ['debug', 'info', 'warn', 'error']);
    mockI18n = jasmine.createSpyObj('I18nService', ['translate', 'setLanguage']);
    mockI18n.translate.and.callFake((key: string) => key);

    await TestBed.configureTestingModule({
      imports: [InstallationInProgressComponent],
      providers: [
        { provide: CommunicationService, useValue: mockComm },
        { provide: LogService,           useValue: mockLog  },
        { provide: I18nService,          useValue: mockI18n },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
    }).compileComponents();

    fixture   = TestBed.createComponent(InstallationInProgressComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => fixture.destroy());

  // ── Creation ────────────────────────────────────────────────────────────────

  describe('Component creation', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('has spinnerLabelKey set to installation.status.inProgress', () => {
      expect(component.spinnerLabelKey).toBe('installation.status.inProgress');
    });
  });

  // ── ngOnInit ────────────────────────────────────────────────────────────────

  describe('ngOnInit', () => {
    beforeEach(() => fixture.detectChanges());

    it('logs info on enter', () => {
      expect(mockLog.info).toHaveBeenCalledWith(
        'InstallationInProgressComponent', 'enter', 'Entered Installation In Progress step'
      );
    });

    it('sends InstallSoftware to backend', () => {
      expect(mockComm.send).toHaveBeenCalledWith('InstallSoftware');
    });

    it('logs debug after sending', () => {
      expect(mockLog.debug).toHaveBeenCalledWith(
        'InstallationInProgressComponent', 'be.request', 'Sent InstallSoftware to backend'
      );
    });

    it('logs in order: info, then send, then debug', () => {
      const order: string[] = [];
      // Reset and re-run in a new fixture to capture order
      mockLog.info.calls.reset();
      mockLog.debug.calls.reset();
      mockComm.send.calls.reset();

      mockLog.info.and.callFake(() => order.push('info'));
      mockComm.send.and.callFake(() => order.push('send'));
      mockLog.debug.and.callFake(() => order.push('debug'));

      component.ngOnInit();
      expect(order).toEqual(['info', 'send', 'debug']);
    });

    it('sends exactly once', () => {
      expect(mockComm.send).toHaveBeenCalledTimes(1);
    });
  });

  // ── Logging consistency ─────────────────────────────────────────────────────

  describe('Logging consistency', () => {
    beforeEach(() => fixture.detectChanges());

    it('uses "InstallationInProgressComponent" as source', () => {
      mockLog.info.calls.all().forEach(c  => expect(c.args[0]).toBe('InstallationInProgressComponent'));
      mockLog.debug.calls.all().forEach(c => expect(c.args[0]).toBe('InstallationInProgressComponent'));
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
  });

  // ── Dependency injection ────────────────────────────────────────────────────

  describe('Dependency injection', () => {
    it('injects CommunicationService', () => {
      expect((component as any).comm).toBe(mockComm);
    });

    it('injects LogService', () => {
      expect((component as any).log).toBe(mockLog);
    });
  });
});

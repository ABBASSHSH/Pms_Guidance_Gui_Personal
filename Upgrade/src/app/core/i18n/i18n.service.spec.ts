import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { I18nService } from './i18n.service';
import { LogService } from '../log/log.service';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

describe('I18nService', () => {
  let service: I18nService;
  let httpMock: HttpTestingController;
  let mockLog: jasmine.SpyObj<LogService>;

  const mockEn = { 'app.title': 'Software Upgrade', 'common.proceed': 'Proceed' };
  const mockDe = { 'app.title': 'Software-Upgrade',  'common.proceed': 'Weiter'  };

  beforeEach(() => {
    mockLog = jasmine.createSpyObj('LogService', ['debug', 'info', 'warn', 'error']);

    TestBed.configureTestingModule({
      imports: [],
      providers: [
        I18nService,
        { provide: LogService, useValue: mockLog },
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),
      ]
    });
    service  = TestBed.inject(I18nService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('setLanguage', () => {
    it('maps "English" to en.json and loads translations', () => {
      service.setLanguage('English');
      const req = httpMock.expectOne('assets/i18n/en.json');
      req.flush(mockEn);
      expect(service.translate('app.title')).toBe('Software Upgrade');
    });

    it('maps "German" to de.json and loads translations', () => {
      service.setLanguage('German');
      const req = httpMock.expectOne('assets/i18n/de.json');
      req.flush(mockDe);
      expect(service.translate('common.proceed')).toBe('Weiter');
    });

    it('falls back to en.json for unknown languages', () => {
      service.setLanguage('French');
      const req = httpMock.expectOne('assets/i18n/en.json');
      req.flush(mockEn);
      expect(service.translate('app.title')).toBe('Software Upgrade');
      expect(mockLog.info).toHaveBeenCalledWith('I18n', 'setLanguage', jasmine.stringContaining('"French"'));
    });

    it('falls back to en.json when asset file is missing', () => {
      service.setLanguage('German');
      httpMock.expectOne('assets/i18n/de.json').error(new ProgressEvent('error'));
      const fallbackReq = httpMock.expectOne('assets/i18n/en.json');
      fallbackReq.flush(mockEn);
      expect(service.translate('app.title')).toBe('Software Upgrade');
      expect(mockLog.warn).toHaveBeenCalledWith('I18n', 'loadTranslations', jasmine.stringContaining('falling back'));
    });

    it('does not loop when en.json itself is missing', () => {
      service.setLanguage('English');
      httpMock.expectOne('assets/i18n/en.json').error(new ProgressEvent('error'));
      // No further requests expected — no infinite retry loop
      httpMock.expectNone('assets/i18n/en.json');
      expect(mockLog.error).toHaveBeenCalledWith('I18n', 'loadTranslations', jasmine.stringContaining('en.json'));
    });

    it('skips the HTTP request when the same language is set again', () => {
      // First call loads the language
      service.setLanguage('English');
      httpMock.expectOne('assets/i18n/en.json').flush(mockEn);

      // Second call with the same language must not fire a new request
      service.setLanguage('English');
      httpMock.expectNone('assets/i18n/en.json');
      expect(service.translate('app.title')).toBe('Software Upgrade');
    });

    it('cancels an in-flight request when a new language is set', () => {
      service.setLanguage('German');
      // German request is in-flight — the subscription will be cancelled when
      // we switch to a new language before the response arrives
      const staleReq = httpMock.expectOne('assets/i18n/de.json');
      expect(staleReq.cancelled).toBeFalse();

      // Switching to English cancels the German subscription
      service.setLanguage('English');
      expect(staleReq.cancelled).toBeTrue(); // unsubscribe() cancelled the request

      // English response arrives and is applied
      httpMock.expectOne('assets/i18n/en.json').flush(mockEn);
      expect(service.translate('common.proceed')).toBe('Proceed'); // English value
    });
  });

  describe('translate', () => {
    beforeEach(() => {
      service.setLanguage('English');
      httpMock.expectOne('assets/i18n/en.json').flush(mockEn);
    });

    it('returns the translated value for a known key', () => {
      expect(service.translate('common.proceed')).toBe('Proceed');
    });

    it('returns the key itself when no translation exists', () => {
      expect(service.translate('unknown.key')).toBe('unknown.key');
    });

    it('returns empty string for an empty key', () => {
      // Empty key must never reach the translations map — the guard returns ''
      expect(service.translate('')).toBe('');
    });
  });
});
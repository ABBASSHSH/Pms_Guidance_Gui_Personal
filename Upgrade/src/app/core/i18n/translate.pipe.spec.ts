import { TranslatePipe } from './translate.pipe';
import { I18nService } from './i18n.service';

describe('TranslatePipe', () => {
  let pipe: TranslatePipe;
  let mockI18nService: jasmine.SpyObj<I18nService>;

  beforeEach(() => {
    mockI18nService = jasmine.createSpyObj('I18nService', ['translate']);
    pipe = new TranslatePipe(mockI18nService);
  });

  describe('Pipe Creation', () => {
    it('should create an instance', () => {
      expect(pipe).toBeTruthy();
    });
  });

  describe('transform', () => {
    it('should return translated value for valid key', () => {
      mockI18nService.translate.and.returnValue('Proceed');
      
      const result = pipe.transform('common.proceed');
      
      expect(result).toBe('Proceed');
      expect(mockI18nService.translate).toHaveBeenCalledWith('common.proceed');
    });

    it('should return empty string for empty key', () => {
      const result = pipe.transform('');
      
      expect(result).toBe('');
      expect(mockI18nService.translate).not.toHaveBeenCalled();
    });

    it('should return empty string for null-like key', () => {
      const result = pipe.transform(null as any);
      
      expect(result).toBe('');
    });

    it('should call translate service for each transform', () => {
      mockI18nService.translate.and.returnValue('Value');
      
      pipe.transform('key1');
      pipe.transform('key2');
      pipe.transform('key3');
      
      expect(mockI18nService.translate).toHaveBeenCalledTimes(3);
    });

    it('should handle key with dots', () => {
      mockI18nService.translate.and.returnValue('Software Upgrade');
      
      const result = pipe.transform('app.title');
      
      expect(result).toBe('Software Upgrade');
      expect(mockI18nService.translate).toHaveBeenCalledWith('app.title');
    });
  });
});

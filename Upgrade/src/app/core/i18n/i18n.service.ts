import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, EMPTY, Subscription } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { LogService } from '../log/log.service';

/**
 * Loads and provides UI translations.
 *
 * The backend sends `{ Action: 'ShowSystemLanguage', Language: 'English' }` (or
 * another language name).  `setLanguage()` maps the full name to the matching
 * asset file (`en.json`, `de.json`, ...) and falls back to English when the
 * file is not found.
 *
 * `translate()` is called synchronously by TranslatePipe on every change-
 * detection cycle and falls back to the key itself when no translation exists.
 */
@Injectable({ providedIn: 'root' })
export class I18nService {
  private readonly translations = new BehaviorSubject<Record<string, string>>({});
  private activeLoad: Subscription | null = null;
  private currentLanguageCode: string | null = null;

  constructor(
    private readonly http: HttpClient,
    private readonly log: LogService,
  ) {}

  /**
   * Load translations for the given language name (e.g. `'English'`, `'German'`).
   * Falls back to English when the corresponding asset file is missing.
   * Skips the load if the requested language is already active.
   * Cancels any in-flight request before starting a new one.
   */
  setLanguage(language: string): void {
    const languageCode = this.toFileLanguageCode(language);
    this.log.info('I18n', 'setLanguage', `Requested: "${language}" → file code: "${languageCode}"`);

    if (languageCode === this.currentLanguageCode) {
      this.log.debug('I18n', 'setLanguage', `Language "${languageCode}" already loaded — skipping`);
      return;
    }

    this.loadTranslations(languageCode);
  }

  /**
   * Returns the translated string for `key`, or the key itself as a fallback.
   * Returns an empty string for an empty key.
   */
  translate(key: string): string {
    if (!key) {
      return '';
    }
    return this.translations.value[key] ?? key;
  }

  // ---- private ----

  private loadTranslations(languageCode: string): void {
    this.log.debug('I18n', 'loadTranslations', `Fetching assets/i18n/${languageCode}.json`);

    // Cancel any in-flight load to prevent stale responses overwriting newer ones
    this.activeLoad?.unsubscribe();

    this.activeLoad = this.http.get<Record<string, string>>(`assets/i18n/${languageCode}.json`)
      .pipe(
        catchError(() => {
          if (languageCode !== 'en') {
            this.log.warn('I18n', 'loadTranslations', `Failed to load "${languageCode}.json" — falling back to "en"`);
            this.loadTranslations('en');
          } else {
            this.log.error('I18n', 'loadTranslations', 'Failed to load fallback "en.json" — UI will show translation keys');
          }
          return EMPTY; // swallow the error — fallback already triggered above
        }),
      )
      .subscribe(t => {
        this.log.info('I18n', 'loadTranslations', `Loaded ${Object.keys(t).length} keys for "${languageCode}"`);
        this.currentLanguageCode = languageCode;
        this.translations.next(t);
      });
  }

  /**
   * Maps the backend language value to the two-letter asset file languageCode.
   * Accepts both full language names (e.g. `'German'`) and BCP 47 tags
   * (e.g. `'de-DE'`, `'de'`).  Unknown languages fall back to `'en'`.
   */
  private toFileLanguageCode(language: string): string {
    const LANGUAGE_MAP: Record<string, string> = {
      english: 'en',
      german:  'de',
      en:      'en',
      de:      'de',
    };
    const normalized    = language.toLowerCase().trim();
    const primarySubtag = normalized.split('-')[0]; // 'de-DE' → 'de'
    return LANGUAGE_MAP[normalized] ?? LANGUAGE_MAP[primarySubtag] ?? 'en';
  }
}
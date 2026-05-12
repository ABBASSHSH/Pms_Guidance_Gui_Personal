import { Pipe, PipeTransform } from '@angular/core';
import { I18nService } from './i18n.service';

/**
 * Translate Pipe - Transforms translation keys to localized strings.
 * 
 * Usage in templates:
 *   {{ 'common.proceed' | t }}
 *   {{ 'introduction.title' | t }}
 * 
 * The pipe is impure (pure: false) to react to language changes.
 * When I18nService loads new translations, the pipe re-evaluates.
 */
@Pipe({
  name: 't',
  standalone: true,
  pure: false  // Re-evaluate when translations change
})
export class TranslatePipe implements PipeTransform {
  constructor(private readonly i18n: I18nService) {}

  /**
   * Transform a translation key to its localized value.
   * 
   * @param key - Translation key (e.g., 'common.proceed')
   * @returns Translated string or key if not found
   */
  transform(key: string): string {
    if (!key) {
      return '';
    }
    return this.i18n.translate(key);
  }
}

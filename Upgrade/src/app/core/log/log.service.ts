import { Injectable } from '@angular/core';
import { LogManager } from './log.manager';

/**
 * Application-level log facade.
 *
 * Signature: source · event · message  →  "[source.event] message"
 * Keeps call-sites expressive while centralising the formatting in one place.
 */
@Injectable({ providedIn: 'root' })
export class LogService {
  constructor(private readonly manager: LogManager) {}

  debug(source: string, event: string, message: string): void {
    this.manager.debug(source, `${event}: ${message}`);
  }

  info(source: string, event: string, message: string): void {
    this.manager.info(source, `${event}: ${message}`);
  }

  warn(source: string, event: string, message: string): void {
    this.manager.warn(source, `${event}: ${message}`);
  }

  error(source: string, event: string, message: string): void {
    this.manager.error(source, `${event}: ${message}`);
  }
}

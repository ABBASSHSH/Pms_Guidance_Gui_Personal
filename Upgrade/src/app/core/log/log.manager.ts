import { Injectable, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { LogLevel, ILog } from './log.models';
import { LogBus } from './log.bus';
import { CommunicationService } from '../communication/communication.service';

const LOG_LEVEL_ORDER: readonly LogLevel[] = ['debug', 'info', 'warn', 'error'];

@Injectable({ providedIn: 'root' })
export class LogManager implements ILog, OnDestroy {
  private readonly minLevel: LogLevel = 'debug';
  private readonly busSubscription: Subscription;

  constructor(
    private readonly comm: CommunicationService,
    private readonly bus:  LogBus
  ) {
    // Forward infrastructure log entries (CommunicationService, ConnectionManager)
    // to the console only — NOT to the backend transport — to avoid a feedback
    // loop where every comm.send() internally logs, which would trigger another send.
    this.busSubscription = this.bus.entries$.subscribe(entry => {
      if (!this.allowed(entry.level)) return;
      const ts  = this.timestamp();
      const msg = entry.message ?? '';
      console[entry.level](`[${ts}] [${entry.level.toUpperCase()}] [${entry.source}]: ${msg}`);
    });
  }

  debug(source: string, message?: string): void {
    this.write('debug', source, message);
  }

  info(source: string, message?: string): void {
    this.write('info', source, message);
  }

  warn(source: string, message?: string): void {
    this.write('warn', source, message);
  }

  error(source: string, message?: string): void {
    this.write('error', source, message);
  }

  // ---- private ----

  ngOnDestroy(): void {
    this.busSubscription.unsubscribe();
  }

  private write(level: LogLevel, source: string, message?: string): void {
    if (!this.allowed(level)) return;

    const msg  = message ?? '';
    const ts   = this.timestamp();
    // console[level](`[${ts}] [${level.toUpperCase()}] [${source}]: ${msg}`);
    this.comm.send('LogMessage', { Message: `[${level.toUpperCase()}] [${source}] ${msg}`, Timestamp: ts });
  }


  private timestamp(): string {
    return new Date().toISOString();
  }

  private allowed(level: LogLevel): boolean {
    return LOG_LEVEL_ORDER.indexOf(level) >= LOG_LEVEL_ORDER.indexOf(this.minLevel);
  }
}


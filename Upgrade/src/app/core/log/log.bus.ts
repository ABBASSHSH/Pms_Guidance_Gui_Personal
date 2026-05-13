import { Injectable, OnDestroy } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { LogEntry } from './log.models';

/**
 * Dependency-free log bus.
 *
 * Infrastructure services that sit below LogManager in the DI graph
 * (CommunicationService, ConnectionManager) push LogEntry values here
 * instead of injecting LogManager — which would create a circular dependency.
 *
 * LogManager subscribes to entries$ at construction time and forwards every
 * entry through its own write() pipeline (console + backend transport).
 *
 * Dependency graph (no cycles):
 *
 *   LogBus  ← (no deps)
 *     ↑ push                     ↑ subscribe
 *   ConnectionManager          LogManager → CommunicationService → ConnectionManager
 *   CommunicationService
 */
@Injectable({ providedIn: 'root' })
export class LogBus implements OnDestroy {
  private readonly subject = new Subject<LogEntry>();

  /** Stream of all log entries pushed by infrastructure services. */
  readonly entries$: Observable<LogEntry> = this.subject.asObservable();

  /** Push a log entry onto the bus. */
  push(entry: LogEntry): void {
    this.subject.next(entry);
  }

  ngOnDestroy(): void {
    this.subject.complete();
  }
}

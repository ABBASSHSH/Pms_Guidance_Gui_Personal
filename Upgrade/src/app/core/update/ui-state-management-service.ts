import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { StepId, StepStatus } from './update.models';
import { LogService } from '../log/log.service';


@Injectable({ providedIn: 'root' })
export class UiStateManagementService {

  private readonly activeStepSubject = new BehaviorSubject<StepId>(StepId.Introduction);
  private readonly stepStatusSubject = new BehaviorSubject<Record<number, StepStatus>>({[StepId.Introduction]: StepStatus.Active});

  readonly activeStepId$:  Observable<StepId> = this.activeStepSubject.asObservable();
  readonly stepStatuses$:  Observable<Record<StepId, StepStatus>> = this.stepStatusSubject.asObservable();

  constructor(private readonly log: LogService) {
    this.log.info('UiStateManagementService', 'init', 'State service initialized');
  }

  next(): void {
    const current = this.activeStepSubject.getValue();
    if (current >= StepId.Installation) {
      this.log.warn('UiStateManagementService', 'next', 'Already on last step');
      return;
    }
    const next = current + 1;
    this.log.info('UiStateManagementService', 'next', `${StepId[current]} -> ${StepId[next]}`);
    this.setStepStatus(current, StepStatus.Success);
    this.setStepStatus(next, StepStatus.Active);
    this.activeStepSubject.next(next);
  }

  setActive(stepId: StepId): void {
    this.log.info('UiStateManagementService', 'setActive', `-> ${StepId[stepId]}`);
    this.setStepStatus(stepId, StepStatus.Active);
    this.activeStepSubject.next(stepId);
  }

  setStepStatus(stepId: StepId, status: StepStatus): void {
    const current = this.stepStatusSubject.getValue();
    this.stepStatusSubject.next({ ...current, [stepId]: status });
    this.log.info('UiStateManagementService', 'setStepStatus', `${StepId[stepId]} -> ${status}`);
  }
}

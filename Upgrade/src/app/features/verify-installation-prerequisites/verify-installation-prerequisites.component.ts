import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit, DestroyRef, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { filter, map, switchMap, timer, interval } from 'rxjs';

import { UiStateManagementService } from '../../core/update/ui-state-management-service';
import { CommunicationService } from '../../core/communication/communication.service';
import { LogService } from '../../core/log/log.service';
import { StepId, StepStatus } from '../../core/update/update.models';
import { TranslatePipe } from '../../core/i18n';

@Component({
    selector: 'app-verify-installation-prerequisites',
    imports: [TranslatePipe],
    templateUrl: './verify-installation-prerequisites.component.html',
    styleUrls: ['./verify-installation-prerequisites.component.css'],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class VerifyInstallationPrerequisitesComponent implements OnInit {
  private static readonly TICK_INTERVAL_MS = 250;
  private static readonly PROGRESS_CAP     = 75;
  private static readonly RESULT_DELAY_MS  = 2_000;

  progress = 0;
  statusTextKey = 'verification.status.inProgress';
  isError = false;
  showAbortModal = false;

  private readonly destroyRef = inject(DestroyRef);

  constructor(
    private readonly uiStateUpdate: UiStateManagementService,
    private readonly comm: CommunicationService,
    private readonly log: LogService
  ) {}

  ngOnInit(): void {
    this.log.info('VerifyPrerequisites', 'step.entered', 'Entered Verify Installation Prerequisites step.');
    this.comm.send('VerifyInstallationPrerequisite');
    this.log.debug('VerifyPrerequisites', 'be.request', 'Sent VerifyInstallationPrerequisite to backend.');

    // Animate progress bar while waiting for the backend response.
    interval(VerifyInstallationPrerequisitesComponent.TICK_INTERVAL_MS)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        if (this.progress < VerifyInstallationPrerequisitesComponent.PROGRESS_CAP) {
          this.progress++;
        }
      });

    // React to the backend result: snap to final value then show result after delay.
    this.uiStateUpdate.stepStatuses$
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        map(statuses => statuses[StepId.VerifyPrereq]),
        filter(status => status === StepStatus.Success || status === StepStatus.Error),
        switchMap(status => {
          this.log.info('VerifyPrerequisites', 'state.update', `VerifyPrereq status: ${status}`);
          this.isError   = status === StepStatus.Error;
          this.progress  = status === StepStatus.Success ? 100 : 75;
          this.statusTextKey = status === StepStatus.Success ? 'verification.status.success' : 'verification.status.error';
          return timer(VerifyInstallationPrerequisitesComponent.RESULT_DELAY_MS);
        })
      )
      .subscribe(() => {
        this.log.info('VerifyPrerequisites', 'navigate', 'Navigating to VerificationResult after delay');
        this.uiStateUpdate.setActive(StepId.VerificationResult);
      });
  }

  onAbort(): void {
    this.log.warn('VerifyPrerequisites', 'ui.abort', 'User pressed Abort - showing confirmation modal.');
    this.showAbortModal = true;
  }

  onAbortConfirmed(): void {
    this.showAbortModal = false;
    this.log.warn('VerifyPrerequisites', 'ui.abort.confirmed', 'User confirmed abort - sending CloseApp to backend.');
    this.comm.send('CloseApp');
    this.log.debug('VerifyPrerequisites', 'be.request', 'Sent CloseApp to backend.');
  }

  onAbortCancelled(): void {
    this.showAbortModal = false;
    this.log.info('VerifyPrerequisites', 'ui.abort.cancelled', 'User cancelled abort - verification continues.');
  }
}

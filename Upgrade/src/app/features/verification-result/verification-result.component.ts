import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit, DestroyRef, inject } from '@angular/core';
import { map } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommunicationService } from '../../core/communication/communication.service';
import { UiStateManagementService } from '../../core/update/ui-state-management-service';
import { LogService } from '../../core/log/log.service';
import { StepId } from '../../core/update/update.models';
import { TranslatePipe } from '../../core/i18n';

@Component({
    selector: 'app-verification-result',
    imports: [TranslatePipe],
    templateUrl: './verification-result.component.html',
    styleUrls: ['./verification-result.component.css'],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class VerificationResultComponent implements OnInit {
  private readonly source = 'VerificationResultComponent';

  prereqOk = false;

  private readonly destroyRef = inject(DestroyRef);

  constructor(
    private readonly uiStateUpdate: UiStateManagementService,
    private readonly comm: CommunicationService,
    private readonly log: LogService
  ) {}

  ngOnInit(): void {
    this.uiStateUpdate.stepStatuses$
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        map(s => s[StepId.VerifyPrereq])
      )
      .subscribe(status => {
        this.prereqOk = status === 'success';
      });
  }

  onProceedInstall(): void {
    this.log.info(this.source, 'proceed', 'Proceed with installation clicked');
    this.uiStateUpdate.next();
  }

  onCancel(): void {
    this.log.info(this.source, 'cancel', 'Cancel clicked');
    this.comm.send('CloseApp');
  }

  onOk(): void {
    this.log.info(this.source, 'ok', 'OK clicked');
  }

  onShowReport(): void {
    this.log.info(this.source, 'showReport', 'Show report clicked');
  }
}

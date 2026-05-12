import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit } from '@angular/core';
import { StepId, StepStatus } from '../../core/update/update.models';
import { UiStateManagementService } from '../../core/update/ui-state-management-service';
import { TranslatePipe } from '../../core/i18n';

@Component({
    selector: 'app-guidance-overview',
    imports: [TranslatePipe],
    templateUrl: './guidance-overview.component.html',
    styleUrls: ['./guidance-overview.component.css'],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class GuidanceOverviewComponent implements OnInit {

  readonly steps: { id: StepId; status: StepStatus; label: string; active: boolean }[] = [
    { id: StepId.Introduction,       status: StepStatus.Active,  label: 'steps.introduction',       active: true  },
    { id: StepId.VerifyPrereq,       status: StepStatus.Pending, label: 'steps.verifyPrereq',       active: false },
    { id: StepId.VerificationResult, status: StepStatus.Pending, label: 'steps.verificationResult', active: false },
    { id: StepId.SaveImages,         status: StepStatus.Pending, label: 'steps.saveImages',         active: false },
    { id: StepId.DriveToPark,        status: StepStatus.Pending, label: 'steps.driveToPark',        active: false },
    { id: StepId.Installation,       status: StepStatus.Pending, label: 'steps.installation',       active: false },
  ];

  constructor(private readonly uiState: UiStateManagementService) {}

  ngOnInit(): void {
    this.uiState.stepStatuses$.subscribe(statuses => {
      this.steps.forEach(step => {
        step.status = statuses[step.id];
        step.active = step.status === StepStatus.Active;
      });
    });
  }
}

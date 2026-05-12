import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { UiStateManagementService } from '../../core/update/ui-state-management-service';
import { CommunicationService } from '../../core/communication/communication.service';
import { LogService } from '../../core/log/log.service';
import { TranslatePipe } from '../../core/i18n';

@Component({
    selector: 'app-save-patient-images',
    imports: [TranslatePipe],
    templateUrl: './save-patient-images.component.html',
    styleUrls: ['./save-patient-images.component.css'],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class SavePatientImagesComponent {
  private readonly source = 'SavePatientImagesComponent';

  constructor(
    private readonly uiStateUpdate: UiStateManagementService,
    private readonly comm: CommunicationService,
    private readonly log: LogService
  ) {}

  onProceed(): void {
    this.log.info(this.source, 'proceed', 'Proceed clicked');
    this.uiStateUpdate.next();
  }

  onCancel(): void {
    this.log.warn(this.source, 'cancel', 'Cancel clicked');
    this.comm.send('CloseApp');
  }
}

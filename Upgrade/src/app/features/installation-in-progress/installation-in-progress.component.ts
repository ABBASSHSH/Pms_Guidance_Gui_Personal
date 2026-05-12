import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit } from '@angular/core';

import { CommunicationService } from '../../core/communication/communication.service';
import { LogService } from '../../core/log/log.service';
import { TranslatePipe } from '../../core/i18n';

@Component({
    selector: 'app-installation-in-progress',
    imports: [TranslatePipe],
    templateUrl: './installation-in-progress.component.html',
    styleUrls: ['./installation-in-progress.component.css'],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class InstallationInProgressComponent implements OnInit {
  private readonly source = 'InstallationInProgressComponent';

  /** i18n key shown next to the spinner. */
  readonly spinnerLabelKey = 'installation.status.inProgress';

  constructor(
    private readonly comm: CommunicationService,
    private readonly log: LogService
  ) {}

  ngOnInit(): void {
    this.log.info(this.source, 'enter', 'Entered Installation In Progress step');
    this.comm.send('InstallSoftware');
    this.log.debug(this.source, 'be.request', 'Sent InstallSoftware to backend');
    // Navigation will be driven by the backend response via the Converter / HANDLERS.
  }
}

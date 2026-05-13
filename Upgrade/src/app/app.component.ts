
import { AsyncPipe } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, Component, OnInit, OnDestroy, DestroyRef, HostListener, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UiStateManagementService } from './core/update/ui-state-management-service';
import { GuidanceOverviewComponent } from './features/guidance-overview/guidance-overview.component';
import { IntroductionComponent } from './features/introduction/introduction.component';
import { SavePatientImagesComponent } from './features/save-patient-images/save-patient-images.component';
import { VerifyInstallationPrerequisitesComponent } from './features/verify-installation-prerequisites/verify-installation-prerequisites.component';
import { DriveToParkPositionComponent } from './features/drive-to-park-position/drive-to-park-position.component';
import { InstallationInProgressComponent } from './features/installation-in-progress/installation-in-progress.component';
import { VerificationResultComponent } from './features/verification-result/verification-result.component';

import { LogService } from './core/log/log.service';
import { TranslatePipe } from './core/i18n';
import { CommunicationService } from './core/communication/communication.service';
import { Converter } from './core/update/converter';

@Component({
    selector: 'app-root',
    imports: [
        AsyncPipe,
        TranslatePipe,
        GuidanceOverviewComponent,
        IntroductionComponent,
        SavePatientImagesComponent,
        VerifyInstallationPrerequisitesComponent,
        DriveToParkPositionComponent,
        InstallationInProgressComponent,
        VerificationResultComponent,
    ],
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppComponent implements OnInit, OnDestroy {

  private readonly destroyRef = inject(DestroyRef);

  readonly activeStepId$ = this.stepUpdate.activeStepId$;

  constructor(
    private readonly stepUpdate: UiStateManagementService,
    private readonly log: LogService,
    private readonly comm: CommunicationService,
    private readonly converter: Converter
  ) {}

  ngOnInit(): void {
    this.log.info('AppComponent', 'init', 'App started');

    this.comm.connect();
    this.log.debug('AppComponent', 'init', 'Communication connected');

    this.converter.start();
    this.log.debug('AppComponent', 'init', 'Converter started');
    this.comm.send('UIAppStarted');
    const browserLanguage = navigator.language || 'en-US';
    this.log.info('AppComponent', 'init', `Browser language: ${browserLanguage}`);
  }

  // @HostListener('document:contextmenu', ['$event'])
  // onContextMenu(event: MouseEvent): void {
  //   event.preventDefault();
  // }

  ngOnDestroy(): void {
    this.comm.shutdown();
  }
}

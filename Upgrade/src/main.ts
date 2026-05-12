import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
import { bootstrapShui } from './app/core/shui/shui.bootstrap';


async function main(): Promise<void> {

  bootstrapShui();

  await bootstrapApplication(AppComponent, appConfig);
}

main().catch(err => console.error(err));

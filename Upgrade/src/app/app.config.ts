import { ApplicationConfig } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';

import { MESSAGE_RECEIVER } from './core/communication/i-message-receiver.token';
import { ConnectionManager } from './core/communication/connection.manager';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(),
    // Bind the IMessageReceiver token to the concrete ConnectionManager singleton.
    // Keeps Converter decoupled from ConnectionManager (Dependency Inversion).
    { provide: MESSAGE_RECEIVER, useExisting: ConnectionManager },
  ],
};

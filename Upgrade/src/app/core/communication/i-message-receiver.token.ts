import { InjectionToken } from '@angular/core';
import { IMessageReceiver } from './i-message-receiver';

/**
 * Injection token for IMessageReceiver interface.
 * Use this token when injecting the message receiver service.
 * 
 * TypeScript interfaces don't exist at runtime, so we need an InjectionToken
 * to enable Angular's dependency injection for the IMessageReceiver interface.
 */
export const MESSAGE_RECEIVER = new InjectionToken<IMessageReceiver>('MESSAGE_RECEIVER');

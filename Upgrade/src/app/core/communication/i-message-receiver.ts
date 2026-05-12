import { Observable } from 'rxjs';
import { RawMessage } from './raw-message';

/**
 * Interface for receiving messages from the backend.
 * Defines contract for any class that can receive backend messages.
 * 
 * This abstraction allows the Converter to depend on an interface
 * rather than a concrete implementation (Dependency Inversion Principle).
 */
export interface IMessageReceiver {
  /**
   * Observable stream of messages received from the backend.
   * Emits whenever a new message arrives.
   * 
   * @returns Observable that emits backend messages
   */
  readonly messages$: Observable<RawMessage>;
}



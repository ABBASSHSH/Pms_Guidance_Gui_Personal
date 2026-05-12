/**
 * Represents a single raw message received from the WebView2 host after JSON parsing.
 *
 * At the communication layer the structure is not yet validated — any key may be present
 * and any value may appear.  The `Converter` narrows this to `BackendMessage` once the
 * `Action` field has been confirmed.
 */
export interface RawMessage {
  readonly [key: string]: unknown;
}

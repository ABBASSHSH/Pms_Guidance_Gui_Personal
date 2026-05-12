/** Severity level used internally by LogManager. */
export type LogLevel = 'debug' | 'info' | 'warn' | 'error';

export interface ILog {
  debug(source: string, message?: string): void;
  info(source: string, message?: string): void;
  warn(source: string, message?: string): void;
  error(source: string, message?: string): void;
}

/**
 * A single log entry emitted onto the LogBus by infrastructure services
 * (CommunicationService, ConnectionManager) that cannot inject LogManager
 * directly because of circular DI.
 */
export interface LogEntry {
  readonly level:   LogLevel;
  readonly source:  string;
  readonly message: string;
}

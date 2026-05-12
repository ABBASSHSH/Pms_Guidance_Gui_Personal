/** Identifies each GuidanceUI step in display order. */
export enum StepId {
  Introduction       = 0,
  VerifyPrereq       = 1,
  VerificationResult = 2,
  SaveImages         = 3,
  DriveToPark        = 4,
  Installation       = 5,
}

/** Lifecycle status of a single GuidanceUI step.
 *  Terminal values (success, error, warning) match the SHUI [type] attribute directly. */
export enum StepStatus {
  Pending = 'pending',
  Active = 'active',
  Success = 'success',
  Error = 'error',
  Warning = 'warning',
}

/** Result of the prerequisite-verification check. */
export enum PrereqStatus {
  OK      = 'OK',
  NotOk   = 'Not Ok',
  Unknown = 'unknown',
}



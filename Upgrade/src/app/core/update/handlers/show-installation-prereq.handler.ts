import { BackendMessage, ShowInstallationPrereqMsg } from '../message.interfaces';
import { HandlerContext } from '../message-handlers';
import { StepId, PrereqStatus, StepStatus } from '../update.models';

export class ShowInstallationPrereqHandler {
  static isValid(message: BackendMessage): message is ShowInstallationPrereqMsg {
    return (
      message.Action === 'ShowInstallationPrerequisite' &&
      'Status' in message &&
      (message.Status === PrereqStatus.OK || message.Status === PrereqStatus.NotOk)
    );
  }

  static handle(message: BackendMessage, context: HandlerContext): void {
    const { state, log } = context;

    if (!ShowInstallationPrereqHandler.isValid(message)) {
      log.error('Handler', 'ShowInstallationPrereq', `Invalid message structure: ${JSON.stringify(message)}`);
      return;
    }

    const prereqOk = message.Status === PrereqStatus.OK;
    log.info('Handler', 'ShowInstallationPrereq', `Processing status: ${message.Status}`);
    state.setStepStatus(
      StepId.VerifyPrereq,
      prereqOk ? StepStatus.Success : StepStatus.Error
    );
    log.debug('Handler', 'ShowInstallationPrereq', 'VerifyPrereq status set; component will navigate after delay');
  }
}

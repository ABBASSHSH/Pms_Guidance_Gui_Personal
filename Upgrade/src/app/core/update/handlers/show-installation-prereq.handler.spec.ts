import { ShowInstallationPrereqHandler } from './show-installation-prereq.handler';
import { PrereqStatus, StepId } from '../update.models';
import { ShowInstallationPrereqMsg } from '../message.interfaces';
import { HandlerContext } from '../message-handlers';

describe('ShowInstallationPrereqHandler', () => {
  let mockState: any;
  let mockLog: any;
  let context: HandlerContext;

  beforeEach(() => {
    mockState = jasmine.createSpyObj('UiStateManagementService', ['setStepStatus', 'setActive', 'next']);
    mockLog = jasmine.createSpyObj('LogService', ['info', 'debug', 'warn', 'error']);
    context = { state: mockState, log: mockLog, i18n: {} as any };
  });

  it('should validate correct message structure', () => {
    const msg: ShowInstallationPrereqMsg = { Action: 'ShowInstallationPrerequisite', Status: PrereqStatus.OK };
    expect(ShowInstallationPrereqHandler.isValid(msg)).toBeTrue();
  });

  it('should reject invalid message structure', () => {
    const msg = { Action: 'ShowInstallationPrerequisite', Status: 'Invalid' } as any;
    expect(ShowInstallationPrereqHandler.isValid(msg)).toBeFalse();
  });

  it('should call setPrereqStatus and complete for OK', () => {
    const msg: ShowInstallationPrereqMsg = { Action: 'ShowInstallationPrerequisite', Status: PrereqStatus.OK };
    ShowInstallationPrereqHandler.handle(msg, context);
    expect(mockState.setStepStatus).toHaveBeenCalledWith(StepId.VerifyPrereq, jasmine.any(String));
    expect(mockState.setActive).not.toHaveBeenCalled();
    expect(mockLog.debug).toHaveBeenCalledWith('Handler', 'ShowInstallationPrereq', 'VerifyPrereq status set; component will navigate after delay');
  });

  it('should call setPrereqStatus and error for NotOk', () => {
    const msg: ShowInstallationPrereqMsg = { Action: 'ShowInstallationPrerequisite', Status: PrereqStatus.NotOk };
    ShowInstallationPrereqHandler.handle(msg, context);
    expect(mockState.setStepStatus).toHaveBeenCalledWith(StepId.VerifyPrereq, jasmine.any(String));
    expect(mockState.setActive).not.toHaveBeenCalled();
    expect(mockLog.debug).toHaveBeenCalledWith('Handler', 'ShowInstallationPrereq', 'VerifyPrereq status set; component will navigate after delay');
  });

  it('should log error for invalid message', () => {
    const msg = { Action: 'ShowInstallationPrerequisite', Status: 'Invalid' } as any;
    ShowInstallationPrereqHandler.handle(msg, context);
    expect(mockLog.error).toHaveBeenCalledWith('Handler', 'ShowInstallationPrereq', jasmine.stringContaining('Invalid message structure'));
  });
});

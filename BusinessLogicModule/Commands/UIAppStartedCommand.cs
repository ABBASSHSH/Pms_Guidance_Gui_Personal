#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : UIAppStartedCommand.cs
// Description: Command that signals the Angular front end has finished initialising.
// Notes:
//--------------------------------------------------------------------
#endregion

using Infrastructure;

namespace BusinessLogicModule.Commands
{
    /// <summary>
    /// Command that signals the Angular front end has finished initialising and
    /// is ready to communicate with the backend.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class UIAppStartedCommand : ICommand
    {
    }
}

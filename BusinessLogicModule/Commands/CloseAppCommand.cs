#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : CloseAppCommand.cs
// Description: Command that instructs the application to shut down gracefully.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using Infrastructure;

namespace BusinessLogicModule.Commands
{
    /// <summary>
    /// Command that instructs the application to shut down gracefully.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class CloseAppCommand : ICommand
    {
    }
}

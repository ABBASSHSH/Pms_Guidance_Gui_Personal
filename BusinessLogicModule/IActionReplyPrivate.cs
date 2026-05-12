#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : IActionReplyPrivate.cs
// Description: Internal contract for invoking action reply events.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – added interface summary; compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

namespace BusinessLogicModule
{
    /// <summary>
    /// Internal contract used by command handlers to raise the
    /// <see cref="Infrastructure.IActionReply.OnCommandHandled"/> event after
    /// finishing their work.
    /// </summary>
    internal interface IActionReplyPrivate
    {
        /// <summary>
        /// Invokes the command handled event with the specified event arguments.
        /// </summary>
        /// <param name="theEventArgs">The event arguments to pass to subscribers.</param>
        void InvokeEvent(System.EventArgs theEventArgs);
    }
}

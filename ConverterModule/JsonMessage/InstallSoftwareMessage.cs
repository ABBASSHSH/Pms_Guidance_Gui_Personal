#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : InstallSoftwareMessage.cs
// Description: Represents the inbound JSON payload for the InstallSoftware action
//              received from the front end. No fields are required — the action
//              name alone is sufficient to trigger the command.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

namespace ConverterModule.JsonMessage
{
    /// <summary>
    /// Represents the inbound JSON payload for the <c>InstallSoftware</c>
    /// action received from the front end.
    /// </summary>
    /// <remarks>
    /// Expected JSON shape from the front end:
    /// <code>
    /// {
    ///     "Action": "InstallSoftware"
    /// }
    /// </code>
    /// No payload fields are required for this action.
    /// </remarks>
    internal class InstallSoftwareMessage
    {
    }
}

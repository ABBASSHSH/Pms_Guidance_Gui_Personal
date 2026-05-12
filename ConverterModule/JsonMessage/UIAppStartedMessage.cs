#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : UIAppStartedMessage.cs
// Description: Represents the inbound JSON payload for the UIAppStarted
//              action received from the front end. No fields are required — the
//              action name alone is sufficient to trigger the command.
// Notes:
//--------------------------------------------------------------------
#endregion

namespace ConverterModule.JsonMessage
{
    /// <summary>
    /// Represents the inbound JSON payload for the <c>UIAppStarted</c>
    /// action received from the front end.
    /// </summary>
    /// <remarks>
    /// Expected JSON shape from the front end:
    /// <code>
    /// {
    ///     "Action": "UIAppStarted"
    /// }
    /// </code>
    /// No payload fields are required for this action.
    /// </remarks>
    internal class UIAppStartedMessage
    {
    }
}

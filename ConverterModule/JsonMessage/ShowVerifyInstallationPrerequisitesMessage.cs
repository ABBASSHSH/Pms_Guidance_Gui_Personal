#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : ShowVerifyInstallationPrerequisitesMessage.cs
// Description: Outbound JSON payload sent to the front end with the prerequisites check result.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using Infrastructure;

namespace ConverterModule.JsonMessage
{
    /// <summary>
    /// Outbound JSON message sent to the front end with the prerequisites check result.
    /// </summary>
    internal class ShowVerifyInstallationPrerequisitesMessage : OutboundMessage
    {
        /// <inheritdoc/>
        public override string Action => "ShowInstallationPrerequisite";

        /// <summary>
        /// Gets or sets the prerequisite verification status.
        /// </summary>
        /// <value><c>OK</c> when prerequisites are satisfied; otherwise <c>Not Ok</c>.</value>
        public string Status { get; set; } = string.Empty;
    }
}

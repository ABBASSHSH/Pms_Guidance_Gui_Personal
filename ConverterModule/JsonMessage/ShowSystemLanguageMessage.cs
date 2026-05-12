#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : ShowSystemLanguageMessage.cs
// Description: Outbound JSON payload sent to the front end with the detected system language.
// Notes:
// Modification History : Abbas Bahrainwala, 05-May-2026, Initial creation
//--------------------------------------------------------------------
#endregion

using Infrastructure;

namespace ConverterModule.JsonMessage
{
    /// <summary>
    /// Outbound JSON message sent to the front end with the detected system UI language.
    /// </summary>
    /// <remarks>
    /// Serializes to:
    /// <code>
    /// {
    ///     "Action": "ShowSystemLanguage",
    ///     "Language": "en-US"
    /// }
    /// </code>
    /// </remarks>
    internal class ShowSystemLanguageMessage : OutboundMessage
    {
        /// <inheritdoc/>
        public override string Action => "ShowSystemLanguage";

        /// <summary>
        /// Gets or sets the BCP 47 language tag of the detected system UI culture
        /// (e.g. <c>"en-US"</c>).
        /// </summary>
        public string Language { get; set; } = string.Empty;
    }
}

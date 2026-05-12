#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : ShowSystemLanguageEventArgs.cs
// Description: Event arguments carrying the detected system UI language.
// Notes:
// Modification History : Abbas Bahrainwala, 05-May-2026, Initial creation
//--------------------------------------------------------------------
#endregion

using System;

namespace BusinessLogicModule.EventArgs
{
    /// <summary>
    /// Event arguments carrying the system UI language detected at startup.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class ShowSystemLanguageEventArgs : System.EventArgs
    {
        #region Public Members

        /// <summary>
        /// Gets the BCP 47 language tag of the detected system UI culture
        /// (e.g. <c>"en-US"</c>).
        /// </summary>
        public string Language { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ShowSystemLanguageEventArgs"/>.
        /// </summary>
        /// <param name="language">The detected system UI language tag.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="language"/> is null.
        /// </exception>
        public ShowSystemLanguageEventArgs(string language)
        {
            Language = language ?? throw new ArgumentNullException(nameof(language));
        }

        #endregion
    }
}

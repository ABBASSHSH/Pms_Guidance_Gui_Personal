#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConfigurationModule
// File   : SystemLanguageProvider.cs
// Description: Reads the system UI culture and exposes it via
//              ISystemLanguageProvider. Logs the language at startup.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//                        Abbas Bahrainwala, 05-May-2026, Extracted ISystemLanguageProvider interface
//--------------------------------------------------------------------
#endregion

using System;
using System.Globalization;
using Infrastructure;

namespace ConfigurationModule
{
    /// <summary>
    /// Reads the system UI language from <see cref="CultureInfo.CurrentUICulture"/>
    /// and logs it via <see cref="IBackendLogger"/>.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class SystemLanguageProvider : ISystemLanguageProvider, ILifeCycle
    {
        #region Public Members

        /// <summary>
        /// Initialises a new instance of <see cref="SystemLanguageProvider"/>.
        /// </summary>
        /// <param name="logger">Used to log the detected system UI language.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="logger"/> is null.
        /// </exception>
        public SystemLanguageProvider(ILogger logger)
        {
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns the current system UI culture name by reading
        /// <see cref="CultureInfo.CurrentUICulture"/>.
        /// </summary>
        /// <returns>The BCP 47 language tag of the current UI culture, e.g. <c>"en-US"</c>.</returns>
        public string FetchSystemLanguage()
        {
            return CultureInfo.CurrentUICulture.Name;
          
        }

        /// <summary>
        /// Fetches the system UI language and logs it.
        /// </summary>
        public void Open()
        {
            string language = FetchSystemLanguage();
            m_logger.LogInfo($"[SystemLanguageProvider] System UI language detected: {language}");
        }

        /// <summary>
        /// No resources to release — provided for <see cref="ILifeCycle"/> symmetry.
        /// </summary>
        public void Close()
        {
            // Nothing to clean up.
        }

        #endregion

        #region Private Members

        private readonly ILogger m_logger;

        #endregion
    }
}

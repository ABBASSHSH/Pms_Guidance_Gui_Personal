#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : ISystemLanguageProvider.cs
// Description: Defines the contract for fetching the system UI language.
// Notes:
// Modification History : Abbas Bahrainwala, 05-May-2026, Initial creation
//--------------------------------------------------------------------
#endregion

namespace Infrastructure
{
    /// <summary>
    /// Defines the contract for fetching the system UI language.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface ISystemLanguageProvider
    {
        /// <summary>
        /// Returns the current system UI culture name (e.g. <c>"en-US"</c>).
        /// </summary>
        /// <returns>The BCP 47 language tag of the current UI culture.</returns>
        string FetchSystemLanguage();
    }
}

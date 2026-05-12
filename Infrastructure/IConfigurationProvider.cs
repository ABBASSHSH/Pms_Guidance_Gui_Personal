#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : IConfigurationProvider.cs
// Description: Defines the contract for reading application configuration
//              as flat key-value pairs. Keys for nested JSON objects use
//              colon-separated notation, e.g. "SystemInterfaces:SystemCheck".
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System.Collections.Generic;

namespace Infrastructure
{
    /// <summary>
    /// Defines the contract for reading application configuration values.
    /// Configuration is exposed as flat key-value pairs; nested JSON objects
    /// are flattened using colon-separated keys,
    /// e.g. <c>SystemInterfaces:SystemCheck</c>.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Returns the configured verification command or executable path.
        /// </summary>
        /// <returns>
        /// A command string or executable path, or <c>null</c> when not configured.
        /// </returns>
        string? GetVerificationCommand();

        /// <summary>
        /// Returns the configured installation command or executable path.
        /// </summary>
        /// <returns>
        /// A command string or executable path, or <c>null</c> when not configured.
        /// </returns>
        string? GetInstallationCommand();

        /// <summary>
        /// Returns the value associated with <paramref name="key"/>,
        /// or <c>null</c> if the key is not present.
        /// </summary>
        /// <param name="key">
        /// The configuration key, e.g. <c>"InstallationPath"</c> or
        /// <c>"SystemInterfaces:SystemCheck"</c>.
        /// </param>
        /// <returns>The configuration value, or <c>null</c> if not found.</returns>
        string? GetValue(string key);

        /// <summary>
        /// Returns a read-only view of all configuration key-value pairs
        /// loaded from the configuration file.
        /// </summary>
        IReadOnlyDictionary<string, string> GetAll();
    }
}

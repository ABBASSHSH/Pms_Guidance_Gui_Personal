#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConfigurationModule
// File   : GuidanceConfigurationProvider.cs
// Description: Reads pms_guidance_configuration.json from disk, flattens
//              the JSON object tree into key-value pairs using colon-separated
//              keys, and exposes the result via IConfigurationProvider.
//              Nested objects are keyed as "Parent:Child", e.g.
//              "SystemInterfaces:SystemCheck".
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Infrastructure;

namespace ConfigurationModule
{
    /// <summary>
    /// Reads <c>pms_guidance_configuration.json</c> and exposes the parsed
    /// entries as flat key-value pairs via <see cref="IConfigurationProvider"/>.
    /// </summary>
    /// <remarks>
    /// The JSON file is expected to contain a single flat object whose properties
    /// are the configuration keys and whose string values are the configuration values.
    /// <para>
    /// Call <see cref="Open"/> before reading any values.
    /// </para>
    /// </remarks>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public sealed class GuidanceConfigurationProvider : IConfigurationProvider, ILifeCycle
    {
        #region Public Members

        /// <inheritdoc/>
        public string? GetVerificationCommand()
        {
            return GetValue(VerificationCommandKey);
        }

        /// <inheritdoc/>
        public string? GetInstallationCommand()
        {
            return GetValue(InstallationCommandKey);
        }

        /// <summary>
        /// Initialises a new <see cref="GuidanceConfigurationProvider"/>.
        /// </summary>
        /// <param name="configFilePath">
        /// Absolute path to <c>pms_guidance_configuration.json</c>.
        /// </param>
        /// <param name="logger">Used to log load and parse events.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="configFilePath"/> or
        /// <paramref name="logger"/> is null or whitespace.
        /// </exception>
        public GuidanceConfigurationProvider(string configFilePath, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(configFilePath))
            {
                throw new ArgumentNullException(nameof(configFilePath));
            }

            m_configFilePath = configFilePath;
            m_logger         = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Reads and parses the configuration file, populating the internal
        /// key-value store. Logs the number of entries loaded.
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the configuration file does not exist at the specified path.
        /// </exception>
        /// <exception cref="JsonException">
        /// Thrown when the configuration file contains invalid JSON.
        /// </exception>
        public void Open()
        {
            if (!File.Exists(m_configFilePath))
            {
                throw new FileNotFoundException(
                    "Configuration file not found.");
            }

            string json = File.ReadAllText(m_configFilePath);

            using JsonDocument document = JsonDocument.Parse(json);
            foreach (JsonProperty property in document.RootElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.String)
                {
                    m_entries[property.Name] = property.Value.GetString()!;
                }
            }

            m_logger.LogInfo(
                $"[GuidanceConfigurationProvider] Loaded {m_entries.Count} configuration entries from {m_configFilePath}");
        }

        /// <summary>
        /// Clears the loaded configuration entries and releases resources.
        /// </summary>
        public void Close()
        {
            m_entries.Clear();
            m_logger.LogInfo("[GuidanceConfigurationProvider] Configuration entries cleared.");
        }

        /// <inheritdoc/>
        public string? GetValue(string key)
        {
            m_entries.TryGetValue(key, out string? value);
            return value;
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, string> GetAll()
        {
            return m_entries;
        }

        #endregion

        #region Private Members

        private const string VerificationCommandKey = "InstallationPrerequisitesVerify";
        private const string InstallationCommandKey = "Installationtrigger";

        private readonly string                     m_configFilePath;
        private readonly ILogger                    m_logger;
        private readonly Dictionary<string, string> m_entries = new Dictionary<string, string>();

        #endregion
    }
}

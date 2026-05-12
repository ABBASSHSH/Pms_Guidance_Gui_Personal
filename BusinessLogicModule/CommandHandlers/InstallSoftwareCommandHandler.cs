#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : InstallSoftwareCommandHandler.cs
// Description: Handles InstallSoftwareCommand by running the software installation
//              and firing an event with the result.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using System.Diagnostics;
using System.IO;
using BusinessLogicModule.Commands;
using BusinessLogicModule.EventArgs;
using Infrastructure;

namespace BusinessLogicModule
{
    /// <summary>
    /// Handles <see cref="InstallSoftwareCommand"/> by running the software installation
    /// and reporting the result via <see cref="IActionReplyPrivate"/>.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class InstallSoftwareCommandHandler : AbstractCommandHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public override Type CommandType => typeof(InstallSoftwareCommand);

        #endregion

        #region Protected Members

        /// <inheritdoc/>
        protected override void ExecuteCommand(ICommand theCommand)
        {
            bool isInstalled;

            m_logger.LogInfo("InstallSoftware command received.");

            try
            {
                isInstalled = ExecuteConfiguredCommand(
                    m_configurationProvider.GetInstallationCommand(),
                    "installation");
                m_logger.LogInfo($"Installation execution completed. Result={(isInstalled ? "PASS" : "FAIL")}");
            }
            catch (Exception ex)
            {
                isInstalled = false;
                m_logger.LogError("Error during installation.", ex);
            }

            m_actionReplyPrivate.InvokeEvent(
                new InstallSoftwareStatusEventArgs(isInstalled));
        }

        #endregion

        #region Private Members

        private readonly IConfigurationProvider m_configurationProvider;

        internal InstallSoftwareCommandHandler(
            IActionReplyPrivate replyPrivate,
            ILogger logger,
            IConfigurationProvider configurationProvider)
            : base(replyPrivate, logger)
        {
            m_configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        }

        private bool ExecuteConfiguredCommand(string? configuredCommand, string operationName)
        {
            if (string.IsNullOrWhiteSpace(configuredCommand))
            {
                m_logger.LogWarn($"The configured {operationName} command is missing or empty.");
                return false;
            }

            string commandToRun = configuredCommand;
            if (File.Exists(configuredCommand))
            {
                commandToRun = $"\"{configuredCommand}\"";
            }

            var startInfo = new ProcessStartInfo("cmd.exe", $"/c {commandToRun}")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process? process = Process.Start(startInfo);
            if (process == null)
            {
                m_logger.LogError($"Failed to start process for {operationName}.");
                return false;
            }

            process.WaitForExit();
            m_logger.LogInfo($"The {operationName} command exited with code {process.ExitCode}.");
            return process.ExitCode == 1;
        }

        #endregion
    }
}

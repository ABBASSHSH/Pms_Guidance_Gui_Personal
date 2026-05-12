#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : VerifyInstallationPrerequisitesCommandHandler.cs
// Description: Handles VerifyInstallationPrerequisitesCommand by checking system prerequisites
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
    /// Handles <see cref="VerifyInstallationPrerequisitesCommand"/> by checking system
    /// prerequisites and firing the result via <see cref="IActionReplyPrivate"/>.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class VerifyInstallationPrerequisitesCommandHandler : AbstractCommandHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public override Type CommandType => typeof(VerifyInstallationPrerequisitesCommand);

        #endregion

        #region Protected Members

        /// <inheritdoc/>
        protected override void ExecuteCommand(ICommand theCommand)
        {
            bool prerequisitesMet;

            m_logger.LogInfo("VerifyInstallationPrerequisite command received.");

            try
            {
                prerequisitesMet = ExecuteConfiguredCommand(
                    m_configurationProvider.GetVerificationCommand(),
                    "verification");
                m_logger.LogInfo($"Prerequisite verification completed. Result={(prerequisitesMet ? "PASS" : "FAIL")}");
            }
            catch (Exception ex)
            {
                prerequisitesMet = false;
                m_logger.LogError("Error during prerequisites check.", ex);
            }

            m_actionReplyPrivate.InvokeEvent(
                new VerifyInstallationPrerequisitesStatusEventArgs(prerequisitesMet));
        }

        #endregion

        #region Private Members

        private readonly IConfigurationProvider m_configurationProvider;

        internal VerifyInstallationPrerequisitesCommandHandler(
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

            if (!process.WaitForExit(ProcessWaitTimeoutMilliseconds))
            {
                m_logger.LogError($"The {operationName} command did not finish within the timeout.");
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch (Exception ex)
                {
                    m_logger.LogError($"Failed to terminate timed-out {operationName} process.", ex);
                }

                return false;
            }

            m_logger.LogInfo($"The {operationName} command exited with code {process.ExitCode}.");
            return process.ExitCode == 1;
        }

        private const int ProcessWaitTimeoutMilliseconds = 30000;

        #endregion
    }
}

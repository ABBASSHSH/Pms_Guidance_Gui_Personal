#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : ILogger.cs
// Description: Unified logging contract for both backend and frontend log entries.
//              All log entries — regardless of origin — are written to the same
//              log file through a single interface.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//                        Abbas Bahrainwala, 08-May-2026, Unified IBackendLogger and IFrontendLogger into ILogger
//                        Abbas Bahrainwala, 11-May-2026, Removed LogFrontend; level routing moved to LogActionCommandHandler
//--------------------------------------------------------------------
#endregion

using System.Runtime.CompilerServices;

namespace Infrastructure
{
    /// <summary>
    /// Unified logging contract for backend-originated and frontend-originated log entries.
    /// Implementations must be thread-safe and append-only.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface ILogger
    {
        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerMember">Automatically supplied by the compiler — name of the calling method.</param>
        /// <param name="callerFile">Automatically supplied by the compiler — source file of the caller.</param>
        /// <param name="callerLine">Automatically supplied by the compiler — line number of the call.</param>
        void LogInfo(string message,
            [CallerMemberName] string callerMember = "",
            [CallerFilePath]   string callerFile   = "",
            [CallerLineNumber] int    callerLine   = 0);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerMember">Automatically supplied by the compiler — name of the calling method.</param>
        /// <param name="callerFile">Automatically supplied by the compiler — source file of the caller.</param>
        /// <param name="callerLine">Automatically supplied by the compiler — line number of the call.</param>
        void LogDebug(string message,
            [CallerMemberName] string callerMember = "",
            [CallerFilePath]   string callerFile   = "",
            [CallerLineNumber] int    callerLine   = 0);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerMember">Automatically supplied by the compiler — name of the calling method.</param>
        /// <param name="callerFile">Automatically supplied by the compiler — source file of the caller.</param>
        /// <param name="callerLine">Automatically supplied by the compiler — line number of the call.</param>
        void LogWarn(string message,
            [CallerMemberName] string callerMember = "",
            [CallerFilePath]   string callerFile   = "",
            [CallerLineNumber] int    callerLine   = 0);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">Optional exception to include in the log entry.</param>
        /// <param name="callerMember">Automatically supplied by the compiler — name of the calling method.</param>
        /// <param name="callerFile">Automatically supplied by the compiler — source file of the caller.</param>
        /// <param name="callerLine">Automatically supplied by the compiler — line number of the call.</param>
        void LogError(string message,
            Exception?         ex           = null,
            [CallerMemberName] string callerMember = "",
            [CallerFilePath]   string callerFile   = "",
            [CallerLineNumber] int    callerLine   = 0);

    }
}

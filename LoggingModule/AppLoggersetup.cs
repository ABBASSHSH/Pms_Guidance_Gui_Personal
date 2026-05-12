#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : LoggingModule
// File   : AppLoggerSetup.cs
// Description: Creates a single ILogger that writes all log entries — both backend
//              and frontend — to the same app.log file through a shared FileLogWriter.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//                        Abbas Bahrainwala, 08-May-2026, Unified logger pair into single ILogger
//--------------------------------------------------------------------
#endregion

using System;
using Infrastructure;

namespace LoggingModule
{
    /// <summary>
    /// Factory that creates a single <see cref="ILogger"/> backed by a <see cref="FileLogWriter"/>.
    /// </summary>
    /// <remarks>
    /// All writes — backend log entries and frontend log entries — are serialised
    /// through one <see cref="FileLogWriter"/> lock so no interleaving occurs.
    /// Usage:
    /// <code>
    /// ILogger logger = AppLoggerSetup.Create(AppDomain.CurrentDomain.BaseDirectory);
    /// </code>
    /// </remarks>
    public static class AppLoggerSetup
    {
        /// <summary>
        /// Creates an <see cref="ILogger"/> that writes to
        /// <c>logs/app.log</c> inside <paramref name="applicationFolder"/>.
        /// </summary>
        /// <param name="applicationFolder">
        /// Root folder for the log directory — typically
        /// <see cref="AppDomain.CurrentDomain.BaseDirectory"/>.
        /// </param>
        /// <returns>A single <see cref="ILogger"/> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="applicationFolder"/> is null or whitespace.
        /// </exception>
        public static ILogger Create(string applicationFolder)
        {
            var writer = new FileLogWriter(applicationFolder);
            return new SourceLogger(writer);
        }
    }
}

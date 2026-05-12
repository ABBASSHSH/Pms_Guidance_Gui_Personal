#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : LoggingModule
// File   : FileLogWriter.cs
// Description: Internal, thread-safe append-only file-write sink.
//              Accepts pre-formatted log text and serialises concurrent
//              writes through a single lock — necessary because the logger
//              can be called simultaneously from the WebView callback thread
//              and the UI/task threads.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using System.IO;
using System.Text;

namespace LoggingModule
{
    /// <summary>
    /// Thread-safe append-only file-write sink.
    /// This class is intentionally <c>internal</c> — callers outside this
    /// assembly always interact through the <see cref="ILogger"/> contract.
    /// Use <see cref="AppLoggerSetup"/> to obtain an <see cref="ILogger"/> instance.
    /// </summary>
    internal sealed class FileLogWriter : ILogWriter
    {
        #region Internal Members

        /// <summary>
        /// Initialises a new writer that appends to
        /// <c>&lt;applicationFolder&gt;/logs/app.log</c>.
        /// The <c>logs/</c> sub-directory is created if it does not exist.
        /// </summary>
        /// <param name="applicationFolder">
        /// Root folder for the log directory — typically
        /// <see cref="AppDomain.CurrentDomain.BaseDirectory"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="applicationFolder"/> is null or whitespace.
        /// </exception>
        internal FileLogWriter(string applicationFolder)
        {
            if (string.IsNullOrWhiteSpace(applicationFolder))
            {
                throw new ArgumentNullException(nameof(applicationFolder));
            }

            string logsFolder = Path.Combine(applicationFolder, "logs");
            Directory.CreateDirectory(logsFolder);
            m_logFilePath = Path.Combine(logsFolder, "app.log");
        }

        /// <summary>
        /// Appends <paramref name="text"/> to the log file.
        /// The write is serialised by an exclusive lock because the logger
        /// may be called from multiple threads concurrently (e.g. WebView
        /// callback thread and UI thread).
        /// </summary>
        /// <param name="text">Pre-formatted log entry text to append.</param>
        /// <inheritdoc/>
        public void Write(string text)
        {
            lock (m_lock)
            {
                File.AppendAllText(m_logFilePath, text, Encoding.UTF8);
            }
        }

        #endregion

        #region Private Members

        private readonly string m_logFilePath;
        private readonly object m_lock = new object();

        #endregion
    }
}

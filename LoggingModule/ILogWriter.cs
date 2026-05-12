#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : LoggingModule
// File   : ILogWriter.cs
// Description: Internal abstraction for the log write sink.
//              Decouples SourceLogger and FrontendLogger from the concrete
//              FileLogWriter so that either can be tested with a mock writer
//              without touching the file system.
// Notes:
// Modification History : Abbas Bahrainwala, 05-May-2026, Extracted from FileLogWriter
//                        to support internal dependency injection for testability.
//--------------------------------------------------------------------
#endregion

namespace LoggingModule
{
    /// <summary>
    /// Internal contract for a log write sink.
    /// </summary>
    /// <remarks>
    /// This interface is intentionally <c>internal</c> — it is an implementation
    /// detail of the LoggingModule.  External consumers always interact through
    /// <see cref="Infrastructure.IBackendLogger"/> or
    /// <see cref="Infrastructure.IFrontendLogger"/>.
    /// The test project gains access via <c>InternalsVisibleTo</c>.
    /// </remarks>
    internal interface ILogWriter
    {
        /// <summary>
        /// Appends <paramref name="text"/> to the underlying log destination.
        /// </summary>
        /// <param name="text">Pre-formatted log entry text to write.</param>
        void Write(string text);
    }
}

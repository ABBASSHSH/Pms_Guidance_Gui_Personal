#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : LoggerTests.cs
// Description: Unit tests for the LoggingModule (AppLoggerSetup, FileLogWriter,
//              SourceLogger, LogEntryFormatter) and SystemLanguageProvider.
//              Internal logging classes are exercised through the public
//              AppLoggerSetup API; all file I/O uses per-test temp directories
//              that are cleaned up in TestCleanup.
//              All tests follow the Given/When/Then naming convention.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConfigurationModule;
using Infrastructure;
using LoggingModule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pms_GuidanceGUI.Tests.Unit
{
    // ─────────────────────────────────────────────────────────────────────────────
    // SystemLanguageProvider tests (kept from the original file)
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Unit tests for <see cref="SystemLanguageProvider"/>.
    /// <see cref="ILogger"/> is mocked so no file I/O occurs.
    /// </summary>
    [TestClass]
    public class SystemLanguageProviderTests_Legacy
    {
        // ── Constructor guards ────────────────────────────────────────────────────

        /// <summary>
    /// Given SystemLanguageProvider Legacy NullLogger When Constructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_SystemLanguageProvider_Legacy_NullLogger_When_Constructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new SystemLanguageProvider(null!));
        }

        // ── Open ─────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given SystemLanguageProvider Legacy ValidLogger When OpenCalled Then LogInfoIsCalledWithLanguageName
        /// </summary>
        [TestMethod]
        public void Given_SystemLanguageProvider_Legacy_ValidLogger_When_OpenCalled_Then_LogInfoIsCalledWithLanguageName()
        {
            var mockLogger = new Mock<ILogger>();
            var provider   = new SystemLanguageProvider(mockLogger.Object);

            provider.Open();

            mockLogger.Verify(
                x => x.LogInfo(It.Is<string>(s => s.Contains("System UI language")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        /// <summary>
    /// Given SystemLanguageProvider Legacy ValidLogger When OpenCalled Then LoggedMessageContainsDetectedLanguage
        /// </summary>
        [TestMethod]
        public void Given_SystemLanguageProvider_Legacy_ValidLogger_When_OpenCalled_Then_LoggedMessageContainsDetectedLanguage()
        {
            string? loggedMessage = null;
            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                      .Callback<string, string, string, int>((msg, _, _, _) => loggedMessage = msg);

            new SystemLanguageProvider(mockLogger.Object).Open();

            Assert.IsNotNull(loggedMessage);
            Assert.IsTrue(loggedMessage.Length > 0);
        }

        // ── Close ────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given SystemLanguageProvider Legacy ValidProvider When CloseCalled Then NoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_SystemLanguageProvider_Legacy_ValidProvider_When_CloseCalled_Then_NoExceptionThrown()
        {
            var provider = new SystemLanguageProvider(new Mock<ILogger>().Object);
            provider.Open();

            bool threw = false;
            try { provider.Close(); } catch { threw = true; }

            Assert.IsFalse(threw);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // LoggingModule tests — AppLoggerSetup / FileLogWriter / SourceLogger /
    //                       LogEntryFormatter
    // All internal types are exercised through the public AppLoggerSetup API.
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Unit tests for the LoggingModule.
    /// All file I/O uses isolated per-test temp directories.
    /// </summary>
    [TestClass]
    public class LoggingModuleTests_Legacy
    {
        #region Private Members

        private string m_tempFolder = null!;

        #endregion

        /// <summary>
        /// Initializes the test context before each test method runs.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            m_tempFolder = Path.Combine(
                System.IO.Path.GetTempPath(),
                "PmsLogTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(m_tempFolder);
        }

        /// <summary>
        /// Cleans up resources after each test method runs.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (Directory.Exists(m_tempFolder))
            {
                Directory.Delete(m_tempFolder, recursive: true);
            }
        }

        // ── AppLoggerSetup: constructor guards ──────────────────────────────────

        /// <summary>
    /// Given LoggingModule Legacy NullFolder When CreateCalled Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_NullFolder_When_CreateCalled_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => AppLoggerSetup.Create(null!));
        }

        /// <summary>
    /// Given LoggingModule Legacy EmptyFolder When CreateCalled Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_EmptyFolder_When_CreateCalled_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => AppLoggerSetup.Create(string.Empty));
        }

        /// <summary>
    /// Given LoggingModule Legacy WhitespaceFolder When CreateCalled Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_WhitespaceFolder_When_CreateCalled_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => AppLoggerSetup.Create("   "));
        }

        // ── AppLoggerSetup: valid creation ──────────────────────────────────────

        /// <summary>
    /// Given LoggingModule Legacy ValidFolder When CreateCalled Then BackendLoggerIsNotNull
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_ValidFolder_When_CreateCalled_Then_BackendLoggerIsNotNull()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            Assert.IsNotNull(backend);
        }

        /// <summary>
    /// Given LoggingModule Legacy ValidFolder When CreateCalled Then FrontendLoggerIsNotNull
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_ValidFolder_When_CreateCalled_Then_FrontendLoggerIsNotNull()
        {
            var frontend = AppLoggerSetup.Create(m_tempFolder);

            Assert.IsNotNull(frontend);
        }

        /// <summary>
    /// Given LoggingModule Legacy ValidFolder When CreateCalled Then LogsSubdirectoryIsCreated
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_ValidFolder_When_CreateCalled_Then_LogsSubdirectoryIsCreated()
        {
            AppLoggerSetup.Create(m_tempFolder);

            Assert.IsTrue(Directory.Exists(Path.Combine(m_tempFolder, "logs")));
        }

        // ── SourceLogger (Backend): each log level writes to file ─────────────────

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogInfoCalled Then EntryIsWrittenToFile
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogInfoCalled_Then_EntryIsWrittenToFile()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogInfo("info message");

            string content = ReadLogFile();
            StringAssert.Contains(content, "info message");
        }

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogDebugCalled Then EntryIsWrittenToFile
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogDebugCalled_Then_EntryIsWrittenToFile()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogDebug("debug message");

            StringAssert.Contains(ReadLogFile(), "debug message");
        }

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogWarnCalled Then EntryIsWrittenToFile
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogWarnCalled_Then_EntryIsWrittenToFile()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogWarn("warn message");

            StringAssert.Contains(ReadLogFile(), "warn message");
        }

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogErrorCalled Then EntryIsWrittenToFile
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogErrorCalled_Then_EntryIsWrittenToFile()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogError("error message");

            StringAssert.Contains(ReadLogFile(), "error message");
        }

        // ── SourceLogger: entry format / tag content ──────────────────────────────

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogInfoCalled Then EntryContainsBackendTag
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogInfoCalled_Then_EntryContainsBackendTag()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogInfo("hello");

            StringAssert.Contains(ReadLogFile(), "hello");
        }

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogInfoCalled Then EntryContainsTimestampPattern
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogInfoCalled_Then_EntryContainsTimestampPattern()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogInfo("stamp check");

            // Timestamp format: [yyyy-MM-dd HH:mm:ss.fff]
            string content = ReadLogFile();
            Assert.IsTrue(
                System.Text.RegularExpressions.Regex.IsMatch(
                    content,
                    @"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\]"),
                "Log entry must contain a formatted timestamp.");
        }

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogInfoCalled Then EntryContainsMessageText
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogInfoCalled_Then_EntryContainsMessageText()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogInfo("unique_message_xyz");

            StringAssert.Contains(ReadLogFile(), "unique_message_xyz");
        }

        // ── LogEntryFormatter: exception detail lines ─────────────────────────────

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogErrorWithExceptionCalled Then ExceptionTypeIsWritten
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogErrorWithExceptionCalled_Then_ExceptionTypeIsWritten()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogError("fail", new InvalidOperationException("boom"));

            StringAssert.Contains(ReadLogFile(), "InvalidOperationException");
        }

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogErrorWithExceptionCalled Then ExceptionMessageIsWritten
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogErrorWithExceptionCalled_Then_ExceptionMessageIsWritten()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogError("fail", new InvalidOperationException("boom message"));

            StringAssert.Contains(ReadLogFile(), "boom message");
        }

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogErrorWithExceptionCalled Then ErrorMessageIsAlsoWritten
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogErrorWithExceptionCalled_Then_ErrorMessageIsAlsoWritten()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogError("outer error text", new Exception("ex"));

            StringAssert.Contains(ReadLogFile(), "outer error text");
        }

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogErrorWithInnerExceptionCalled Then InnerExceptionTypeIsWritten
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogErrorWithInnerExceptionCalled_Then_InnerExceptionTypeIsWritten()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);
            var inner = new ArgumentNullException("param");
            var outer = new InvalidOperationException("outer", inner);

            backend.LogError("nested", outer);

            StringAssert.Contains(ReadLogFile(), "ArgumentNullException");
        }

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogErrorWithInnerExceptionCalled Then InnerExceptionMessageIsWritten
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogErrorWithInnerExceptionCalled_Then_InnerExceptionMessageIsWritten()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);
            var inner = new InvalidOperationException("inner detail");
            var outer = new Exception("outer", inner);

            backend.LogError("nested", outer);

            StringAssert.Contains(ReadLogFile(), "inner detail");
        }

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogErrorWithNullException Then NoExceptionSectionWritten
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogErrorWithNullException_Then_NoExceptionSectionWritten()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogError("clean error");

            string content = ReadLogFile();
            Assert.IsFalse(content.Contains("Exception :"),
                "No exception section should be written when ex is null.");
        }

        // ── FileLogWriter: append behaviour ──────────────────────────────────────

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When MultipleLogInfoCalled Then AllEntriesAppendedToFile
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_MultipleLogInfoCalled_Then_AllEntriesAppendedToFile()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            backend.LogInfo("first entry");
            backend.LogInfo("second entry");
            backend.LogInfo("third entry");

            string content = ReadLogFile();
            StringAssert.Contains(content, "first entry");
            StringAssert.Contains(content, "second entry");
            StringAssert.Contains(content, "third entry");
        }

        /// <summary>
    /// Given LoggingModule Legacy SecondFactory ForSameFolder When LogInfoCalled Then EntryAppendedToExistingFile
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_SecondFactory_ForSameFolder_When_LogInfoCalled_Then_EntryAppendedToExistingFile()
        {
            // First logger writes entry A.
            var first = AppLoggerSetup.Create(m_tempFolder);
            first.LogInfo("entry A");

            // Second factory for the same folder writes entry B — must not truncate A.
            var second = AppLoggerSetup.Create(m_tempFolder);
            second.LogInfo("entry B");

            string content = ReadLogFile();
            StringAssert.Contains(content, "entry A");
            StringAssert.Contains(content, "entry B");
        }

        // ── Shared writer: multiple entries written to the same file ─────────────

        /// <summary>
    /// Given LoggingModule Legacy MultipleLogCalls When BothWrite Then BothEntriesAreInSameFile Legacy
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_MultipleLogCalls_When_BothWrite_Then_BothEntriesAreInSameFile_Legacy()
        {
            var logger = AppLoggerSetup.Create(m_tempFolder);

            logger.LogInfo("first entry");
            logger.LogInfo("second entry");

            string content = ReadLogFile();
            StringAssert.Contains(content, "first entry");
            StringAssert.Contains(content, "second entry");
        }

        /// <summary>
    /// Given LoggingModule Legacy MultipleLogCalls When BothWrite Then OnlyOneLogFileExists Legacy
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_MultipleLogCalls_When_BothWrite_Then_OnlyOneLogFileExists_Legacy()
        {
            var logger = AppLoggerSetup.Create(m_tempFolder);

            logger.LogInfo("a");
            logger.LogInfo("b");

            int logFileCount = Directory
                .GetFiles(Path.Combine(m_tempFolder, "logs"), "*.log")
                .Length;
            Assert.AreEqual(1, logFileCount);
        }

        // ── Thread safety ─────────────────────────────────────────────────────────

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When ConcurrentLogInfoCalled Then NoExceptionThrownAndAllEntriesPresent
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_ConcurrentLogInfoCalled_Then_NoExceptionThrownAndAllEntriesPresent()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);
            const int threadCount = 20;
            var exceptions = new System.Collections.Concurrent.ConcurrentBag<Exception>();

            Parallel.For(0, threadCount, i =>
            {
                try
                {
                    backend.LogInfo($"concurrent entry {i}");
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });

            Assert.AreEqual(0, exceptions.Count, "No exceptions should be thrown during concurrent writes.");

            string content = ReadLogFile();
            for (int i = 0; i < threadCount; i++)
            {
                StringAssert.Contains(content, $"concurrent entry {i}");
            }
        }

        // ── LogEntryFormatter: StackTrace branch ──────────────────────────────────

        /// <summary>
    /// Given LoggingModule Legacy BackendLogger When LogErrorWithThrownExceptionCalled Then StackTraceIsWritten
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_BackendLogger_When_LogErrorWithThrownExceptionCalled_Then_StackTraceIsWritten()
        {
            var backend = AppLoggerSetup.Create(m_tempFolder);

            Exception? caughtEx = null;
            try { throw new InvalidOperationException("has stack trace"); }
            catch (Exception ex) { caughtEx = ex; }

            backend.LogError("error with stack trace", caughtEx);

            StringAssert.Contains(ReadLogFile(), "StackTrace:");
        }

        // ── SourceLogger: null writer guard ──────────────────────────────────────

        /// <summary>
    /// Given LoggingModule Legacy NullWriter When SourceLoggerConstructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_NullWriter_When_SourceLoggerConstructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new SourceLogger(null!));
        }

        // ── SourceLogger: ILogWriter integration via mock ─────────────────────────

        /// <summary>
    /// Given LoggingModule Legacy MockWriter When SourceLoggerLogInfoCalled Then WriteIsInvokedOnce
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_MockWriter_When_SourceLoggerLogInfoCalled_Then_WriteIsInvokedOnce()
        {
            var mockWriter = new Mock<ILogWriter>();
            var logger     = new SourceLogger(mockWriter.Object);

            logger.LogInfo("test message");

            mockWriter.Verify(w => w.Write(It.Is<string>(s => s.Contains("test message"))), Times.Once());
        }

        /// <summary>
    /// Given LoggingModule Legacy MockWriter When SourceLoggerLogErrorWithExceptionCalled Then WrittenTextContainsExceptionType
        /// </summary>
        [TestMethod]
        public void Given_LoggingModule_Legacy_MockWriter_When_SourceLoggerLogErrorWithExceptionCalled_Then_WrittenTextContainsExceptionType()
        {
            string? captured = null;
            var mockWriter   = new Mock<ILogWriter>();
            mockWriter
                .Setup(w => w.Write(It.IsAny<string>()))
                .Callback<string>(text => captured = text);

            var logger = new SourceLogger(mockWriter.Object);
            logger.LogError("oops", new InvalidOperationException("bad state"));

            Assert.IsNotNull(captured);
            StringAssert.Contains(captured, "InvalidOperationException");
            StringAssert.Contains(captured, "bad state");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private string ReadLogFile()
            => File.ReadAllText(Path.Combine(m_tempFolder, "logs", "app.log"));
    }
}

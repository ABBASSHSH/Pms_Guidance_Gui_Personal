#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : SystemLanguageProviderTests.cs
// Description: Unit tests for SystemLanguageProvider. ILogger is mocked
//              so no file I/O occurs.
//              All tests follow the Given/When/Then naming convention.
// Notes:
// Modification History : Code Review, 05-May-2026, Added test cases for SystemLanguageProvider
//--------------------------------------------------------------------
#endregion

using System;
using ConfigurationModule;
using Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pms_GuidanceGUI.Tests.Unit
{
    /// <summary>
    /// Unit tests for <see cref="SystemLanguageProvider"/>.
    /// <see cref="ILogger"/> is mocked so no file I/O occurs.
    /// </summary>
    [TestClass]
    public class SystemLanguageProviderTests
    {
        // ── Constructor guards ────────────────────────────────────────────────────

        /// <summary>
    /// Given SystemLanguageProvider NullLogger When Constructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_SystemLanguageProvider_NullLogger_When_Constructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new SystemLanguageProvider(null!));
        }

        // ── Open ─────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given SystemLanguageProvider ValidLogger When OpenCalled Then LogInfoIsCalledWithLanguageName
        /// </summary>
        [TestMethod]
        public void Given_SystemLanguageProvider_ValidLogger_When_OpenCalled_Then_LogInfoIsCalledWithLanguageName()
        {
            var mockLogger = new Mock<ILogger>();
            var provider   = new SystemLanguageProvider(mockLogger.Object);

            provider.Open();

            mockLogger.Verify(
                x => x.LogInfo(It.Is<string>(s => s.Contains("System UI language")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        /// <summary>
    /// Given SystemLanguageProvider ValidLogger When OpenCalled Then LoggedMessageContainsDetectedLanguage
        /// </summary>
        [TestMethod]
        public void Given_SystemLanguageProvider_ValidLogger_When_OpenCalled_Then_LoggedMessageContainsDetectedLanguage()
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
    /// Given SystemLanguageProvider ValidProvider When CloseCalled Then NoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_SystemLanguageProvider_ValidProvider_When_CloseCalled_Then_NoExceptionThrown()
        {
            var provider = new SystemLanguageProvider(new Mock<ILogger>().Object);
            provider.Open();

            bool threw = false;
            try { provider.Close(); } catch { threw = true; }

            Assert.IsFalse(threw);
        }
    }
}

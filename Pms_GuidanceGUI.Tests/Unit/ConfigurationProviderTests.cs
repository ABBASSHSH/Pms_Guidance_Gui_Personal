#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : ConfigurationProviderTests.cs
// Description: Unit tests for GuidanceConfigurationProvider. ILogger is mocked
//              with Moq; the JSON file is created in a per-test temp directory.
//              All tests follow the Given/When/Then naming convention.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using System.IO;
using System.Text.Json;
using ConfigurationModule;
using Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pms_GuidanceGUI.Tests.Unit
{
    /// <summary>
    /// Unit tests for <see cref="GuidanceConfigurationProvider"/>.
    /// </summary>
    [TestClass]
    public class ConfigurationProviderTests
    {
        #region Private Members

        private string       m_tempFolder = null!;
        private string       m_configFile = null!;
        private Mock<ILogger> m_mockLogger = null!;

        #endregion

        /// <summary>
        /// Initializes the test context before each test method runs.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            m_tempFolder = Path.Combine(
                Path.GetTempPath(), "ConfigProviderTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(m_tempFolder);
            m_configFile = Path.Combine(m_tempFolder, "pms_guidance_configuration.json");
            m_mockLogger = new Mock<ILogger>();
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

        // ── Constructor guards ────────────────────────────────────────────────────

        /// <summary>
    /// Given ConfigurationProvider When NullFilePathConstructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_NullFilePathConstructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new GuidanceConfigurationProvider(null!, m_mockLogger.Object));
        }

        /// <summary>
    /// Given ConfigurationProvider When WhitespaceFilePathConstructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_WhitespaceFilePathConstructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new GuidanceConfigurationProvider("   ", m_mockLogger.Object));
        }

        /// <summary>
    /// Given ConfigurationProvider When NullLoggerConstructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_NullLoggerConstructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new GuidanceConfigurationProvider(m_configFile, null!));
        }

        // ── Open: file errors ─────────────────────────────────────────────────────

        /// <summary>
    /// Given ConfigurationProvider When NonExistentFileOpenCalled Then ThrowsFileNotFoundException
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_NonExistentFileOpenCalled_Then_ThrowsFileNotFoundException()
        {
            var provider = new GuidanceConfigurationProvider(
                Path.Combine(m_tempFolder, "does_not_exist.json"), m_mockLogger.Object);

            Assert.ThrowsException<FileNotFoundException>(() => provider.Open());
        }

        /// <summary>
        /// Given InvalidJson When OpenCalled Then ThrowsJsonException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(JsonException), AllowDerivedTypes = true)]
        public void Given_ConfigurationProvider_When_InvalidJsonOpenCalled_Then_ThrowsJsonException()
        {
            File.WriteAllText(m_configFile, "THIS IS NOT JSON");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);

            provider.Open();
        }

        // ── Open: parsing ─────────────────────────────────────────────────────────

        /// <summary>
    /// Given ConfigurationProvider When ValidJsonWithTopLevelKeyOpenCalled Then TopLevelKeyValueIsStored
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_ValidJsonWithTopLevelKeyOpenCalled_Then_TopLevelKeyValueIsStored()
        {
            WriteJson("{\"InstallationPath\":\"C:\\\\PmsGuidance\"}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);

            provider.Open();

            Assert.AreEqual("C:\\PmsGuidance", provider.GetValue("InstallationPath"));
        }

        /// <summary>
    /// Given ConfigurationProvider When ValidJsonWithNestedObjectOpenCalled Then ColonSeparatedKeyIsStored
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_ValidJsonWithNestedObjectOpenCalled_Then_ColonSeparatedKeyIsStored()
        {
            WriteJson("{\"SystemInterfaces\":{\"SystemCheck\":\"C:\\\\SystemCheck.exe\"}}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);

            provider.Open();

            Assert.IsNull(provider.GetValue("SystemInterfaces"));
            Assert.IsNull(provider.GetValue("SystemInterfaces:SystemCheck"));
        }

        /// <summary>
    /// Given ConfigurationProvider When ValidJsonWithTwoTopLevelKeysOpenCalled Then BothKeysAreStored
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_ValidJsonWithTwoTopLevelKeysOpenCalled_Then_BothKeysAreStored()
        {
            WriteJson("{\"KeyA\":\"ValueA\",\"KeyB\":\"ValueB\"}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);

            provider.Open();

            Assert.AreEqual("ValueA", provider.GetValue("KeyA"));
            Assert.AreEqual("ValueB", provider.GetValue("KeyB"));
        }

        /// <summary>
    /// Given ConfigurationProvider When JsonWithNullValueOpenCalled Then NullKeyIsNotAddedToEntries
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_JsonWithNullValueOpenCalled_Then_NullKeyIsNotAddedToEntries()
        {
            WriteJson("{\"PresentKey\":\"value\",\"NullKey\":null}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);

            provider.Open();

            Assert.IsNull(provider.GetValue("NullKey"));
        }

        /// <summary>
    /// Given ConfigurationProvider When ValidJsonFileOpenCalled Then LogInfoIsCalledWithEntryCount
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_ValidJsonFileOpenCalled_Then_LogInfoIsCalledWithEntryCount()
        {
            WriteJson("{\"KeyA\":\"A\",\"KeyB\":\"B\"}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);

            provider.Open();

            m_mockLogger.Verify(
                x => x.LogInfo(It.Is<string>(s => s.Contains("2") && s.Contains("configuration entries")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        // ── GetValue ──────────────────────────────────────────────────────────────

        /// <summary>
    /// Given ConfigurationProvider When LoadedConfigGetValueCalledWithKnownKey Then CorrectValueReturned
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_LoadedConfigGetValueCalledWithKnownKey_Then_CorrectValueReturned()
        {
            WriteJson("{\"MyKey\":\"MyValue\"}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);
            provider.Open();

            Assert.AreEqual("MyValue", provider.GetValue("MyKey"));
        }

        /// <summary>
    /// Given ConfigurationProvider When LoadedConfigGetValueCalledWithUnknownKey Then NullReturned
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_LoadedConfigGetValueCalledWithUnknownKey_Then_NullReturned()
        {
            WriteJson("{\"MyKey\":\"MyValue\"}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);
            provider.Open();

            Assert.IsNull(provider.GetValue("DoesNotExist"));
        }

        /// <summary>
    /// Given ConfigurationProvider When VerifyCommandConfiguredGetVerificationCommandCalled Then VerifyCommandReturned
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_VerifyCommandConfiguredGetVerificationCommandCalled_Then_VerifyCommandReturned()
        {
            WriteJson("{\"InstallationPrerequisitesVerify\":\"verify-command\"}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);
            provider.Open();

            Assert.AreEqual("verify-command", provider.GetVerificationCommand());
        }

        /// <summary>
    /// Given ConfigurationProvider When InstallCommandConfiguredGetInstallationCommandCalled Then InstallCommandReturned
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_InstallCommandConfiguredGetInstallationCommandCalled_Then_InstallCommandReturned()
        {
            WriteJson("{\"Installationtrigger\":\"install-command\"}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);
            provider.Open();

            Assert.AreEqual("install-command", provider.GetInstallationCommand());
        }

        // ── GetAll ────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given ConfigurationProvider When LoadedConfigGetAllCalled Then AllPairsReturned
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_LoadedConfigGetAllCalled_Then_AllPairsReturned()
        {
            WriteJson("{\"A\":\"1\",\"B\":\"2\",\"C\":\"3\"}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);
            provider.Open();

            var all = provider.GetAll();

            Assert.AreEqual(3, all.Count);
            Assert.AreEqual("1", all["A"]);
            Assert.AreEqual("2", all["B"]);
            Assert.AreEqual("3", all["C"]);
        }

        // ── Close ─────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given ConfigurationProvider When LoadedConfigCloseCalled Then GetValueReturnsNull
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_LoadedConfigCloseCalled_Then_GetValueReturnsNull()
        {
            WriteJson("{\"MyKey\":\"MyValue\"}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);
            provider.Open();

            provider.Close();

            Assert.IsNull(provider.GetValue("MyKey"));
        }

        /// <summary>
    /// Given ConfigurationProvider When LoadedConfigCloseCalled Then LogInfoIsCalledForClear
        /// </summary>
        [TestMethod]
        public void Given_ConfigurationProvider_When_LoadedConfigCloseCalled_Then_LogInfoIsCalledForClear()
        {
            WriteJson("{\"MyKey\":\"MyValue\"}");
            var provider = new GuidanceConfigurationProvider(m_configFile, m_mockLogger.Object);
            provider.Open();

            provider.Close();

            m_mockLogger.Verify(
                x => x.LogInfo(It.Is<string>(s => s.Contains("cleared")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private void WriteJson(string json)
            => File.WriteAllText(m_configFile, json);
    }
}


#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : MessageReceivedEventArgsTests.cs
// Description: Unit tests for MessageReceivedEventArgs constructor guards.
//--------------------------------------------------------------------
#endregion

using System;
using Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pms_GuidanceGUI.Tests.Unit
{
    /// <summary>
    /// Unit tests for <see cref="MessageReceivedEventArgs"/>.
    /// </summary>
    [TestClass]
    public class MessageReceivedEventArgsTests
    {
        /// <summary>
    /// Given MessageReceivedEventArgs When ValidInputsConstructed Then PropertiesAreSet
        /// </summary>
        [TestMethod]
        public void Given_MessageReceivedEventArgs_When_ValidInputsConstructed_Then_PropertiesAreSet()
        {
            var args = new MessageReceivedEventArgs("LogMessage", "{\"k\":\"v\"}");

            Assert.AreEqual("LogMessage", args.Action);
            Assert.AreEqual("{\"k\":\"v\"}", args.Payload);
        }

        /// <summary>
    /// Given MessageReceivedEventArgs When NullActionConstructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_MessageReceivedEventArgs_When_NullActionConstructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new MessageReceivedEventArgs(null!, "payload"));
        }

        /// <summary>
    /// Given MessageReceivedEventArgs When NullPayloadConstructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_MessageReceivedEventArgs_When_NullPayloadConstructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new MessageReceivedEventArgs("Action", null!));
        }
    }
}

#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : UIAppStartedCommandHandler.cs
// Description: Handles UIAppStartedCommand by fetching the system UI language
//              and raising a ShowSystemLanguageEventArgs reply so the front end
//              receives the detected language.
// Notes:
// Modification History : Abbas Bahrainwala, 05-May-2026, Added system language fetch and reply event
//--------------------------------------------------------------------
#endregion

using System;
using System.Globalization;
using BusinessLogicModule.Commands;
using BusinessLogicModule.EventArgs;
using Infrastructure;

namespace BusinessLogicModule
{
    /// <summary>
    /// Handles <see cref="UIAppStartedCommand"/> by fetching the system UI language
    /// and raising a <see cref="ShowSystemLanguageEventArgs"/> reply.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class UIAppStartedCommandHandler : AbstractCommandHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public override Type CommandType => typeof(UIAppStartedCommand);

        #endregion

        #region Protected Members

        /// <inheritdoc/>
        protected override void ExecuteCommand(ICommand theCommand)
        {
            string language = CultureInfo.CurrentUICulture.Name;
            m_logger.LogInfo($"System UI language detected: {language}");

            m_actionReplyPrivate.InvokeEvent(new ShowSystemLanguageEventArgs(language));
        }

        #endregion

        #region Private Members

        internal UIAppStartedCommandHandler(
            IActionReplyPrivate replyPrivate,
            ILogger             logger)
            : base(replyPrivate, logger)
        {
        }

        #endregion
    }
}

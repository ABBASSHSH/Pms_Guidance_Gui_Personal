#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : AbstractCommandHandler.cs
// Description: Provides the common infrastructure shared by all command handlers.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Extracted common code from concrete command handlers
//--------------------------------------------------------------------
#endregion

using System;
using Infrastructure;

namespace BusinessLogicModule
{
    /// <summary>
    /// Base class for all command handlers. Holds the shared
    /// <see cref="IActionReplyPrivate"/> and <see cref="ILogger"/> dependencies,
    /// validates the incoming command in <see cref="HandleCommand"/>, and
    /// delegates to the abstract <see cref="ExecuteCommand"/> method for
    /// handler-specific logic.
    /// </summary>
    internal abstract class AbstractCommandHandler : ICommandHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public abstract Type CommandType { get; }

        /// <inheritdoc/>
        public void HandleCommand(ICommand theCommand)
        {
            if (theCommand == null)
            {
                throw new ArgumentNullException(nameof(theCommand));
            }

            ExecuteCommand(theCommand);
        }

        #endregion

        #region Protected Members

        /// <summary>The action-reply event invoker available to subclasses.</summary>
        protected readonly IActionReplyPrivate m_actionReplyPrivate;

        /// <summary>The logger available to subclasses.</summary>
        protected readonly ILogger m_logger;

        /// <summary>
        /// Initializes the common handler state.
        /// </summary>
        /// <param name="replyPrivate">The action reply event invoker.</param>
        /// <param name="logger">The logger instance.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="replyPrivate"/> or <paramref name="logger"/> is <c>null</c>.
        /// </exception>
        protected AbstractCommandHandler(IActionReplyPrivate replyPrivate, ILogger logger)
        {
            m_actionReplyPrivate = replyPrivate ?? throw new ArgumentNullException(nameof(replyPrivate));
            m_logger             = logger        ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executes the handler-specific logic. Called by <see cref="HandleCommand"/>
        /// after the null guard has passed.
        /// </summary>
        /// <param name="theCommand">The validated, non-null command to process.</param>
        protected abstract void ExecuteCommand(ICommand theCommand);

        #endregion
    }
}

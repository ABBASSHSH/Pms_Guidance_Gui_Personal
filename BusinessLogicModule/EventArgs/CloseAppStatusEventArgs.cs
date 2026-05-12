#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : CloseAppStatusEventArgs.cs
// Description: Event arguments confirming that the application close sequence has been initiated.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

namespace BusinessLogicModule.EventArgs
{
    /// <summary>
    /// Event arguments confirming that the application close sequence has been initiated.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class CloseAppStatusEventArgs : System.EventArgs
    {
        #region Public Members

        /// <summary>
        /// Gets a value indicating whether the close sequence was initiated successfully.
        /// </summary>
        /// <value><c>true</c> if the shutdown was triggered; otherwise, <c>false</c>.</value>
        public bool IsClosing { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseAppStatusEventArgs"/> class.
        /// </summary>
        /// <param name="isClosing">Whether the close sequence was triggered.</param>
        public CloseAppStatusEventArgs(bool isClosing)
        {
            IsClosing = isClosing;
        }

        #endregion
    }
}

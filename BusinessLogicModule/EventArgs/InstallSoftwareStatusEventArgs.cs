#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : InstallSoftwareStatusEventArgs.cs
// Description: Event arguments carrying the result of a software installation attempt.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

namespace BusinessLogicModule.EventArgs
{
    /// <summary>
    /// Event arguments carrying the result of a software installation attempt.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class InstallSoftwareStatusEventArgs : System.EventArgs
    {
        #region Public Members

        /// <summary>
        /// Gets a value indicating whether the installation completed successfully.
        /// </summary>
        /// <value><c>true</c> if installation succeeded; otherwise, <c>false</c>.</value>
        public bool IsInstalled { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallSoftwareStatusEventArgs"/> class.
        /// </summary>
        /// <param name="isInstalled">Whether the installation succeeded.</param>
        public InstallSoftwareStatusEventArgs(bool isInstalled)
        {
            IsInstalled = isInstalled;
        }

        #endregion
    }
}

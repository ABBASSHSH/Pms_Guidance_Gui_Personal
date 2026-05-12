#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : VerifyInstallationPrerequisitesStatusEventArgs.cs
// Description: Event arguments carrying the result of the installation prerequisites check.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

namespace BusinessLogicModule.EventArgs
{
    /// <summary>
    /// Event arguments carrying the result of the installation prerequisites check.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class VerifyInstallationPrerequisitesStatusEventArgs : System.EventArgs
    {
        #region Public Members

        /// <summary>
        /// Gets a value indicating whether all prerequisites are satisfied.
        /// </summary>
        /// <value><c>true</c> if all prerequisites are met; otherwise, <c>false</c>.</value>
        public bool PrerequisitesMet { get; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="VerifyInstallationPrerequisitesStatusEventArgs"/> class.
        /// </summary>
        /// <param name="prerequisitesMet">Whether all prerequisites are satisfied.</param>
        public VerifyInstallationPrerequisitesStatusEventArgs(bool prerequisitesMet)
        {
            PrerequisitesMet = prerequisitesMet;
        }

        #endregion
    }
}

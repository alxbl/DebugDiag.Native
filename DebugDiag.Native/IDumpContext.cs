using System.Security.Cryptography.X509Certificates;
using DebugDiag.DotNet;

namespace DebugDiag.Native
{
    /// <summary>
    /// Represents the context in which a dump is being analyzed.
    /// </summary>
    public interface IDumpContext
    {
        #region Properties
        /// <summary>
        /// The reference to the DebugDiag script management engine.
        /// </summary>
        NetScriptManager Manager { get; }

        /// <summary>
        /// The reference to the underlying debugger.
        /// </summary>
        NetDbgObj Debugger { get; }

        /// <summary>
        /// The reference to the DebugDiag progress tracker.
        /// </summary>
        NetProgress Progress { get; }
        #endregion

        #region API

        /// <summary>
        /// Executes a command in the debugger engine.
        /// </summary>
        /// <param name="cmd">The command for the debugger engine.</param>
        /// <returns>The output from the debugger engine.</returns>
        string Execute(string cmd);

        #endregion
    }
}

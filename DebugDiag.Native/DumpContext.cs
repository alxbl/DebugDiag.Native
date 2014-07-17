using System;
using DebugDiag.DotNet;

namespace DebugDiag.Native
{
    /// <summary>
    /// Represents the context in which a dump is being analyzed.
    /// </summary>
    public class DumpContext : IDumpContext
    {
        private bool _contextValid = false;
        private NetScriptManager _mgr;
        private NetDbgObj _dbg;
        private NetProgress _progress;
        public DumpContext(NetScriptManager mgr, NetDbgObj dbg, NetProgress progress)
        {
            Manager = mgr;
            Debugger = dbg;
            Progress = progress;
            if (Manager != null && Debugger != null && Progress != null)
                _contextValid = true;

        }

        /// <summary>
        /// The reference to the DebugDiag script management engine.
        /// </summary>
        public NetScriptManager Manager {
            get
            {
                CheckContext();
                return _mgr;
            }

            private set { _mgr = value; }
        }

        /// <summary>
        /// The reference to the underlying debugger.
        /// </summary>
        public NetDbgObj Debugger
        {
            get
            {
                CheckContext();
                return _dbg;
            }

            private set { _dbg = value; }
        }

        /// <summary>
        /// The reference to the DebugDiag progress tracker.
        /// </summary>
        public NetProgress Progress
        {
            get
            {
                CheckContext();
                return _progress;
            }

            private set { _progress = value; }
        }

        public string Execute(string cmd)
        {
            return Debugger.Execute(cmd);
        }

        /// <summary>
        /// Verifies that the context is valid before using Native calls.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the context is not properly set.</exception>
        private void CheckContext()
        {
            if (!_contextValid) throw new InvalidOperationException("The Native Context must be set with Initialize() before using DebugDiag.Native");
        }
    }
}

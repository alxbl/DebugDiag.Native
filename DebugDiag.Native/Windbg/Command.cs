using System;
using System.Diagnostics;

namespace DebugDiag.Native.Windbg
{
    /// <summary>
    /// Represents a Windbg command and stores information about its state,
    /// and outcome.
    /// </summary>
    public abstract class Command
    {
        private string _output; /// Output cache.

        
        private string _command; /// Command cache.

        /// <summary>
        /// The raw output of that command.
        /// </summary>
        public string Output
        {
            get
            {
                if (!Executed) Execute();
                return _output;
            }
            protected set { _output = value; }
        }

        public bool Executed { get; private set; }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <exception cref="CommandException">Thrown if something went wrong while executing this command.</exception>
        public void Execute()
        {
            _command = BuildCommand();
            Output = Native.Context.Execute(_command); // Should we really use a static context for commands?

            try
            {

                Parse(_output);
            }
            catch (Exception ex)
            {
                throw new CommandException(string.Format("Error while executing command. See inner exception.\r\n<br><br>{0}", ex), ex);
            }
            Executed = true;
        }

        /// <summary>
        /// Builds the command to be executed against the debugging engine.
        /// </summary>
        /// <returns>The command to be executed.</returns>
        protected abstract string BuildCommand();

        /// <summary>
        /// Parses the command's output and populates the command object as needed.
        /// 
        /// Commands must implement this method. Any exception that is thrown during parsing
        /// will result in the command failing.
        /// </summary>
        /// <param name="output">The output returned by the debugging engine.</param>
        protected abstract void Parse(string output);
    }
}
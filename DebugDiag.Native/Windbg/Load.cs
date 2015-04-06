namespace DebugDiag.Native.Windbg
{
    public class Load : Command
    {
        private readonly string _ext;
        private bool _success;

        public Load(string extension)
        {
            _ext = extension;
        }
        protected override string BuildCommand()
        {
            Success = false;
            return string.Format(".load {0}", _ext);
        }

        protected override void Parse(string output)
        {

            if (!string.IsNullOrWhiteSpace(output)) // windbg doesn't return anything when the load is successful.
                throw new CommandException(string.Format("Could not load windbg extension: ({0}):\r\n{1}", _ext, output));
            Success = true;
        }

        public bool Success
        {
            get
            {
                if (!Executed) Execute();
                return _success;
            }
            private set { _success = value; }
        }
    }
}

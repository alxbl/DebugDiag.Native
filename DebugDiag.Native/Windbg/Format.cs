using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Windbg
{
    /// <summary>
    /// Allows to dump arbitrary memory according to a format.
    /// </summary>
    public class Format<TType> : Command where TType : struct
    {
        private readonly string _format;
        private readonly ulong _addr;
        private TType _value;

        public TType Value
        {
            get
            {
                if (!Executed) Execute();
                return _value;
            }
        }

        public Format(string type, ulong address)
        {
            _format = type;
            _addr = address;
        }

        protected override string BuildCommand()
        {
            // > ?? *((int*)0xbaadf00d)
            // int 0n42
            return string.Format("?? *(({0}*)0x{1:x})", _format, _addr);
        }

        protected override void Parse(string output)
        {
            try
            {
                // Any failure here will result in the command failing.
                // ReSharper disable once PossibleNullReferenceException
                _value = (TType) TypeDescriptor.GetConverter(typeof (TType)).ConvertFromString(output.Split(' ')[1]);
            }
            catch (Exception ex)
            {
                throw new CommandException(string.Format("The command failed `{0}` See inner exception for details.", BuildCommand()), ex);
            }
        }
    }
}

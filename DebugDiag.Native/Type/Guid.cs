using System;
using System.Text.RegularExpressions;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a GUID object inside the dump file.
    /// 
    /// The GUID is treated as a primitive object, and as such acts as a leaf node in the type hierarchy.
    /// </summary>
    public class Guid : Primitive
    {
        public static readonly Regex Syntax = new Regex(@"^(ntdll!)?_GUID$"); // ntdll!_GUID.
        
        private System.Guid _value;
        private static readonly Regex InstanceSyntax = new Regex(@"\{([a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12})\}");

        #region Type Implementation

        public override string ToString()
        {
            return string.Format("{{{0}}}", _value);
        }

        public override NativeType GetField(string name)
        {
            throw new InvalidOperationException("This is a primitive type.");
        }

        public override NativeType GetField(ulong offset)
        {
            throw new InvalidOperationException("This is a primitive type.");
        }

        protected override void Parse(string detail)
        {
            try
            {
                var m = InstanceSyntax.Match(detail);
                _value = new System.Guid(m.Groups[1].Value);
            }
            catch // Any failure here means we couldn't parse the GUID.
            {
                _value = System.Guid.Empty;
            }
        }

        protected override void Rebase()
        {
            // In the case of a GUID, we need to grab the GUID representation.
            var dt = new DumpType("ntdll!_GUID", Address);
            Parse(dt.Output);
        }

        #endregion
        #region Constructor

        protected override NativeInstance DeepCopy()
        {
            return new Guid(this);
        }

        public Guid(string typename)
            : base(typename)
        {
        }

        public Guid(Guid other)
            : base(other)
        {
            _value = other._value;
        }
        
        #endregion
    }
}

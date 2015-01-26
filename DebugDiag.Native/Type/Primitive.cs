using System;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a primitive value type.
    /// </summary>
    public abstract class Primitive : NativeType
    {
        #region Constructor
        protected Primitive(string typename) :
            base(typename)
        {
        }

        internal Primitive(NativeType other)
            : base(other)
        {
        }
        #endregion
        #region Internal API

        internal static NativeType CreatePrimitive(string typename, DumpType.Line? dt, bool isInstance)
        {
            Primitive type;
            if (Pointer.Syntax.IsMatch(typename)) // A pointer or a string
            {
                // TODO: Allow Vtable inspection.
                type = new Pointer(typename); // Should be unqualified name

                // Special case: pointer to string. This could be improved to not cause parsing of the pointer tree.
                if (String.Syntax.IsMatch(type.TypeName))
                    type = new String(type.TypeName);
            }
            else if (Guid.Syntax.IsMatch(typename)) // a GUID
                type = new Guid(typename);
            else if (String.Syntax.IsMatch(typename)) // A string (STL, char [] or wchar [])
            {
                type = new String(typename);
            }
            else // Any numerical type.
            {
                type = new Integer(typename);
            }

            if (dt.HasValue && isInstance) type.Parse(dt.Value.Detail);
            return type;
        }

        /// <summary>
        /// Extract the value of this primitive field. Must be implemented by primitive types.
        /// </summary>
        /// <param name="detail">The field detail as provided by `dt`.</param>
        protected abstract void Parse(string detail);

        #endregion
        #region Default Behavior
        
        // By default, every Getter function returns an invalid operation exception. This means derived types need only implement
        // the getters they wish to support.

        public override ulong GetIntValue()
        {
            throw new InvalidOperationException("This is not an integer.");
        }

        public override string GetStringValue()
        {
            throw new InvalidOperationException("This is not a string.");
        }

        #endregion
    }
}

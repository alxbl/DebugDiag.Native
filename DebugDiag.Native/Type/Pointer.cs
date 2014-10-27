using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Type
{
    public class Pointer : NativeType
    {
        public NativeType PointedType { get; private set; }

        public Pointer(string typename)
            : base(typename)
        {
            // Parse the pointed type. It is always of the form `<Typename> *`.
            PointedType = TypeParser.Parse(typename.Substring(0, typename.Length - 2));
        }

        
        protected override void Rebase()
        {
            // Don't need to rebase a pointer.
        }

        protected override void BuildOffsetTable(string type)
        {
            // Do nothing. Pointers do not need an offset table.
        }


        #region Copy
        protected override NativeInstance DeepCopy()
        {
            return new Pointer(this);
        }

        protected Pointer(Pointer other)
            : base(other)
        {
            PointedType = other.PointedType;
        }
        #endregion
    }
}

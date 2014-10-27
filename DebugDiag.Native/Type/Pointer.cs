using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a pointer to another type instance.
    /// 
    /// Calling investigation methods on the pointer will automatically forward the methods
    /// to the pointed-to instance, thereby implicitly dereferencing the type.
    /// </summary>
    public class Pointer : NativeType
    {
        private readonly ulong _pointsTo; // The address to use when rebasing this pointer.

        /// <summary>
        /// Type information of the type this pointer this points to. This is not an instance.
        /// </summary>
        public NativeType PointedType { get; private set; }

        /// <summary>
        /// The instance this pointer points to.
        /// </summary>
        public NativeType Dereference { get; private set; }

        public Pointer(string typename, ulong pointsTo)
            : this(typename)
        {
            _pointsTo = pointsTo;
        }
        public Pointer(string typename)
            : base(typename)
        {
            // Parse the pointed type. It is always of the form `<Typename> *`.
            PointedType = TypeParser.Parse(typename.Substring(0, typename.Length - 2));
        }

        public override NativeType GetField(string name)
        {
            return Dereference.GetField(name);
        }

        public override NativeType GetField(ulong offset)
        {
            return Dereference.GetField(offset);
        }
        protected override void Rebase()
        {
            Dereference = PointedType.RebaseAt(_pointsTo);
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

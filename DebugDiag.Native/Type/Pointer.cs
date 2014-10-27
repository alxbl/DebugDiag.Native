using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugDiag.Native.Windbg;

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
        private ulong _pointsTo; // The address to use when re-basing this pointer.

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
            : base(StandardizePointerType(typename))
        {
            // Use the standardized pointer type.
            PointedType = TypeParser.Parse(TypeName.Substring(0, TypeName.Length - 1).TrimEnd());
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
            // We don't call base.Rebase() because a pointer is a (special) primitive.
            var dp = new Dp(Address, 1);
            _pointsTo = dp.BytesAt(0);

            // When accessing a pointer, to an object instance, we have to preload the object.
            if (!(PointedType is Primitive) && !(PointedType is Pointer))
                PointedType = Preload(PointedType.QualifiedName);

            Dereference = PointedType.RebaseAt(_pointsTo);
        }

        protected override void BuildOffsetTable(string type)
        {
            // Do nothing. Pointers do not need an offset table.
        }

        /// <summary>
        /// Converts a Ptr32 Ptr32 [...] Type into a Type **[...].
        /// 
        /// Vtables become `vftable' *.
        /// </summary>
        /// <param name="typename">The original pointer type.</param>
        /// <returns></returns>
        private static string StandardizePointerType(string typename)
        {
            string std = typename;

            // Special case for virtual tables.
            if (std.Equals("Ptr32") || std.Equals("Ptr64")) return "`vftable' *";

            // For each `Ptr` occurence, append a `*` to the type and strip the occurence.
            while (std.StartsWith("Ptr32 ") || std.StartsWith("Ptr64 "))
            {
                std = std.Substring(6);
                std += (std.EndsWith(" *")) ? "*" : " *";
            }
            return std;
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

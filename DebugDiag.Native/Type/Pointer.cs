using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a pointer to another type instance.
    /// 
    /// Calling investigation methods on the pointer will automatically forward the methods
    /// to the pointed-to instance, thereby implicitly dereferencing the type.
    /// </summary>
    public sealed class Pointer : NativeType
    {
        public ulong PointsTo { get; private set; } // The address to use when re-basing this pointer.

        /// <summary>
        /// Type information of the type this pointer this points to. This is not an instance.
        /// </summary>
        public NativeType PointedType { get; private set; }

        /// <summary>
        /// The instance this pointer points to.
        /// </summary>
        public NativeType Dereference { get; private set; }

        #region Type Implementation

        public override NativeType GetField(string name)
        {
            // If this pointer was created directly from a Rebase(), we need to inspect the pointed type.
            LazyDereference();
            return Dereference.GetField(name);
        }

        public override NativeType GetField(ulong offset)
        {
            return Dereference.GetField(offset);
        }

        public override ulong GetIntValue()
        {
            LazyDereference();
            return Dereference != null ? Dereference.GetIntValue() : 0;
        }

        protected override void Rebase()
        {
            // We don't call base.Rebase() because a pointer is a (special) primitive.

            var dp = new Dp(Address, 1);
            PointsTo = dp.BytesAt(0);

            // When accessing a pointer, to an object instance, we have to preload the object.
            if (!(PointedType is Primitive) && !(PointedType is Pointer))
                PointedType = Preload(PointedType.QualifiedName);

            Dereference = PointedType.RebaseAt(PointsTo);
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

        /// <summary>
        /// Will inspect the memory pointer to by the pointer if it hasn't yet been dereferenced.
        /// </summary>
        private void LazyDereference()
        {
            if (Dereference == null) Dereference = AtAddress(PointsTo, PointedType.QualifiedName);
        }

        #endregion
        #region Constructor

        protected override NativeInstance DeepCopy()
        {
            return new Pointer(this);
        }

        private Pointer(Pointer other)
            : base(other)
        {
            PointedType = other.PointedType;
        }

        public Pointer(string typename)
            : base(StandardizePointerType(typename))
        {
            // Use the standardized pointer type.
            PointedType = Parser.Parse(TypeName.Substring(0, TypeName.Length - 1).TrimEnd());
        }

        public Pointer(string typename, ulong pointsTo)
            : this(typename)
        {
            PointsTo = pointsTo;
        }

        #endregion
    }
}

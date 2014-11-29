namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a type's field at a specific offset. Internal structure used to navigate instances.
    /// </summary>
    internal class Offset
    {
        /// <summary>
        /// Absolute address of this instance.
        /// </summary>
        public ulong Address { get; internal set; }

        /// <summary>
        /// Offset bytes from the base address.
        /// </summary>
        public ulong Bytes { get; internal set; }

        /// <summary>
        /// The name (fully qualified if possible) of the type at this offset.
        /// </summary>
        public string TypeName { get; internal set; }

        /// <summary>
        /// The sub-instance, if it has been inspected.
        /// </summary>
        public NativeType Instance { get; internal set; }

        /// <summary>
        /// The raw memory value at that offset (for primitive types)
        /// </summary>
        public ulong? RawMemory { get; internal set; }

        /// <summary>
        /// Whether this "offset" represents a static type.
        /// 
        /// When this is true, RawMemory is null and Bytes is equal to Address 
        /// since the offset really points to an arbitrary memory location.
        /// </summary>
        public bool IsStatic { get; internal set; }

        /// <summary>
        /// Whether this offset deals with a primitive type.
        /// </summary>
        public bool IsPrimitive { get; internal set; }

        public Offset DeepCopy()
        {
            return new Offset
                   {
                       Bytes = Bytes,
                       TypeName = TypeName,
                       Instance = null, // Don't copy the instance data over.
                       IsStatic = IsStatic,
                       IsPrimitive = IsPrimitive,
                   };
        }
    }
}
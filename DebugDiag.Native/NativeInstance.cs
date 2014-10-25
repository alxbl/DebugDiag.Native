using System;
using System.Dynamic;

namespace DebugDiag.Native
{
    /// <summary>
    /// Provides the interface for native type instances.
    /// 
    /// This exposes customizable behavior based on the subtype.
    /// </summary>
    public abstract class NativeInstance : DynamicObject
    {
        /// <summary>
        /// The base address of this object instance.
        /// 
        /// If the object does not represent an instance, this is 0UL.
        /// </summary>
        public ulong Address { get; internal set; }

        /// <summary>
        /// Returns an instance to an object's field.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>A NativeType instance representing the field.</returns>
        public abstract NativeType GetField(string name);

        /// <summary>
        /// Returns an instance to an object's field.
        /// </summary>
        /// <param name="offset">The field name.</param>
        /// <returns>A NativeType instance representing the field.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset does not exist for this type.</exception>
        public abstract NativeType GetField(ulong offset);

        /// <summary>
        /// Converts a primitive instance into its integer value.
        /// </summary>
        /// <returns>The raw memory at this instance's base address as a 64 bit integer.</returns>
        public abstract ulong GetIntValue();

        /// <summary>
        /// Shortcut method for dumping out the integer value of a field.
        /// </summary>
        /// <param name="field">The name of the field in the current type.</param>
        /// <returns>The raw memory at this instance's base address as a 64 bit integer.</returns>
        public abstract ulong GetIntValue(string field);

        /// <summary>
        /// Shortcut method for dumping out the integer value of a field.
        /// </summary>
        /// <param name="offset">The offset of the field in the current type.</param>
        /// <returns>The raw memory at this instance's base address as a 64 bit integer.</returns>
        public abstract ulong GetIntValue(ulong offset);

        /// <summary>
        /// Converts a primitive null-terminated C-style string NativeType into the string literal based at that location.
        /// </summary>
        /// <returns>The string based at this object's given location</returns>
        public abstract string GetStringValue();

        public abstract string GetUnicodeStringValue();
    }
}
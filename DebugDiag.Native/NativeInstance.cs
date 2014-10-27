using System;
using System.Dynamic;
using DebugDiag.Native.Type;

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
        /// Whether this NativeType object represents an object instance.
        /// </summary>
        public bool IsInstance { get; internal set; }

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

        /// <summary>
        /// Adjusts each field in the type to reflect the instance context.
        /// 
        /// This method must be overridden by user defined types and serves as a hook
        /// to do any additional parsing needed to instantiate the type when it is parsed.
        /// The method will be called upon inspecting the type, and should populate specific fields.
        /// 
        /// This method is called by RebaseAt and the `Address` field is guaranteed to be valid by the
        /// caller.
        /// <see cref="Vector.Rebase"/>
        /// <see cref="Map.Rebase"/>
        /// <see cref="NativeType.Rebase"/>
        /// </summary>
        protected abstract void Rebase();

        /// <summary>
        /// Creates a deep copy of this type.
        /// </summary>
        /// <returns>A copy of this type.</returns>
        protected abstract NativeInstance DeepCopy();

        /// <summary>
        /// Hook into building the offset table for a native type. This allows user types to bypass the offset table.
        /// </summary>
        /// <param name="type"></param>
        protected abstract void BuildOffsetTable(string type);
    }
}
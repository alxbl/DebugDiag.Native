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

        #region Casting

        protected abstract int ToInt32();
        protected abstract uint ToUInt32();
        protected abstract long ToInt64();
        protected abstract ulong ToUInt64();
        protected abstract float ToFloat();
        protected abstract double ToDouble();
        protected abstract bool ToBool();

        #endregion
    }
}
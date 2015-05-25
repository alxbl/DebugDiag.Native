using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents an integer primitive.
    /// 
    /// Integer primitives can be any primitive represented as a number. int, uint, long, bool, float, double, char, wchar, ...
    /// </summary>
    public sealed class Integer : Primitive
    {
        private ulong? _value; // 64 bits should fit every native type.

        #region Type Implementation

        protected override void BuildOffsetTable(string type)
        {
            // Native integer types do not have an offset table.
            // Do nothing here so that NativeType.Preload() does not fail.
        }

        public override NativeType GetField(string field)
        {
            throw new InvalidOperationException("Cannot call GetField() on a primitive type.");
        }

        public override NativeType GetField(ulong offset)
        {
            throw new InvalidOperationException("Cannot call GetField() on a primitive type.");
        }

        protected override void Rebase()
        {
            // Don't do anything when rebasing an integer type.
        }

        protected override void Parse(string detail)
        {
            try
            {
                // Handle long integer
                _value = Native.StringAddrToUlong(TypeName.Equals("_LARGE_INTEGER") ? detail.Split(' ')[1] : detail.Split(' ')[0]);
            }
            catch (ArgumentException)
            {
                _value = null;
            }
        }
        #endregion

        #region Casts

        // TODO: Is it a good idea to have different behaviors when the value was read directly vs. read implicitly?
        // Behavior on implicit read: The real type is known in the dump, so it's possible to cast only those bytes.
        // Behavior on explicit read: The caller is asking to interpret an address as a type. Casting will change the number of bytes read.

        protected override uint ToUInt32()
        {
            if (_value.HasValue) return (uint)_value.Value;
            var val = new Format<uint>("unsigned int", Address);
            return val.Value;
        }

        protected override long ToInt64()
        {
            if (_value.HasValue) return (long)_value.Value;
            var val = new Format<long>("long", Address);
            return val.Value;
        }

        protected override int ToInt32()
        {
            if (_value.HasValue) return (int)_value.Value;
            var val = new Format<int>("int", Address);
            return val.Value;
        }

        protected override bool ToBool()
        {
            if (_value.HasValue) return _value.Value != 0;
            var val = new Format<bool>("bool", Address);
            return val.Value;
        }

        protected override ulong ToUInt64()
        {
            if (_value.HasValue) return _value.Value;
            var val = new Format<ulong>("unsigned long", Address);
            return val.Value;
        }

        protected override double ToDouble()
        {
            if (_value.HasValue) return _value.Value;
            var val = new Format<double>("double", Address);
            return val.Value;
        }
        
        protected override float ToFloat()
        {
            if (_value.HasValue) return _value.Value;
            var val = new Format<float>("float", Address);
            return val.Value;
        }

        #endregion
        #region Constructor
        protected override NativeInstance DeepCopy()
        {
            return new Integer(this);
        }

        public Integer(string typename) : base(typename)
        {
            _value = null; // Default value is invalid.
        }

        public Integer(Integer other) : base(other)
        {
            _value = other._value;
        }
        #endregion
    }
}

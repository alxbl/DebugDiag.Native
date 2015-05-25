using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Integer(string typename) : base(typename)
        {
            _value = null; // Default value is invalid.
        }

        public Integer(Integer other) : base(other)
        {
            _value = other._value;
        }

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

        protected override uint ToUInt32()
        {
            return _value.HasValue ? (uint)_value : uint.MaxValue;
        }

        protected override long ToInt64()
        {
            return _value.HasValue ? (long)_value : long.MaxValue;
        }

        protected override int ToInt32()
        {
            return _value.HasValue ? (int)_value : int.MaxValue;
        }

        protected override bool ToBool()
        {
            return _value.HasValue && _value != 0;
        }

        protected override ulong ToUInt64()
        {
            return _value ?? ulong.MaxValue;
        }

        protected override double ToDouble()
        {
            return _value.HasValue ? (double) _value : double.NaN;
        }
        
        protected override float ToFloat()
        {
            return _value.HasValue ? (float)_value : float.NaN;
        }

        #endregion
    }
}

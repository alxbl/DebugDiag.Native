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
        private ulong _value;

        public Integer(string typename) : base(typename)
        {
            _value = ulong.MaxValue; // Default value is invalid.
        }

        public Integer(Integer other) : base(other)
        {
            _value = other._value;
        }

        #region Type Implementation
        public override NativeType GetField(string field)
        {
            throw new InvalidOperationException("Cannot call GetField() on a primitive type.");
        }

        public override NativeType GetField(ulong offset)
        {
            throw new InvalidOperationException("Cannot call GetField() on a primitive type.");
        }
        
        public override ulong GetIntValue()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString("D");
        }

        protected override void Parse(string detail)
        {
            try
            {
                // Handle long integer...
                _value = Native.StringAddrToUlong(TypeName.Equals("_LARGE_INTEGER") ? detail.Split(' ')[1] : detail.Split(' ')[0]);
            }
            catch (ArgumentException)
            {
                _value = ulong.MaxValue;
            }
        }
        #endregion
    }
}

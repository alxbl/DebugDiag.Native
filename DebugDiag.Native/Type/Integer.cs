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
            return (string) this;
        }

        protected override void Parse(string detail)
        {
            try
            {
                _value = Native.StringAddrToUlong(detail.Split(' ')[0]);
            }
            catch (ArgumentException)
            {
                _value = ulong.MaxValue;
            }
        }
        #endregion

        #region Casts
        public static explicit operator int(Integer i)
        {
            return (int)i._value;
        }

        public static explicit operator uint(Integer i)
        {
            return (uint)i._value;
        }

        public static explicit operator float(Integer i)
        {
            return i._value;
        }

        public static explicit operator double(Integer i)
        {
            return  i._value;
        }

        public static explicit operator bool(Integer i)
        {
            return i._value != 0;
        }

        public static explicit operator char(Integer i)
        {
            return (char)i._value;
        }

        public static explicit operator string(Integer i)
        {
            return i._value.ToString(CultureInfo.InvariantCulture);
        }

        public static explicit operator long(Integer i)
        {
            return (long)i._value;
        }

        /// <summary>
        /// Implicitly use Integer as a ulong.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static implicit operator ulong(Integer i)
        {
            return i._value;
        }
        #endregion
    }
}

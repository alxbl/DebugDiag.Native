using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a native type that supports enumeration.
    /// </summary>
    public abstract class Enumerable : UserType, IEnumerable<NativeType>
    {
        #region Public API
        /// <summary>
        /// The number of objects in this enumerable type.
        /// </summary>
        public ulong Size { get; internal set; }

        /// <summary>
        /// The type of object contained in this enumerable type.
        /// </summary>
        public NativeType ElementType { get; internal set; }
        #endregion

        #region Enumerable Interface
        public abstract IEnumerator<NativeType> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Constructor
        protected Enumerable(string typename) : base(typename) { }

        protected Enumerable(Enumerable other)
            : base(other)
        {
            Size = other.Size;
            ElementType = other.ElementType;
        }
        #endregion
    }
}

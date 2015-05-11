using System.Collections;
using System.Collections.Generic;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a native type that supports enumeration.
    /// </summary>
    public abstract class Enumerable : UserType, IEnumerable<NativeType>
    {
        // Keep a cached copy of the instances to avoid constantly querying the dump file.
        private List<NativeType> _elements;

        #region Public API

        /// <summary>
        /// The number of objects in this enumerable type.
        /// </summary>
        public ulong Size { get; internal set; }

        /// <summary>
        /// The type of object contained in this enumerable type.
        /// </summary>
        public NativeType ValueType { get; internal set; }

        #endregion
        #region Enumerable Interface

        public abstract IEnumerable<NativeType> EnumerateInternal();

        public IEnumerator<NativeType> GetEnumerator()
        {
            if (_elements != null) 
                foreach (var e in _elements) yield return e;
            else
            {
                _elements = new List<NativeType>();
                foreach (var e in EnumerateInternal())
                {
                    _elements.Add(e);
                    yield return e;
                }
            }
        }
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
            ValueType = other.ValueType;
        }

        #endregion
    }
}

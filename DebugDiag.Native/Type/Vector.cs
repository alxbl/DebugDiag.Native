using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    public class Vector : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::vector<(.*),std::allocator<.*> >$");
        // Keep a cached copy of the instances to avoid constantly querying the dump file.
        private List<NativeType> _elements = new List<NativeType>();
        private bool _built;
        private ulong _first, _last, _end, _elementSize;

        public ulong Capacity { get; private set; }

        #region Type Implementation
        internal override void OnCreateInstance(string typename, Match match)
        {
            Debug.Assert(match.Groups.Count == 2, "Vector expects only one group");

            // Recursively parse the type of elements inside the vector.
            ElementType = TypeParser.Parse(match.Groups[1].Value);
        }

        public override IEnumerator<NativeType> GetEnumerator()
        {
            if (_built) // Use the cached elements if possible.
            {
                foreach (var e in _elements) yield return e;
                yield break;
            }

            // Build the list of elements.
            ulong idx = 0;
            while (idx < Size)
            {
                var e = ElementType.RebaseAt(_first + idx*_elementSize);
                _elements.Add(e);
                idx++;
                yield return e;
            }
            _built = true; // Cache the iterated list for future iterations.
        }

        protected override void Rebase()
        {
            base.Rebase(); // Let NativeType identify the vector's members.
            _first = GetIntValue("_Myfirst");
            _last = GetIntValue("_Mylast");
            _end = GetIntValue("_Myend");

            var size = new SizeOf(ElementType.TypeName);
            _elementSize = size.Size; // Implicit  size.Execute();

            Size = _elementSize > 0 ? (_last - _first)/_elementSize : 0;
            Capacity = _elementSize > 0 ? (_end - _first)/_elementSize : 0;
        }

        #endregion

        #region Constructor
        protected override NativeInstance DeepCopy()
        {
            return new Vector(this);
        }

        protected Vector(Vector other)
            : base(other)
        {
            _elements = other._elements;
        }

        public Vector(string typename)
            : base(typename)
        {

        }
        #endregion
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    public sealed class Vector : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::vector<(.*),std::allocator<.*> >$");
        private ulong _first, _last, _end, _elementSize;

        public ulong Capacity { get; private set; }

        #region Type Implementation

        public override void OnCreateInstance(string typename, Match match)
        {
            Debug.Assert(match.Groups.Count == 2, "Vector expects only one group");

            // Recursively parse the type of elements inside the vector.
            ValueType = Parser.Parse(match.Groups[1].Value);
        }

        public override IEnumerable<NativeType> EnumerateInternal()
        {
            if (Size == 0) yield break;

            // Build the list of elements.
            ulong idx = 0;
            while (idx < Size)
            {
                var e = ValueType.RebaseAt(_first + idx * _elementSize);
                idx++;
                yield return e;
            }
        }

        protected override void Rebase()
        {
            base.Rebase(); // Let NativeType identify the vector's members.

            var first = GetField("_Myfirst") as Pointer;
            Debug.Assert(first != null, "Vector cannot have null _Myfirst");

            var last = GetField("_Mylast") as Pointer;
            Debug.Assert(last != null, "Vector cannot have null _Mylast");

            var end = GetField("_Myend") as Pointer;
            Debug.Assert(end != null, "Vector cannot have null _Myend");

            _first = first.PointsTo;
            _last = last.PointsTo;
            _end = end.PointsTo;

            var size = new SizeOf(ValueType.TypeName);
            _elementSize = size.Size; // Implicit  size.Execute();

            Size = _elementSize > 0 ? (_last - _first) / _elementSize : 0;
            Capacity = _elementSize > 0 ? (_end - _first) / _elementSize : 0;
        }

        #endregion
        #region Constructor

        protected override NativeInstance DeepCopy()
        {
            return new Vector(this);
        }

        private Vector(Vector other)
            : base(other)
        {
        }

        public Vector(string typename)
            : base(typename)
        {

        }

        #endregion
    }
}

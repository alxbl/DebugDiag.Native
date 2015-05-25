using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    public sealed class Set : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::set<(.*),.*,std::allocator<.*> >$");

        #region Type Implementation

        protected override void Rebase()
        {
            base.Rebase();
            Size = GetField("_Mysize");
        }

        public override IEnumerable<NativeType> EnumerateInternal()
        {
            if (Size == 0) yield break;

            var foreachStl = new ForeachStl(ForeachStl.Type.Set, Address);

            foreach (var e in foreachStl.GetElements())
                yield return AtAddress(e, ValueType.TypeName);
        }

        #endregion
        #region Constructor

        protected override NativeInstance DeepCopy()
        {
            return new Set(this);
        }

        public Set(string typename)
            : base(typename)
        {
        }

        public Set(Set other)
            : base(other)
        {
        }

        public override void OnCreateInstance(string typename, Match match)
        {
            Debug.Assert(match.Groups.Count == 2, "Set expects 1 group.");
            ValueType = Parser.Parse(match.Groups[1].Value);
        }

        #endregion
    }
}

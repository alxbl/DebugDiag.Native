using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    public sealed class List : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::list<(.*),std::allocator<.*> >$");

        #region Constructor

        protected override NativeInstance DeepCopy()
        {
            return new List(this);
        }

        private List(List other)
            : base(other)
        {
        }

        public List(string typename)
            : base(typename)
        {

        }

        #endregion
        #region Type Implementation

        public override void OnCreateInstance(string typename, Match match)
        {
            Debug.Assert(match.Groups.Count == 2, "List expects only one group");

            // Recursively parse the type of elements inside the list.
            ValueType = Parser.Parse(match.Groups[1].Value);
        }

        protected override void Rebase()
        {
            base.Rebase(); // Let NativeType identify the list's members.
            Size = GetIntValue("_Mysize");
        }

        public override IEnumerable<NativeType> EnumerateInternal()
        {
            if (Size == 0) yield break;

            var foreachStl = new ForeachStl(ForeachStl.Type.List, Address);
            foreach (var e in foreachStl.GetElements())
            {
                yield return AtAddress(e, ValueType.TypeName);
            }
        }

        #endregion
    }
}

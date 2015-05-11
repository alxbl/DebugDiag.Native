using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DebugDiag.Native.Type
{
    public sealed class List : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::list<(.*),std::allocator<.*> >$");
        private NativeType _head;

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
            _head = GetField("_Myhead");
        }

        public override IEnumerable<NativeType> EnumerateInternal()
        {
            if (Size == 0) yield break;

            dynamic cur = _head.GetField("_Next");
            ulong idx = 0;
            
            // Traverse the linked list.
            while (idx < Size)
            {
                idx++;
                NativeType e = cur._Myval;
                cur = cur._Next;
                yield return e;
            }
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DebugDiag.Native.Type
{
    public sealed class Set : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::set<(.*),.*,std::allocator<.*> >$");
        private Pointer _head;

        #region Type Implementation

        protected override void Rebase()
        {
            base.Rebase();
            Size = GetIntValue("_Mysize");
            _head = GetField("_Myhead") as Pointer;
            Debug.Assert(_head != null, "Set cannot have a null head node.");
        }

        public override IEnumerable<NativeType> EnumerateInternal()
        {
            if (Size == 0) yield break;
            dynamic root = _head.GetField("_Parent");
            yield return EnumerateSubtree(root);
        }

        /// <summary>
        /// In-order traversal of the tree.
        /// </summary>
        /// <param name="node">One node in the tree.</param>
        private IEnumerable<NativeType> EnumerateSubtree(dynamic node)
        {
            if (node.PointsTo == _head.PointsTo) yield break; // base case: leaf node
            yield return EnumerateSubtree(node._Left);
            
            var e = node._Myval;
            yield return e;
            
            yield return EnumerateSubtree(node._Right);
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

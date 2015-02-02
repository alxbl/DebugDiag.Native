using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DebugDiag.Native.Type
{
    public sealed class Set : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::set<(.*),.*,std::allocator<.*> >$");
        private readonly List<NativeType> _elements = new List<NativeType>();

        private bool _built;
        private Pointer _head;

        #region Type Implementation

        protected override void Rebase()
        {
            base.Rebase();
            Size = GetIntValue("_Mysize");
            _head = GetField("_Myhead") as Pointer;
            Debug.Assert(_head != null, "Set cannot have a null head node.");
        }

        public override IEnumerator<NativeType> GetEnumerator()
        {
            if (Size == 0) yield break;
            if (!_built)
            {
                dynamic root = _head.GetField("_Parent");
                EnumerateSubtree(root);
                _built = true;
            }
            foreach (var e in _elements) yield return e;
        }

        /// <summary>
        /// In-order traversal of the tree.
        /// </summary>
        /// <param name="node">One node in the tree.</param>
        private void EnumerateSubtree(dynamic node)
        {
            if (node.PointsTo == _head.PointsTo) return; // base case: leaf node
            EnumerateSubtree(node._Left);
            
            var e = node._Myval;
            _elements.Add(e);
            
            EnumerateSubtree(node._Right);
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
            _elements = other._elements;
        }

        public override void OnCreateInstance(string typename, Match match)
        {
            Debug.Assert(match.Groups.Count == 2, "Set expects 1 group.");
            ValueType = Parser.Parse(match.Groups[1].Value);
        }

        #endregion
    }
}

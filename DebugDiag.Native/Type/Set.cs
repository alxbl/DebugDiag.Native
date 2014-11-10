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
        private NativeType _head;

        #region Type Implementation

        protected override void Rebase()
        {
            base.Rebase();
            Size = GetIntValue("_Mysize");
            _head = GetField("_Myhead");
        }

        public override IEnumerator<NativeType> GetEnumerator()
        {
            if (Size == 0) yield break;
            if (_built)
            {
                foreach (var e in _elements) yield return e;
                yield break;
            }
            dynamic root = _head.GetField("_Parent");
            yield return EnumerateSubtree(root);
            _built = true;
        }

        /// <summary>
        /// In-order traversal of the tree.
        /// </summary>
        /// <param name="node">One node in the tree.</param>
        /// <returns></returns>
        private IEnumerable<NativeType> EnumerateSubtree(dynamic node)
        {
            if (node.Address == _head.Address) yield break; // base case: leaf node
            yield return EnumerateSubtree(node._Left);
            var e = node._Myval;
            _elements.Add(e);
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
            _elements = other._elements;
        }

        internal override void OnCreateInstance(string typename, Match match)
        {
            Debug.Assert(match.Groups.Count == 2, "Set expects 1 group.");
            ValueType = TypeParser.Parse(match.Groups[1].Value);
        }

        #endregion
    }
}

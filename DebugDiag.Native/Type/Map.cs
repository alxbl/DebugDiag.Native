using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DebugDiag.Native.Type
{
    public sealed class Map : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::map<(.*),(.*),.*,std::allocator<std::pair<.*> > >$");
        // Keep a cached copy of the instances to avoid constantly querying the dump file.
        private readonly List<NativeType> _elements = new List<NativeType>();
        private bool _built;
        private Pointer _head;

        public NativeType KeyType { get; private set; }

        #region Type Implementation

        protected override void Rebase()
        {
            base.Rebase();
            Size = GetIntValue("_Mysize");
            _head = GetField("_Myhead") as Pointer;
            Debug.Assert(_head != null, "Map cannot have a null head node.");
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
        /// <returns></returns>
        private void EnumerateSubtree(dynamic node)
        {
            if (node.PointsTo == _head.PointsTo) return; // base case: leaf node
            EnumerateSubtree(node._Left);
            
            // Fake that this is an instance, so that it behaves as expected.
            var e = new Pair { Address = node._Myval.Address, IsInstance = true, First = node._Myval.first, Second = node._Myval.second };
            _elements.Add(e);

            EnumerateSubtree(node._Right);
        }

        #endregion
        #region Constructor

        protected override NativeInstance DeepCopy()
        {
            return new Map(this);
        }


        public Map(Map other)
            : base(other)
        {
            _elements = other._elements;
        }

        public Map(string typename)
            : base(typename)
        {
        }

        public override void OnCreateInstance(string typename, Match match)
        {
            Debug.Assert(match.Groups.Count == 3, "Map expects 2 groups.");
            KeyType = Parser.Parse(match.Groups[1].Value);
            ValueType = Parser.Parse(match.Groups[2].Value);
        }

        #endregion
    }
}

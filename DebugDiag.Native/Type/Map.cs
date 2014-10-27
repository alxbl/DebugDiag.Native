using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DebugDiag.Native.Type
{
    public class Map : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::map<(.*),(.*),.*,std::allocator<std::pair<.*> > >$");
        // Keep a cached copy of the instances to avoid constantly querying the dump file.
        private readonly List<NativeType> _elements = new List<NativeType>();
        private bool _built;
        private NativeType _Myhead;

        public NativeType KeyType { get; private set; }
        public NativeType ValueType { get; private set; }

        #region Type Implementation
        protected override void Rebase()
        {
            base.Rebase();
            Size = GetIntValue("_Mysize");
            _Myhead = GetField("_Myhead");
            
        }

        public override IEnumerator<NativeType> GetEnumerator()
        {
            if (Size == 0) yield break;
            if (_built)
            {
                foreach (var e in _elements) yield return e;
                yield break;
            }
            dynamic root = _Myhead.GetField("_Parent");
            yield return EnumerateSubtree(root);
            _built = true;
        }

        /// <summary>
        /// In-order traversal of the tree.
        /// </summary>
        /// <param name="node">One node in the tree.</param>
        /// <returns></returns>
        private IEnumerable<Pair> EnumerateSubtree(dynamic node)
        {
            if (node.Address == _Myhead.Address) yield break; // base case: leaf node
            yield return EnumerateSubtree(node._Left);
            var e = new Pair {First = node._Myval.first, Second = node._Myval.second};
            _elements.Add(e);
            yield return e;
            yield return EnumerateSubtree(node._Right);
        }
        #endregion

        #region Constructor
        protected override NativeInstance DeepCopy()
        {
            return new Map(this);
        }


        public Map(Enumerable other) : base(other)
        {
        }

        public Map(string typename) : base(typename)
        {
        }

        internal override void OnCreateInstance(string typename, Match match)
        {
            Debug.Assert(match.Groups.Count == 3, "Map expects 2 groups.");
            KeyType = TypeParser.Parse(match.Groups[1].Value);
            ValueType = TypeParser.Parse(match.Groups[2].Value);
        }
        #endregion
    }
}

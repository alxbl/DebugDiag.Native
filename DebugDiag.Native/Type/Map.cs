﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    public sealed class Map : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::map<(.*),(.*),.*,std::allocator<(std::pair<.*>) > >$");
        // Keep a cached copy of the instances to avoid constantly querying the dump file.
        private readonly List<Pair> _elements = new List<Pair>();
        private ulong _offset; // The offset of std::pair<K,V>::second

        public NativeType KeyType { get; private set; }

        #region Type Implementation

        protected override void Rebase()
        {
            base.Rebase();
            Size = GetIntValue("_Mysize");
        }

        public override IEnumerator<NativeType> GetEnumerator()
        {
            if (Size == 0) yield break;
            if (_elements.Count == 0)
            {
                // If there are no elements in the list, we need to extract it from the dump.
                // This will only extract the base address of the pairs to maximize performance.
                var foreachStl = new ForeachStl(ForeachStl.Type.Map, Address);
                _elements.AddRange(foreachStl.GetElements().Select(x => new Pair(){Address = x, IsInstance = false, First = null, Second = null}));
            }

            foreach (var e in _elements)
            {
                if (!e.IsInstance)
                {
                    // This is our first iteration on this collection, we need to instantiate the dump objects.
                    // Pair has its data embedded directly in the type, so we can use offset manipulations.
                    e.First = AtAddress(e.Address, KeyType.TypeName);
                    e.Second = AtAddress(e.Address + _offset, ValueType.TypeName);
                    e.IsInstance = true;
                }
                yield return e;
            }
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
            KeyType = other.KeyType;
            _elements = other._elements; // We can do shallow copies since these should never change.
        }

        public Map(string typename)
            : base(typename)
        {
        }

        public override void OnCreateInstance(string typename, Match match)
        {
            Debug.Assert(match.Groups.Count == 4, "Map expects 3 groups.");

            var first = match.Groups[1].Value;
            var second = match.Groups[2].Value;
            var allocator = match.Groups[3].Value;

            // Find the offset for the pair's `second`. Add the const keyword to match the actual std::pair type.
            var pair = Preload(allocator);
            _offset = pair.GetOffset("second");

            // Create the native types for later instantiation
            KeyType = Parser.Parse(first);
            ValueType = Parser.Parse(second);


        }

        #endregion
    }
}

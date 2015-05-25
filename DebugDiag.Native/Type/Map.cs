using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    public sealed class Map : Enumerable
    {
        public static readonly Regex Syntax = new Regex(@"^std::map<(.*),(.*),.*,std::allocator<(std::pair<.*>) > >$");
        private ulong _offset; // The offset of std::pair<K,V>::second
        private string _allocator;

        public NativeType KeyType { get; private set; }

        #region Type Implementation

        protected override void Rebase()
        {
            base.Rebase();
            Size = GetField("_Mysize");
        }

        public override IEnumerable<NativeType> EnumerateInternal()
        {
            if (_offset == 0)
            {
                // Find the offset for the pair's `second`. Add the const keyword to match the actual std::pair type.
                var pair = Preload(_allocator);
                _offset = pair.GetOffset("second");
            }

            if (Size == 0) yield break;
            
            var elements = new List<Pair>();
            // This will only extract the base address of the pairs to maximize performance.
            var foreachStl = new ForeachStl(ForeachStl.Type.Map, Address);
                
            elements.AddRange(foreachStl.GetElements().Select(x => new Pair(){TypeName = _allocator, Address = x, IsInstance = false, First = null, Second = null}));

            foreach (var e in elements)
            {
                // This is our first iteration on this collection, we need to instantiate the dump objects.
                // Pair has its data embedded directly in the type, so we can use offset manipulations.
                // TODO: Possible Optimization when (KeyType is Primitive) or (ValueType is Primitive)?
                // -> Get the value directly instead of going through .AtAddress.
                e.First = AtAddress(e.Address, KeyType.TypeName);
                e.Second = AtAddress(e.Address + _offset, ValueType.TypeName);
                e.IsInstance = true;
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
            _offset = other._offset;
            _allocator = other._allocator;
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

            _allocator = match.Groups[3].Value;

            // Create the native types for later instantiation
            KeyType = Parser.Parse(first);
            ValueType = Parser.Parse(second);


        }

        #endregion
    }
}

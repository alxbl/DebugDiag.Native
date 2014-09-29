using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DebugDiag.Native
{
    /// <summary>
    /// Stores information about a native type as well as the instance it is linked to.
    /// </summary>
    public class NativeType
    {
        #region Public API

        /// <summary>
        /// Whether this NativeType object represents an object instance.
        /// </summary>
        public bool IsInstance { get; private set; }

        /// <summary>
        /// The base address of this object instance.
        /// </summary>
        public ulong Address { get; private set; }

        /// <summary>
        /// The module in which this type is defined.
        /// </summary>
        public string ModuleName { get; private set; }
        /// <summary>
        /// The name of the type.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// The fully qualified name of this type ([module]![type])
        /// </summary>
        public string QualifiedName { get { return String.Format("{0}!{1}", ModuleName, TypeName); } }

        /// <summary>
        /// True if this type is virtual and has a Vtable entry.
        /// </summary>
        public bool HasVtable { get; private set; }

        /// <summary>
        /// Returns whether this object instance is a primitive type.
        /// 
        /// A primitive type is one of: Pointer, Char, Short, Int or Long.
        /// </summary>
        public bool IsPrimitive
        {
            get { return _type != Native.PrimitiveType.Object && IsInstance; }
        }

        /// <summary>
        /// Returns an instance to an object's field.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>A NativeType instance representing the field.</returns>
        public NativeType GetField(string name)
        {
            if (string.IsNullOrEmpty(name) || !_nameLookup.ContainsKey(name))
                throw new Exception(String.Format("The field `{0}` does not exist in type `{1}`", name, QualifiedName));

            var o = _nameLookup[name];
            return GetInstance(o);
        }

        /// <summary>
        /// Returns an instance to an object's field.
        /// </summary>
        /// <param name="offset">The field name.</param>
        /// <returns>A NativeType instance representing the field.</returns>
        public NativeType GetField(ulong offset)
        {
            if (offset == 0) return this; // Type+0x0 == Type
            if (!_offsetLookup.ContainsKey(offset))
                throw new Exception(String.Format("The offset `+0x{0:03x}` does not exist in type `{1}`", offset, QualifiedName));
            var o = _offsetLookup[offset];
            return GetInstance(o);
        }

        #region Primitive Access

        public ulong GetIntValue()
        {
            return _rawMem;
        }

        public string GetStringValue()
        {
            return Native.Context.Execute(String.Format("ds {0}", _rawMem));
        }

        public string GetUnicodeStringValue()
        {
            return Native.Context.Execute(String.Format("du {0}", _rawMem));
        }
        
        #endregion
        #endregion

        #region Private API

        private static Regex _vtableFormat = new Regex(@" *([^ :]+)::`vftable'");
        // TODO: Type instances should be globally cached in case the user reuses AtAddress(); ?
        /// <summary>
        /// The type instance's offset table, indexed by field name.
        /// </summary>
        private readonly IDictionary<string, Offset> _nameLookup = new Dictionary<string, Offset>();

        /// <summary>
        /// The type instance's offset table, indexed by field offset.
        /// </summary>
        private readonly IDictionary<ulong, Offset> _offsetLookup = new Dictionary<ulong, Offset>();

        /// <summary>
        /// The memory value at this instance's address. Useful for primitive types.
        /// </summary>
        private ulong _rawMem;

        /// <summary>
        /// The type of primitive this instance is.
        /// </summary>
        private Native.PrimitiveType _type;

        private NativeType()
        {
            IsInstance = false; // When a default object is constructed, it is not an instance.
        }

        /// <summary>
        /// Returns the type instance at a given offset of this type.
        /// 
        /// If the native type at the given offset cannot be parsed, this method returns null.
        /// </summary>
        /// <param name="o">The offset information object.</param>
        /// <returns>An instance of the NativeType at the given offset.</returns>
        private NativeType GetInstance(Offset o)
        {
            if (o.Instance != null) return o.Instance;
            // The type has not been initialized. Do so.
            o.Instance = AtAddress(o.Address, o.TypeName);
            return o.Instance;
        }
        
        /// <summary>
        /// Loads a copy of the type information from the cache.
        /// 
        /// This method assumes the type information is already cached.
        /// </summary>
        /// <param name="type">The name of the type</param>
        private void LoadFromCache(string type)
        {
            var cache = TypeLookup(type);
            Debug.Assert(cache != null);

            foreach (var kp in cache)
            {
                var o = new Offset(kp.Value); // Deep Copy.
                _nameLookup[kp.Key] = o;
                _offsetLookup[o.Bytes] = o;
            }
        }

        #region Type Cache
        private static readonly IDictionary<string, IDictionary<string, Offset>> _typeCache = new Dictionary<string, IDictionary<string, Offset>>();

        private static IDictionary<string, Offset> TypeLookup(string type)
        {
            if (!_typeCache.ContainsKey(type)) return null;
            return _typeCache[type];
        }

        private static void CacheType(string type, IDictionary<string, Offset> offsetTable)
        {
            _typeCache.Add(type, offsetTable);
        }

        private struct Offset
        {
            public Offset(Offset other) : this()
            {
                Address = other.Address;
                Bytes = other.Bytes;
                TypeName = other.TypeName;
                Type = other.Type;
                Instance = null; // Don't copy the instance data over.
            }

            /// <summary>
            /// Absolute address of this instance.
            /// </summary>
            public ulong Address { get; private set; }
            /// <summary>
            /// Offset bytes from the base address.
            /// </summary>
            public ulong Bytes { get; private set; }

            /// <summary>
            /// The name (fully qualified if possible) of the type at this offset.
            /// </summary>
            public string TypeName { get; private set; }

            /// <summary>
            /// Primitive Type information.
            /// </summary>
            public Native.PrimitiveType Type { get; private set; }

            /// <summary>
            /// The sub-instance, if it has been inspected.
            /// </summary>
            public NativeType Instance { get; internal set; }
        }
        #endregion
        #endregion

        #region Static API
        /// <summary>
        /// Preloads the type information of the given type. 
        /// 
        /// This calls the dbgeng.dll command "dt" on the type.
        /// </summary>
        /// <param name="type">The qualified name of the type (</param>
        /// <returns>Whether the type could be preloaded</returns>
        public static bool Preload(string type)
        {
            // Avoid extra work if we already preloaded that type.
            if (TypeLookup(type) != null) return true;

            var data = Native.Context.Execute(String.Format("dt {0}", type));

            // Type not found...
            if (string.IsNullOrWhiteSpace(data) || data.Equals(String.Format("Symbol {0} not found.", type))) return false;

            // Parse each offset.
            IDictionary<string, Offset> offsetTable = new Dictionary<string, Offset>();

            return true;
        }

        /// <summary>
        /// Attempts to retrieve a type located at the specified address.
        /// 
        /// This convenience method works on the assumption that the address pointed to is a virtual type and that it has a valid vtable pointer.
        /// 
        /// </summary>
        /// <param name="addr">String representation of the address.</param>
        /// <returns>An instance of native type at the given address</returns>
        public static NativeType AtAddress(string addr)
        {
            if (string.IsNullOrEmpty(addr) || !Native.AddressFormat.IsMatch(addr)) throw new ArgumentException("Invalid memory location.");

            // Look for a vtable.
            string vtable = Native.Context.Execute(String.Format("ln poi({0})", addr));

            var matches = _vtableFormat.Matches(vtable);
            if (matches.Count == 0) return null; // No matching vtable.
            Debug.Assert(matches.Count == 1); // There should never be more than one vtable for a given type.
            Debug.Assert(matches[0].Groups.Count == 2); // Full match & typename
            var s = matches[0].Groups[1].Value;
            return AtAddress(addr, s);
        }

        public static NativeType AtAddress(ulong addr)
        {
            if (addr == 0) throw new ArgumentException("Invalid memory location.");
            return AtAddress(String.Format("0x{0:x}", addr));
        }

        /// <summary>
        /// Attempts to parse the given address as the type specified as a parameter. 
        /// 
        /// This uses dbgeng.dll under the hood, so there is no way to validate if the parsed type is valid automatically.
        /// This method is useful for dumping POD types that do not have a vtable, but has a drawback that the type being dumped must be known at runtime.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static NativeType AtAddress(ulong addr, string type)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentException("Cannot lookup a null type. Use AtAddress(addr) for vtable lookup.");
            return AtAddress(String.Format("0x{0:x16}", addr), type);
        }

        public static NativeType AtAddress(string addr, string type)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentException("Cannot lookup a null type. Use AtAddress(addr) for vtable lookup.");
            
            // Preload the type if it hasn't been encountered.
            Preload(type);

            var output = Native.Context.Execute(String.Format("dt {0} {1}", addr, type));
            if (string.IsNullOrWhiteSpace(output)) return null;

            // Create an instance.
            var instance = new NativeType();
            instance.LoadFromCache(type); 

            // Parse the `dt` output and initialize the instance.
            instance.Address = Native.StringAddrToUlong(addr);
            //instance.InitializeFromDumpType(DumpType.Parse(output));
            return instance;
        }
        #endregion
    }
}

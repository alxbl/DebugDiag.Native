using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DebugDiag.Native.Windbg;

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
        /// The fully qualified name of this type ([module]![type]). If the type is a primitive, then this is the same as TypeName.
        /// </summary>
        public string QualifiedName
        {
            get
            {
                return !String.IsNullOrEmpty(ModuleName) ? String.Format("{0}!{1}", ModuleName, TypeName) : TypeName;
            }
        }

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

        public bool IsStatic { get; private set; }

        /// <summary>
        /// Returns an instance to an object's field.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>A NativeType instance representing the field.</returns>
        public NativeType GetField(string name)
        {
            if (IsPrimitive) throw new InvalidOperationException("Cannot call GetField() on a primitive type.");

            if (string.IsNullOrEmpty(name) || !_nameLookup.ContainsKey(name))
                throw new ArgumentOutOfRangeException(String.Format("The field `{0}` does not exist in type `{1}`", name, QualifiedName));

            var o = _nameLookup[name];
            return GetInstance(o);
        }

        /// <summary>
        /// Returns an instance to an object's field.
        /// </summary>
        /// <param name="offset">The field name.</param>
        /// <returns>A NativeType instance representing the field.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset does not exist for this type.</exception>
        public NativeType GetField(ulong offset)
        {
            if (IsPrimitive) throw new InvalidOperationException("Cannot call GetField() on a primitive type.");

            if (!_offsetLookup.ContainsKey(offset))
                throw new ArgumentOutOfRangeException(String.Format("The offset `+0x{0:03x}` does not exist in type `{1}`", offset, QualifiedName));
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

        private static readonly Regex _vtableFormat = new Regex(@" *([^ :]+)::`vftable'");

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

        private void ParseTypeName(string type)
        {
            var split = type.Split('!');
            Debug.Assert(split.Length == 2, "A fully qualified name consists of two parts.");
            ModuleName = split[0];
            TypeName = split[1];
        }

        /// <summary>
        /// Populates this instance based on the windbg output.
        /// </summary>
        /// <param name="dt">The parsed dt output from windbg.</param>s
        private void InitializeInstance(DumpType dt)
        {
            if (dt.TypeName != null)
            {
                ParseTypeName(dt.TypeName);
            }

            HasVtable = dt.IsVirtualType;

            bool first = true;
            foreach (var line in dt)
            {
                Debug.Assert(_offsetLookup.ContainsKey(line.Offset), "Type offset tables mismatched");
                var offset = _offsetLookup[line.Offset];

                if (!offset.IsStatic) offset.Address = Address + offset.Bytes; // Compute Absolute Address of this offset.
                // TODO: Handle bitfield details here.
                if (!line.IsBits) offset.RawMemory = Native.ParseWindbgPrimitive(line.Value);

                if (line.Offset == 0 && first)
                {
                    // Offset 0 is self, so populate this instance.
                    _rawMem = offset.RawMemory.HasValue ? offset.RawMemory.Value : 0;
                    // Should always have value though?
                    // What about scenario where the first field of a POD is also a POD?
                }
                first = false;
            }
        }

        /// <summary>
        /// Returns the type instance at a given offset of this type.
        /// 
        /// If the native type at the given offset cannot be parsed, this method returns null.
        /// </summary>
        /// <param name="o">The offset information object.</param>
        /// <returns>An instance of the NativeType at the given offset.</returns>
        private static NativeType GetInstance(Offset o)
        {
            if (o.Instance != null) return o.Instance;

            // Handle primitive fields.
            if (o.Type != Native.PrimitiveType.Object)
            {
                o.Instance = new NativeType { IsInstance = true, HasVtable = false, TypeName = o.TypeName, Address = o.Address, IsStatic = o.IsStatic };
                Debug.Assert(o.RawMemory.HasValue, "A primitive type should always have a raw value available.");
                o.Instance._rawMem = o.RawMemory ?? 0;
                o.Instance._type = o.Type;
            }
            else
                o.Instance = AtAddress(o.Address, o.TypeName);
            return o.Instance;
        }

        #region Type Cache
        private static readonly IDictionary<string, IDictionary<string, Offset>> TypeCache = new Dictionary<string, IDictionary<string, Offset>>();

        private static IDictionary<string, Offset> TypeLookup(string type)
        {
            return !TypeCache.ContainsKey(type) ? null : TypeCache[type];
        }

        private static void CacheType(string type, IDictionary<string, Offset> offsetTable)
        {
            TypeCache[type] = offsetTable;
        }

        /// <summary>
        /// Loads a copy of the type's offset table for this type instance.
        ///
        /// If the type hasn't been discovered yet, attempts to discover it.
        /// </summary>
        /// <param name="type">The name of the type</param>
        /// <exception cref="TypeDoesNotExistException">Thrown when the type does not have symbols available or the type cannot be found.</exception>
        private void LoadOffsetTable(string type)
        {
            var cache = TypeLookup(type);
            if (cache == null)
            {
                Preload(type);
                cache = TypeLookup(type);
                Debug.Assert(cache != null); // Should have been preloaded now. Otherwise there is a problem.
            }

            foreach (var kp in cache)
            {
                var o = new Offset(kp.Value); // Deep Copy.
                if (!o.IsStatic) o.Address = Address + o.Bytes; // Compute the absolute address of this offset unless it is static.
                _nameLookup[kp.Key] = o;

                if (!_offsetLookup.ContainsKey(o.Bytes)) // Do not overwrite with Bitfield details.
                    _offsetLookup[o.Bytes] = o; // TODO: Support Bitfield details.
            }
        }

        /// <summary>
        /// Represents a type's field at a specific offset. Internal structure used to navigate instances.
        /// </summary>
        private class Offset
        {
            public Offset()
            {
            }

            public Offset(Offset other)
            {
                Bytes = other.Bytes;
                TypeName = other.TypeName;
                Type = other.Type;
                Instance = null; // Don't copy the instance data over.
                IsStatic = other.IsStatic;
            }

            /// <summary>
            /// Absolute address of this instance.
            /// </summary>
            public ulong Address { get; internal set; }

            /// <summary>
            /// Offset bytes from the base address.
            /// </summary>
            public ulong Bytes { get; internal set; }

            /// <summary>
            /// The name (fully qualified if possible) of the type at this offset.
            /// </summary>
            public string TypeName { get; internal set; }

            /// <summary>
            /// Primitive Type information.
            /// </summary>
            public Native.PrimitiveType Type { get; internal set; }

            /// <summary>
            /// The sub-instance, if it has been inspected.
            /// </summary>
            public NativeType Instance { get; internal set; }

            /// <summary>
            /// The raw memory value at that offset (for primitive types)
            /// </summary>
            public ulong? RawMemory { get; internal set; }

            /// <summary>
            /// Whether this "offset" represents a static type.
            /// 
            /// When this is true, RawMemory is null and Bytes is equal to Address 
            /// since the offset really points to an arbitrary memory location.
            /// </summary>
            public bool IsStatic { get; internal set; }
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
        /// <returns>The preloaded type information. This is not an instance.</returns>
        /// <exception cref="ArgumentException">When the type cannot be found or preloaded</exception>
        public static void Preload(string type)
        {
            // Avoid extra work if we already preloaded that type.
            if (TypeLookup(type) != null) return;

            var data = Native.Context.Execute(String.Format("dt {0}", type));

            // Type not found...
            if (string.IsNullOrWhiteSpace(data) || data.Contains(String.Format("Symbol {0} not found.", type)))
                throw new TypeDoesNotExistException(String.Format("Symbols {0} not found.", type));

            // Parse each offset.
            IDictionary<string, Offset> offsetTable = new Dictionary<string, Offset>();

            DumpType dt;
            try
            {
                dt = DumpType.Parse(data);
                var vtableCount = 0;
                foreach (var line in dt)
                {
                    // TODO: FIXME: Need to handle nested vtable name collisions in order to be able to inspect them by name.
                    // This is a really hacky bandage fix until a clean solution is designed. It has the
                    // limitation that vtable occurences cannot be lookup by name. Offset still works.
                    string name = (line.Name == "__VFN_table") ? line.Name + vtableCount++ : line.Name;
                    offsetTable.Add(name, new Offset()
                                               {
                                                   // If the offset is static, we already know its address. Otherwise it will be computed by InitializeInstance().
                                                   Address = line.IsStatic ? line.Offset : 0, 
                                                   Bytes = line.Offset,
                                                   Instance = null,
                                                   Type = Native.TypeFromString(line.Value),
                                                   TypeName = line.Value, // We know value here is the type.
                                                   IsStatic = line.IsStatic
                                               });
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception x)
            {
                throw new TypeDoesNotExistException(String.Format("An error occured while preloading symbol {0}:\n{1}", type, x), x);
            }

            // Cache the offset table.
            if (dt.TypeName != null) // Also cache the fully qualified type name.
                CacheType(dt.TypeName, offsetTable);
            CacheType(type, offsetTable);
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
        /// <param name="addr">The address to inspect.</param>
        /// <param name="type">The type name located at that address.</param>
        /// <returns>An instance of NativeType.</returns>
        public static NativeType AtAddress(ulong addr, string type)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentException("Cannot lookup a null type. Use AtAddress(addr) for vtable lookup.");
            return AtAddress(String.Format("0x{0:x}", addr), type);
        }

        public static NativeType AtAddress(string addr, string type)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentException("Cannot lookup a null type. Use AtAddress(addr) for vtable lookup.");

            // Preload the type if it hasn't been encountered.
            Preload(type);

            var output = Native.Context.Execute(String.Format("dt {0} {1}", type, addr)); // TODO: Always use qualified type name internally.
            if (string.IsNullOrWhiteSpace(output)) return null;

            // Create an instance.
            var instance = new NativeType { IsInstance = true, Address = Native.StringAddrToUlong(addr) };
            instance.LoadOffsetTable(type);

            if (type.Contains("!"))
            {
                instance.ParseTypeName(type);
            }

            // Parse the `dt` output and initialize the instance.            
            instance.InitializeInstance(DumpType.Parse(output));

            return instance;
        }

        #endregion
    }
}

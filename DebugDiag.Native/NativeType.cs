using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Text.RegularExpressions;
using DebugDiag.Native.Type;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native
{
    /// <summary>
    /// Stores information about a native type as well as the instance it is linked to. 
    /// This class is externally immutable to avoid mishaps while digging a dump.
    /// </summary>
    public class NativeType : NativeInstance
    {
        #region Constants

        public const ulong InvalidOffset = ulong.MaxValue;

        #endregion
        #region Dynamic API

        public dynamic Dynamic { get { return this; } }

        /// <summary>
        /// Allows to use the member accessors to navigate types easily.
        /// 
        /// <code>
        ///     NativeType foo = NativeType.AtAddress(0x3c0ffee5, "MyApp!Foo");
        ///     NativeType bar = foo.bar; // Accesses a field "bar" in type "MyApp!Foo".
        ///     Console.WriteLine("There are {0} bars in Foo.", bar.GetIntValue());
        /// </code>
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetField(binder.Name); // Might throw.
            return true;
        }

        #endregion
        #region Type Information

        /// <summary>
        /// The module in which this type is defined.
        /// 
        /// If the module is unknown, this is an empty string.
        /// </summary>
        public string ModuleName { get; private set; }

        /// <summary>
        /// The name of the type.
        /// </summary>
        public string TypeName { get; internal set; }

        /// <summary>
        /// The fully qualified name of this type ([module]![type]). 
        /// 
        /// If the type is a primitive or the module unknown, then this is the same as TypeName.
        /// </summary>
        public string QualifiedName
        {
            get
            {
                return !string.IsNullOrEmpty(ModuleName) ? string.Format("{0}!{1}", ModuleName, TypeName) : TypeName;
            }
        }

        /// <summary>
        /// True if this type is virtual and has a Vtable entry.
        /// </summary>
        public bool HasVtable { get; private set; }

        /// <summary>
        /// Returns whether this type is a static instance.
        /// 
        /// If the type is not an instance, this is false. 
        /// </summary>
        public bool IsStatic { get; private set; }

        #endregion
        #region Instance Navigation

        /// <summary>
        /// Returns an instance to an object's field.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>A NativeType instance representing the field.</returns>
        public override NativeType GetField(string name)
        {
            CheckInstance();

            if (string.IsNullOrEmpty(name) || !_nameLookup.ContainsKey(name))
                throw new ArgumentOutOfRangeException(string.Format("The field `{0}` does not exist in type `{1}`", name, QualifiedName));

            var o = _nameLookup[name];
            return GetInstance(o);
        }

        /// <summary>
        /// Returns an instance to an object's field.
        /// </summary>
        /// <param name="offset">The field name.</param>
        /// <returns>A NativeType instance representing the field.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset does not exist for this type.</exception>
        public override NativeType GetField(ulong offset)
        {
            CheckInstance();

            if (!_offsetLookup.ContainsKey(offset))
                throw new ArgumentOutOfRangeException(string.Format("The offset `+0x{0:x04}` does not exist in type `{1}`", offset, QualifiedName));
            var o = _offsetLookup[offset];
            return GetInstance(o);
        }

        /// <summary>
        /// Returns the string representation of the NativeType instance.
        /// 
        /// Unless overridden, the default behavior is to return the typename, and its address.
        /// Primitive types override this method to return their leaf value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("NativeType {{ IsInstance={0}, Address=0x{1:x}, TypeName={2} }}", IsInstance, Address, QualifiedName);
        }

        #endregion
        #region Type Discovery

        /// <summary>
        /// Preloads the type information of the given type.
        /// 
        /// The returned NativeType represents the uninstantiated type. It can be used to query the field offsets
        /// and other information about the type.
        /// This calls the dbgeng.dll command "dt" on the type.
        /// </summary>
        /// <param name="type">The qualified name of the type (</param>
        /// <returns>The preloaded type information. This is not a type instance.</returns>
        /// <exception cref="CommandException">When the type cannot be found or preloaded.</exception>
        public static NativeType Preload(string type)
        {
            // Avoid extra work if we already preloaded that type.
            var typeInfo = TypeLookup(type);
            if (typeInfo != null) return typeInfo;

            typeInfo = Parser.Parse(type);

            typeInfo.BuildOffsetTable(type);

            CacheType(typeInfo.TypeName, typeInfo);
            CacheType(typeInfo.QualifiedName, typeInfo);
            return typeInfo;
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

            return AtAddressInternal(addr, Native.StringAddrToUlong(addr));
        }

        public static NativeType AtAddress(ulong addr)
        {
            if (addr == 0) throw new ArgumentException("Invalid memory location.");
            return AtAddressInternal(string.Format("0x{0:x}", addr), addr);
        }

        public static NativeType AtAddress(string addr, string type)
        {
            return AtAddressInternal(Native.StringAddrToUlong(addr), type);
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
            return AtAddressInternal(addr, type);

        }

        /// <summary>
        /// Returns the offset of a type's field.
        /// </summary>
        /// <param name="field">The field for which to find the offset.</param>
        /// <exception cref="ArgumentException">Thrown when the field does not exist.</exception>
        /// <returns>The offset of the field.</returns>
        public virtual ulong GetOffset(string field)
        {
            if (!_nameLookup.ContainsKey(field))
                throw new ArgumentException(string.Format("Field `{0}` does not exist in type `{1}`.", field, TypeName), "field");
            return _nameLookup[field].Bytes;
        }

        /// <summary>
        /// Returns whether an offset exists in the type.
        /// </summary>
        /// <param name="offset">The offset to look up.</param>
        /// <returns>Whether the offset exists.</returns>
        public virtual bool HasOffset(ulong offset)
        {
            return _offsetLookup.ContainsKey(offset);
        }

        #endregion
        #region Casts

        /// <summary>
        /// Allows to implicitly cast an instance to a string.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static implicit operator string(NativeType i)
        {
            return i.ToString();
        }
        
        public static explicit operator uint(NativeType i)
        {
            return i.ToUInt32();
        }

        public static explicit operator float(NativeType i)
        {
            return i.ToFloat();
        }

        public static explicit operator double(NativeType i)
        {
            return i.ToDouble();
        }

        public static explicit operator bool(NativeType i)
        {
            return i.ToBool();
        }
        
        public static explicit operator long(NativeType i)
        {
            return i.ToInt64();
        }
        
        /// <summary>
        /// Allows to implicitly cast an instance to an unsigned long.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static implicit operator ulong(NativeType i)
        {
            return i.ToUInt64();
        }

        #endregion
        #region Private API
        #region Casting Default Implementation

        private T RaiseUnsupportedCast<T>()
        {
            throw new InvalidCastException(string.Format("Cast from `{0}` to `{1}` is not supported", GetType().Name, typeof(T).Name));
        }

        // This implements a default behavior of failure for all casts.
        protected override bool ToBool()
        {
            return RaiseUnsupportedCast<bool>();
        }

        protected override float ToFloat()
        {
            return RaiseUnsupportedCast<float>();
        }

        protected override double ToDouble()
        {
            return RaiseUnsupportedCast<double>();
        }

        protected override int ToInt32()
        {
            return RaiseUnsupportedCast<int>();
        }

        protected override uint ToUInt32()
        {
            return RaiseUnsupportedCast<uint>();
        }

        protected override long ToInt64()
        {
            return RaiseUnsupportedCast<long>();
        }

        protected override ulong ToUInt64()
        {
            CheckInstance();
            if (_rawMem.HasValue) return _rawMem.Value;

            var dp = new Dp(Address, 1); // TODO: Switch to Format()
            _rawMem = dp.BytesAt(0);
            return _rawMem.Value;
        }


        #endregion
        internal NativeType()
        {
            IsInstance = false; // When a default object is constructed, it is not an instance.
        }

        internal NativeType(string typename)
        {
            ParseTypeName(typename);
        }

        private static NativeType AtAddressInternal(ulong addrUlong, string type)
        {
            // Preload the type if it hasn't been encountered.
            var typeInfo = Preload(type);
            return typeInfo.RebaseAt(addrUlong);
        }

        private static NativeType AtAddressInternal(string addrStr, ulong addrUlong)
        {
            // Look for a vtable.
            string vtable = Native.Context.Execute(string.Format("ln poi({0})", addrStr));

            var matches = VtableFormat.Matches(vtable);
            if (matches.Count == 0) return null; // No matching vtable.
            Debug.Assert(matches.Count == 1); // There should never be more than one vtable for a given type.
            Debug.Assert(matches[0].Groups.Count == 2); // Full match & typename
            var s = matches[0].Groups[1].Value;

            return AtAddressInternal(addrUlong, s);
        }

        /// <summary>
        /// Checks that this type is instantiated, and throws an exception if it is not.
        /// </summary>
        protected void CheckInstance()
        {
            if (!IsInstance) throw new InvalidOperationException("This method needs to be called on an instantiated type.");
        }

        private void ParseTypeName(string type)
        {
            if (type.Contains("!"))
            {
                var split = type.Split('!');
                Debug.Assert(split.Length == 2, "A fully qualified name consists of two parts.");
                ModuleName = split[0];
                TypeName = split[1];
            }
            else // unknown module name.
            {
                ModuleName = "";
                TypeName = type;
            }
        }

        private static readonly Regex VtableFormat = new Regex(@" *([^ :]+)::`vftable'");

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
        private ulong? _rawMem;

        #region Type Instantiation

        /// <summary>
        /// Instantiates a type at a given base memory address. This creates a copy of the type tree
        /// and initializes the type instance based on the memory starting at the provided address.
        /// 
        /// Offsets will be recomputed based on the new address, and type navigation methods will
        /// work.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public NativeType RebaseAt(ulong addr)
        {
            Debug.Assert(!IsInstance, "Should not re-base an instance."); // Really?
            // Rebase things.
            var instance = (NativeType)DeepCopy();
            instance.IsInstance = true;
            instance.Address = addr;
            instance.Rebase();
            return instance;
        }

        /// <summary>
        /// Rebase a generic native type with no special treatment.
        /// 
        /// For NativeType, this method runs a `dt` on the instance, and tries to parse primitive values into the type.
        /// On top of that, it initializes the offset table addresses for this particular instance.
        /// </summary>
        protected override void Rebase()
        {
            var dt = new DumpType(QualifiedName, Address);
            dt.Execute();

            foreach (var l in dt)
            {
                Debug.Assert(_offsetLookup.ContainsKey(l.Offset), "Type offset tables mismatched");
                var o = _offsetLookup[l.Offset];
                if (!o.IsStatic) o.Address = Address + o.Bytes; // Compute the absolute address of this offset unless it is static.

                // Retrieve the instance value of primitive fields.
                if (o.IsPrimitive && !l.IsBits)
                {
                    o.Instance = Parser.Parse(l, o.TypeName, /* isInstance: */ true);
                    o.Instance.IsInstance = true;
                    o.Instance.IsStatic = o.IsStatic;
                    o.Instance.Address = Address + o.Bytes;
                    o.Instance.HasVtable = false;
                }

                // Update the offset.
                _offsetLookup[l.Offset] = o;
                _nameLookup[l.Name] = o;
            }

        }

        protected override void BuildOffsetTable(string type)
        {
            var dt = new DumpType(type);
            dt.Execute(); // Will throw if dt fails.

            if (dt.TypeName != null)
            {
                ParseTypeName(dt.TypeName);
            }
            HasVtable = dt.IsVirtualType;

            var vtableCount = 0;
            foreach (var line in dt)
            {
                // TODO: FIXME: Need to handle nested vtable name collisions in order to be able to inspect them by name.
                // This is a really hacky bandage fix until a clean solution is designed. It has the
                // limitation that vtable occurrences cannot be looked up by name. Offset still works.
                string name = (line.Name == "__VFN_table") ? line.Name + vtableCount++ : line.Name;

                // TODO: Possible optimization store this stub in the cache and lazily inspect it to avoid double parsing.
                // When building the offset table, there is no instance information available so we tell the parser not
                // to bother with instance information.
                var fieldInfo = Parser.Parse(line, line.Detail, /*isInstance:*/ false);

                var offset = new Offset
                             {
                                 // If the offset is static, we already know its address. Otherwise it will be computed at rebase time.
                                 Address = line.IsStatic ? line.Offset : 0,
                                 Bytes = line.Offset,
                                 Instance = null,
                                 TypeName = fieldInfo.TypeName,
                                 IsPrimitive = (fieldInfo is Primitive),
                                 IsStatic = line.IsStatic
                             };

                // Create the offset tables.
                if (!line.IsBits) // TODO: Handle bit fields.
                {
                    _nameLookup[name] = offset;
                    _offsetLookup[offset.Bytes] = offset;
                }
                // typeInfo._preloaded = true;
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

            Debug.Assert(!o.IsPrimitive, "Primitive instances should always be populated during re-basing.");

            o.Instance = AtAddress(o.Address, o.TypeName);
            return o.Instance;
        }

        #endregion
        #region Type Cache

        private static readonly IDictionary<string, NativeType> TypeCache = new Dictionary<string, NativeType>();

        private static NativeType TypeLookup(string type)
        {
            return !TypeCache.ContainsKey(type) ? null : TypeCache[type];
        }

        private static void CacheType(string type, NativeType typeInfo)
        {
            TypeCache[type] = typeInfo;
        }

        #endregion
        #endregion
        #region Copy

        protected override NativeInstance DeepCopy()
        {
            // Copy Type Information.
            return new NativeType(this);
        }

        protected NativeType(NativeType other) :
            this()
        {
            IsInstance = other.IsInstance;
            Address = other.Address;
            ModuleName = other.ModuleName;
            TypeName = other.TypeName;
            HasVtable = other.HasVtable;
            IsStatic = other.IsStatic;
            _rawMem = other._rawMem;

            // Copy offset tables.
            foreach (var o in other._nameLookup) 
                _nameLookup.Add(o.Key, o.Value.DeepCopy());
            foreach (var o in other._offsetLookup) 
                _offsetLookup.Add(o.Key, o.Value.DeepCopy());
        }

        #endregion
    }
}

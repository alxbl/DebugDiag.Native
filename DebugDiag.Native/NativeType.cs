using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native
{
    /// <summary>
    /// Represents a native type in the debugger.
    /// </summary>
    public class NativeType
    {
        #region Properties
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
        #endregion

        #region Private fields
        /// <summary>
        /// The type's offset table.
        /// </summary>
        private readonly IDictionary<string, ulong> _offsetTable = new Dictionary<string, ulong>();

        #endregion

        #region Private API

        private static void BuildOffsetTable(string data)
        {
            // Check if we've already computed the offset table for this type.
            // Parse each offset.
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Preloads the type information of the given type. 
        /// 
        /// This calls the dbgeng.dll command "dt" on the type.
        /// </summary>
        /// <param name="type">The qualified name of the type (</param>
        /// <returns>Whether the type could be preloaded</returns>
        public static bool Preload(string type)
        {
            var data = Native.Context.Execute(String.Format("dt {0}", type));
            if (data.Equals(String.Format("Symbol {0} not found.", type))) return false;

            BuildOffsetTable(data);
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
            return null;
        }

        public static NativeType AtAddress(ulong addr)
        {
            return null;
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
            throw new NotImplementedException();
        }
    }
}

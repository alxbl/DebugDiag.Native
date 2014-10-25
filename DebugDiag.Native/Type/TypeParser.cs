using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Responsible for parsing a typename string into a NativeType object.
    /// 
    /// Provides extension services to register user-defined types into the parser.
    /// </summary>
    public static class TypeParser
    {
        public static NativeType Parse(string typename)
        {
            var unqualifiedType = (typename.Contains("!")) ? typename.Split('!')[1] : typename; // Ignore module name.

            NativeType type = null;

            // In order of priority.
            if (unqualifiedType.EndsWith("*")) // Pointer: Remove one `*` and parse the remaining type.
            {
                // type = new Pointer(typename, Parse(typename.Substring(0, typename.Length - 1)));
            }
            else if (IsPrimitive(unqualifiedType)) // Primitive: Create the matching primitive type.
            {
                // Template black magic here?
                type = new Primitive();
            }
            else if (IsUserType(unqualifiedType, ref type)) // User-types: Let the extension handle creation.
            {
                // Handled by the extension type, nothing to do.
            }
            else // Default: unknown type. Can be explored by hand.
            {
                type = new NativeType();
            }

            return type;
        }

        private static bool IsPrimitive(string typename)
        {
            if (typename.Equals("Char")) return true;
            if (typename.Equals("UChar")) return true;
            if (typename.Equals("Int2B")) return true;
            if (typename.Equals("Uint2B")) return true;
            if (typename.Equals("Int4B")) return true;
            if (typename.Equals("Uint4B")) return true;
            if (typename.Equals("Int8B")) return true;
            if (typename.Equals("Uint8B")) return true;
            if (typename.StartsWith("Ptr32")) return true;
            if (typename.StartsWith("Ptr64")) return true;
            return false;
        }

        private static bool IsUserType(string typename, ref NativeType type)
        {
            return false;
        }
    }
}

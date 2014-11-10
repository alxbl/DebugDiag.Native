using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Responsible for parsing a typename string into a NativeType object.
    /// 
    /// Provides extension services to register user-defined types into the parser.
    /// </summary>
    public static class TypeParser
    {
        private static readonly Dictionary<Regex, System.Type> RegisteredUserTypes = new Dictionary<Regex, System.Type>();

        /// <summary>
        /// Adds a user type to the type parser. When parsing a type, the parser will try to match the typename
        /// against each type in its registered user types. If a match is found, the type parser will create an instance
        /// of that user type and return it.
        /// </summary>
        /// <typeparam name="T">Restricts this method to be called with types that extend UserType.</typeparam>
        /// <param name="pattern">A compiled regular expression user to match against this type.</param>
        /// <param name="type"></param>
        public static void RegisterUserType<T>(Regex pattern, T type) where T : UserType
        {
            Debug.Assert(pattern != null && type != null);
            RegisteredUserTypes[pattern] = type.GetType();
        }

        public static NativeType Parse(string typename)
        {
            var unqualifiedType = (typename.Contains("!")) ? typename.Split('!')[1] : typename; // Ignore module name.

            NativeType type = null;

            // In order of priority.
            if (unqualifiedType.StartsWith("Ptr32") || unqualifiedType.StartsWith("Ptr64")) // Pointer: windbg style pointers.
            {
                // TODO: Allow Vtable inspection.
                type = new Pointer(typename);
            }
            else if (unqualifiedType.EndsWith("*")) // Pointer: C++ style pointers.
            {
                type = new Pointer(typename);
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
                type = new NativeType(typename);
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
            return false;
        }

        private static bool IsUserType(string typename, ref NativeType type)
        {
            foreach (var ut in RegisteredUserTypes)
            {
                var pattern = ut.Key;
                var typeInfo = ut.Value;

                if (!pattern.IsMatch(typename)) continue;

                UserType instance = null;
                var cons = typeInfo.GetConstructor(new[] { typeof(string) });
                if (cons != null)
                    instance = cons.Invoke(new object[] { typename }) as UserType;

                if (instance != null)
                {
                    instance.OnCreateInstance(typename, pattern.Match(typename));
                    type = instance;
                }
                return true;
            }
            return false; // No matching user type found.
        }
    }
}

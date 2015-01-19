using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Responsible for parsing a typename string into a NativeType object.
    /// 
    /// Provides extension services to register user-defined types into the parser.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Decomposes a complex type into its type tree.
        /// </summary>
        /// <param name="typename">The string representing the type.</param>
        /// <returns>The root of the type tree</returns>
        public static NativeType Parse(string typename)
        {
            return Parse(null, typename, false);
        }


        #region User Type Extensibility

        private static readonly Dictionary<Regex, System.Type> RegisteredUserTypes = new Dictionary<Regex, System.Type>();
        /// <summary>
        /// Adds a user type to the type parser. When parsing a type, the parser will try to match the typename
        /// against each type in its registered user types. If a match is found, the type parser will create an dt
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

        #endregion
        #region Private API

        /// <summary>
        /// Decomposes a complex type into its type tree.
        /// 
        /// This overload also makes the distinction between whether it is parsing an instance of a type, or simply a type.
        /// When parsing an instance of a type, it will properly create primitives.
        /// 
        /// This function is only called internally.
        /// 
        /// TODO: FIXME: It should be clear from the API when to call Parse() with isInstance = true;
        /// </summary>
        /// <param name="dt">The `dt` output for this member</param>
        /// <param name="typename"></param>
        /// <param name="isInstance"></param>
        /// <returns></returns>
        internal static NativeType Parse(DumpType.Line? dt, string typename, bool isInstance)
        {
            var unqualifiedType = (typename.Contains("!")) ? typename.Split('!')[1] : typename; // Ignore module name.

            NativeType type = null;

            // In order of priority.
            if (Pointer.Syntax.IsMatch(unqualifiedType)) // A pointer or a string primitive (char*, wchar_t*)
            {
                // TODO: Allow Vtable inspection.
                type = dt.HasValue && isInstance ? 
                    new Pointer(typename, Primitive.ParseWindbgPrimitive(dt.Value.Detail).GetValueOrDefault()) // Instantiate the pointer when possible
                    : new Pointer(typename);

                // Special case: Pointer to string. This could be improved to not cause parsing of the tree and simplify the logic here.
                if (String.Syntax.IsMatch(type.TypeName))
                    type = Primitive.CreatePrimitive(typename, dt, isInstance);

            }
            else if (IsPrimitive(unqualifiedType)) // Primitive: Create the matching primitive type.
            {
                type = Primitive.CreatePrimitive(typename, dt, isInstance);
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
            if (String.Syntax.IsMatch(typename)) return true;
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

        #endregion
    }
}

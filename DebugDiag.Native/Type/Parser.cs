﻿using System;
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
        /// Regular Expression to match a typename and determine if it is a primitive type.
        /// </summary>
        public static Regex PrimitiveSyntax = new Regex(@"^(_LARGE_INTEGER$|U?[Ii]nt([248]B)?|U?[Cc]har)$");

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
        /// <param name="pattern">A compiled regular expression user to match against this type.</param>
        /// <param name="type"></param>
        public static void RegisterUserType(Regex pattern, System.Type type)
        {
            if (!type.IsSubclassOf(typeof(UserType))) throw new ArgumentException("Only user types can be registered.");

            Debug.Assert(pattern != null && type != null);
            RegisteredUserTypes[pattern] = type;
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
        /// </summary>
        /// <param name="dt">The `dt` output for this member</param>
        /// <param name="typename">The name of the type being parsed.</param>
        /// <param name="isInstance">Whether Parse is being called as part of type instanciation.</param>
        /// <returns></returns>
        internal static NativeType Parse(DumpType.Line? dt, string typename, bool isInstance)
        {
            var unqualifiedType = (typename.Contains("!")) ? typename.Split('!')[1] : typename; // Ignore module name.

            NativeType type = null;

            if (IsPrimitive(unqualifiedType)) // Primitive: Create the matching primitive type.
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
            // Real native types.
            if (PrimitiveSyntax.IsMatch(typename)) return true;

            // These types are treated as though they were native, and bypass the user type extension framework.
            if (Guid.Syntax.IsMatch(typename)) return true;
            if (Pointer.Syntax.IsMatch(typename)) return true;
            if (String.Syntax.IsMatch(typename)) return true;
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

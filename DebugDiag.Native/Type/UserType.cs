using System.Text.RegularExpressions;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Entry point for extended type support. Allows the type engine to be extended with type-specific functionality.
    /// 
    /// There are a few built-in user types that showcase how this works.
    /// </summary>
    /// <see cref="Vector"/>
    /// <see cref="Map"/>
    public abstract class UserType : NativeType
    {
        /// <summary>
        /// This method is invoked when the user type is created. It acts as the constructor for the user type.
        /// </summary>
        /// <param name="typename">The full typename that caused this user type to match.</param>
        /// <param name="match">The matches returned by the regular expression.</param>
        public abstract void OnCreateInstance(string typename, Match match);

        #region Constructor

        protected UserType(string typename)
            : base(typename)
        {
        }

        protected UserType(NativeType other)
            : base(other)
        {
        }

        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace DebugDiag.Native.Test.Fixtures
{
    /// <summary>
    /// Generates type fixtures for testing.
    /// 
    /// This type is enumerable and will return a finite list of fixtures depending on the object created.
    /// </summary>
    public abstract class Generator : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// The address to use for the location of this fixture.
        /// This address can overlap with other addresses since fixtures don't have an internal representation of core dumps.
        /// This is exposed internally in case a parent generator needs to override the address output.
        /// </summary>
        internal ulong Address { get; set; }

        #region API

        /// <summary>
        /// Returns the full type name of this fixture.
        /// </summary>
        /// <returns></returns>
        public abstract string GetTypeName();

        /// <summary>
        /// Generates the generic type information for this type.
        /// This method is called to generate the output to `dt 0 [typename]`
        /// </summary>
        /// <returns></returns>
        public abstract KeyValuePair<string, string> GetTypeInfo();

        /// <summary>
        /// Generates the next fixture for that type. This method must be overloaded by generators and must
        /// always succeed.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<KeyValuePair<string, string>> GenerateInternal();

        public IEnumerable<KeyValuePair<string, string>> Generate(bool generateTypeInfo = true)
        {
            foreach (var f in GenerateInternal())
                yield return f;
            
            if (generateTypeInfo) yield return GetTypeInfo(); // Last because some primitive types can set it to null.
        }

        #endregion
        #region Enumerator

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Generate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}

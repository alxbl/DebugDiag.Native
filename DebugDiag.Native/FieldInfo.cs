using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native
{
    /// <summary>
    /// Stores information about a type's field.
    /// </summary>
    public class FieldInfo
    {
        private static readonly FieldInfo Null = new FieldInfo("<invalid>", 0UL, "<nullptr>");
        private FieldInfo(string name, ulong offset, string type)
        {
            Name = name;
            Offset = offset;
            Type = type;
        }

        /// <summary>
        /// Parses a single field line.
        /// </summary>
        /// <param name="dbg"></param>
        /// <returns></returns>
        internal static FieldInfo ParseField(string dbg)
        {
            return Null;
        }

        #region Properties
        public string Name { get; private set; }
        public ulong Offset { get; private set; }
        public string Type { get; private set; }
        #endregion
    }
}

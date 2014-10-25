using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native
{
    /// <summary>
    /// Shows that this object supports deep copy.
    /// </summary>
    /// <typeparam name="T">The type of the object returned by DeepCopy</typeparam>
    public interface IDeepCopyable<out T>
    {
        T DeepCopy();
    }
}

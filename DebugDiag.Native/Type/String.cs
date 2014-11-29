using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a string primitive.
    /// 
    /// String primitives can be anything from an std::string, std::wstring to tchar* and wchar*.
    /// </summary>
    public sealed class String : Primitive
    {
        // Ptr Wchar
        // Ptr Char
        // std::string (?)
        private string _value;
    }
}

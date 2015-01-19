using System;
using System.Collections.Generic;

namespace DebugDiag.Native.Test.Fixtures.Generators
{
    public sealed class String : Generator
    {
        private readonly StringFormat _type;
        private readonly string _string;
        private readonly ulong _addr;

        public enum StringFormat
        {
            /// <summary>
            /// wchar_t *
            /// </summary>
            Wide,
            /// <summary>
            /// char *
            /// </summary>
            Narrow,
            /// <summary>
            /// std::wstring
            /// </summary>
            StlWide,
            /// <summary>
            /// std::string
            /// </summary>
            StlNarrow,
            /// <summary>
            /// wchar_t [n]
            /// </summary>
            ArrayWide,
            /// <summary>
            /// char [n]
            /// </summary>
            ArrayNarrow
        }

        public String(ulong addr, StringFormat type, string str)
        {
            _type = type;
            _string = str;
            _addr = addr;
        }

        public override string GetTypeName()
        {
            string str = "";
            switch (_type)
            {
                case StringFormat.Wide:
                    str = "Ptr32 Wchar";
                    break;
                case StringFormat.Narrow:
                    str = "Ptr32 Char";
                    break;
                case StringFormat.StlWide:
                    str = "std::basic_string<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t> >";
                    break;
                case StringFormat.StlNarrow:
                    str = "std::basic_string<char,std::char_traits<char>,std::allocator<char> >";
                    break;
                case StringFormat.ArrayWide:
                    str = string.Format("[{0}] Wchar", _string.Length+1); // +1 for null terminator
                    break;
                case StringFormat.ArrayNarrow:
                    str = string.Format("[{0}] Char", _string.Length+1); // +1 for null terminator
                    break;
            }
            return str;
        }

        public override KeyValuePair<string, string> GetTypeInfo()
        {
            if (_type == StringFormat.StlNarrow)
                return new KeyValuePair<string, string>(string.Format("dt 0 {0}", GetTypeName()),
@"    +0x000 _Myproxy         : Ptr32 std::_Container_proxy
   +0x004 _Bx              : std::_String_val<std::_Simple_types<char> >::_Bxty
   +0x014 _Mysize          : Uint4B
   +0x018 _Myres           : Uint4B
   =00c70000 npos             : Uint4B");

            if (_type == StringFormat.StlWide)
                return new KeyValuePair<string, string>(string.Format("dt 0 {0}", GetTypeName()),
@"    +0x000 _Myproxy         : Ptr32 std::_Container_proxy
   +0x004 _Bx              : std::_String_val<std::_Simple_types<wchar_t> >::_Bxty
   +0x014 _Mysize          : Uint4B
   +0x018 _Myres           : Uint4B
   =00c70000 npos             : Uint4B
");
            return new KeyValuePair<string, string>(null, null); // Others are primitive types, no dt possible.
        }

        public override IEnumerable<KeyValuePair<string, string>> GenerateInternal()
        {
            switch (_type)
            {
                case StringFormat.Wide:
                case StringFormat.ArrayWide:
                    yield return new KeyValuePair<string, string>(string.Format(".printf \"%mu\", 0x{0:x}", _addr), _string);
                    break;
                case StringFormat.Narrow:
                case StringFormat.ArrayNarrow:
                    yield return new KeyValuePair<string, string>(string.Format(".printf \"%ma\", 0x{0:x}", _addr), _string);
                    break;
                case StringFormat.StlWide:
                    // There is an optimization that does not require to dump _Bx by using poi().
                    foreach (var v in GenerateStlWide()) yield return v;
                    break;
                case StringFormat.StlNarrow:
                    foreach (var v in GenerateStlNarrow()) yield return v;
                    break;

            }
        }

        private IEnumerable<KeyValuePair<string, string>> GenerateStlWide()
        {
            // dt
            yield return new KeyValuePair<string, string>(string.Format("dt 0x{0:x} {1}", _addr, GetTypeName()),
                        string.Format(@"   +0x000 _Myproxy         : 0xbaadf00d std::_Container_proxy
   +0x004 _Bx              : std::_String_val<std::_Simple_types<wchar_t> >::_Bxty
   +0x014 _Mysize          : 0x{0:x}
   +0x018 _Myres           : 0x{0:x}
   =00c70000 npos             : 0x905a4d", _string.Length));

            // .printf
            yield return (_string.Length >= Type.String.WStringBufLen)
                ? new KeyValuePair<string, string>(string.Format(".printf \"%mu\", poi(0x{0:x})", _addr + 4), _string)
                : new KeyValuePair<string, string>(string.Format(".printf \"%mu\", 0x{0:x}", _addr + 4), _string);
        }

        private IEnumerable<KeyValuePair<string, string>> GenerateStlNarrow()
        {
            // dt
            yield return new KeyValuePair<string, string>(string.Format("dt 0x{0:x} {1}", _addr, GetTypeName()),
                        string.Format(@"   +0x000 _Myproxy         : 0xbaadf00d std::_Container_proxy
   +0x004 _Bx              : std::_String_val<std::_Simple_types<char> >::_Bxty
   +0x014 _Mysize          : 0x{0:x}
   +0x018 _Myres           : 0x{0:x}
   =00c70000 npos             : 0x905a4d", _string.Length));

            // .printf
            yield return (_string.Length >= Type.String.StringBufLen)
                ? new KeyValuePair<string, string>(string.Format(".printf \"%ma\", poi(0x{0:x})", _addr + 4), _string)
                : new KeyValuePair<string, string>(string.Format(".printf \"%ma\", 0x{0:x}", _addr + 4), _string);
        }
    }
}

using System.Text.RegularExpressions;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a string primitive.
    /// 
    /// String primitives can be anything from an std::string, std::wstring to char* and wchar_t*.
    /// </summary>
    public sealed class String : Primitive
    {
        public const int WStringBufLen = 8; // The length of the std::wstring built-in buffer.
        public const int StringBufLen = 16; // The length of the std::string built-in buffer.
        private readonly bool _isStl, _isWide;
        private string _cache;

        public static readonly Regex Syntax = new Regex(@"^((\[\d+\]|Ptr32|Ptr64) (Wchar|Char))|std::basic_string<.*>|Wchar \*|Char \*$");

        #region Type Implementation

        public int Length
        {
            get
            {
                if (string.IsNullOrEmpty(_cache)) GetStringValue();
                return _cache.Length;
            }
        }

        public override string GetStringValue()
        {
            if (string.IsNullOrEmpty(_cache))
            {
                DumpString ds = null;
                if (_isStl)
                {
                    // This next part warrants a bit of explanation: _Bx is a union of a buffer and pointer.
                    // If we determined that the string is too big to be stored in the local buffer, then we need to
                    // dump out the address that _Bx is pointing to. This is done by getting the raw memory at _Bx.
                    var len = GetIntValue("_Mysize");
                    // Check if we need to use the pointer or the built-in buffer. Remove one from the buffer length to account for null terminator.
                    bool isPtr = _isWide ? len > WStringBufLen - 1 : len > StringBufLen - 1;

                    // _Bx resides at offset 4 of the string object. If we're using a pointer, use poi(_Bx).
                    ds = new DumpString(Address + 4, _isWide, isPtr);
                }
                else
                {
                    // When we are looking at a real string primitive, we already have the address of the string.
                    ds = new DumpString(Address, _isWide, false);
                }

                // Do we want to do some special handling of the returned value? That should probably be done in DumpString.Parse()
                _cache = ds.Output;
            }
            return _cache;
        }

        protected override void BuildOffsetTable(string type)
        {
            if (_isStl) // This is an STL object, we need to inspect it.
                base.BuildOffsetTable(type);
            // Otherwise, this is a string primitive.
        }

        protected override void Rebase()
        {
            if (_isStl) // This is an STL object, we need to inspect it.
                base.Rebase();
            // Otherwise, this is a string primitive.
        }

        #endregion

        #region Constructor
        protected override NativeInstance DeepCopy()
        {
            return new String(this);
        }

        private String(String other)
            : base(other)
        {
            _isStl = other._isStl;
            _isWide = other._isWide;
            _cache = other._cache;
        }
        
        public String(string typename, ulong value)
            : base(typename, value)
        {
            _isStl = typename.Contains("std::basic_string");
            _isWide = typename.Contains("wchar") || typename.Contains("Wchar");
        }
        #endregion

        //private string _value;
    }
}

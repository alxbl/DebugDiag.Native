namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a string primitive.
    /// 
    /// String primitives can be anything from an std::string, std::wstring to char* and wchar_t*.
    /// </summary>
    public sealed class String : Primitive
    {
        public String(string typename, ulong value)
            : base(typename, value)
        {

        }

        //private string _value;
    }
}

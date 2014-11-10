namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Analog representation of an std::pair.
    /// </summary>
    public sealed class Pair : NativeType
    {
        public NativeType First { get; internal set; }
        public NativeType Second { get; internal set; }
    }
}

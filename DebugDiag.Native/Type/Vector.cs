using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Type
{
    class Vector : Enumerable
    {
        // Keep a cached copy of the instances to avoid constantly querying the dump file.
        private List<NativeType> _impl = null;

        public Vector(ulong size, string type) : base(size, type)
        {
        }

        public override IEnumerator<NativeType> GetEnumerator()
        {
            if (_impl != null) return _impl.GetEnumerator();
            // Build the list of objects.
            _impl = new List<NativeType>();
            return _impl.GetEnumerator(); // TODO: Need to iterate the type.
        }
    }
}

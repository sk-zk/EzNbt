using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EzNbt
{
    /// <summary>
    /// An NBT tag.
    /// </summary>
    [DebuggerDisplay("{Name}: {Data}")]
    public struct NbtTag
    {
        public string Name;
        public dynamic Data;

        public NbtTag(string name, dynamic data)
        {
            Name = name;
            Data = data;
        }
    }
}

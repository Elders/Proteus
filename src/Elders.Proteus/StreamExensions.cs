using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus
{
    public static class StreamExensions
    {
        public static void WriteAllBytes(this Stream str, byte[] bytes)
        {
            str.Write(bytes, 0, bytes.Length);
        }
    }
}

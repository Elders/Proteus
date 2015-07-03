using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Elders.Proteus
{
    internal static class ProteusRuntimeTypeModelHeader
    {
        internal static byte[] Header = new Guid("0b097880-00b7-449c-b487-4c7ed365924c").ToByteArray();

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        /// <summary>
        /// Tries to read a header from a stream.If the stream does not contains the header, the stream position is returned.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool TryReadHeader(Stream stream)
        {
            byte[] readBytes = new byte[Header.Length];
            stream.Read(readBytes, 0, Header.Length);
            bool hasHeader = readBytes.Length == Header.Length && memcmp(Header, readBytes, Header.Length) == 0;
            if (hasHeader == false)
                stream.Position = stream.Position - Header.Length;
            return hasHeader;
        }

    }
}

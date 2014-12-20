using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf.Meta;

namespace Elders.Proteus
{
    public class ProteusRuntimeTypeModel
    {
        const int SizeOfInt = 4;
        private static int SizeOfGuid = 16;
        private readonly ITypeIdentifier identifier;

        public ProteusRuntimeTypeModel(ITypeIdentifier identifier)
        {
            this.identifier = identifier;
        }

        public Type ReadType(Stream stream)
        {
            int idLenght;
            if (!ProteusRuntimeTypeModelHeader.TryReadHeader(stream))
            {
                throw new InvalidOperationException("Can not find input stream header");
            }
            else
            {
                if (identifier.IsDynamicLenght)
                {
                    byte[] typeIdLenght = new byte[SizeOfInt];
                    stream.Read(typeIdLenght, 0, SizeOfInt);
                    //if (BitConverter.IsLittleEndian) WHY??? IT DOES NOT WORK LIKE THIS
                    //    Array.Reverse(typeIdLenght); WHY??? IT DOES NOT WORK LIKE THIS
                    //System.Diagnostics.Trace.WriteLine("WARNING::CHECK FOR LITTLE ENDIAN");
                    idLenght = BitConverter.ToInt32(typeIdLenght, 0);
                }
                else
                {
                    stream.Position = stream.Position + SizeOfInt;
                    idLenght = identifier.Lenght;
                }
            }
            byte[] id = new byte[idLenght];
            stream.Read(id, 0, idLenght);
            return identifier.GetTypeById(id);

        }

        public void WriteType(Stream stream, Type type)
        {
            stream.WriteAllBytes(ProteusRuntimeTypeModelHeader.Header);
            var id = identifier.GetTypeId(type);
            var idLenght = BitConverter.GetBytes(id.Length);
            stream.WriteAllBytes(idLenght);
            stream.WriteAllBytes(id);
        }
    }
}

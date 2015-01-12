using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus.Conversion
{
    [DataContract(Name = "a8abe115-95c9-4ec6-83b0-01af6c762b17")]
    public class RuntimeSurrogate<T, V>
    {
        public RuntimeSurrogate()
        {

        }
        public RuntimeSurrogate(object value)
        {
            Id = Identifier.GetTypeId(value.GetType());
            var str = new MemoryStream();
            Model.Serialize(str, value);
            Wraper = str.ToArray();

        }

        [DataMember(Order = 1)]
        public byte[] Wraper { get; set; }

        [DataMember(Order = 2)]
        public byte[] Id { get; set; }

        public static implicit operator T(RuntimeSurrogate<T, V> value)
        {
            return (T)Model.Deserialize(new MemoryStream(value.Wraper), null, Identifier.GetTypeById(value.Id));
        }

        public static implicit operator RuntimeSurrogate<T, V>(T value)
        {
            return value == null ? null : new RuntimeSurrogate<T, V>(value);
        }

        public static ITypeIdentifier Identifier;

        public static ProtoBuf.Meta.RuntimeTypeModel Model;
    }

}

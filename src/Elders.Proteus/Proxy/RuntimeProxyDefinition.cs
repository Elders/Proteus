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
    public class RuntimeProxy<T, V>
    {
        static string ProxyName = typeof(V).Name;
        
        [DataMember(Order = 1)]
        public byte[] Wraper { get; set; }

        public static implicit operator T(RuntimeProxy<T, V> value)
        {
            var str = new MemoryStream();
            var restored = (T)Serializer.DeserializeWithHeaders(new MemoryStream(value.Wraper));
            
            return restored;
        }

        public static implicit operator RuntimeProxy<T, V>(T value)
        {
            if (value != null)
            {
                var str = new MemoryStream();
                Serializer.SerializeWithHeaders(str, value);
                str.Position = 0;

                return new RuntimeProxy<T, V>() { Wraper = str.ToArray() };
            }
            else return null;
            
        }

        public static Serializer Serializer;
    }
}

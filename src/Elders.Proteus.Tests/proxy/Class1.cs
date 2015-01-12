using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Elders.Proteus.Tests.proxy
{
    [DataContract(Name = "cb14a430-4669-497e-a037-8e5be206679a")]
    public class BaseClass
    {
        public BaseClass()
        {

        }
        [DataMember(Order = 1)]
        private string BaseProperty { get; set; }
        public void SetStuff()
        {
            BaseProperty = "base value";
        }
    }
    [DataContract(Name = "a694dac8-0c1f-4cfe-a8a8-a5381c2ac44e")]
    public class ToProxy : BaseClass
    {
        [DataMember(Order = 2)]
        public string TestProperty { get; set; }
    }
    [Subject(typeof(Serializer))]
    public class When_serializng_simple_type_with_headers
    {
        Establish context = () =>
        {
            ser = new ToProxy() { TestProperty = "aaaa" };
            ser.SetStuff();
            serializer = new Serializer();
            serStream = new MemoryStream();
            serializer.SerializeWithHeaders(serStream, ser);
            serStream.Position = 0;
        };
        Because of_deserialization = () => { deser = (ToProxy)serializer.DeserializeWithHeaders(serStream); };


        It should_not_be_null = () => deser.ShouldNotBeNull();


        static ToProxy ser;
        static ToProxy deser;
        static Stream serStream;
        static Serializer serializer;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Elders.Proteus.Tests
{
    [Subject("protobuf-net")]
    public class Whem_serializing_simple_type
    {
        Establish context = () =>
        {
            ser = new SimpleType() { Int = 5, Date = DateTime.UtcNow.AddDays(1), String = "a" };
            serializer = new Serializer();
            serStream = new MemoryStream();
            serializer.Serialize(serStream, ser);
            serStream.Position = 0;
        };
        Because of_deserialization = () => { deser = serializer.Deserialize<SimpleType>(serStream); };


        It should_not_be_null = () => deser.ShouldNotBeNull();
        It should_have_the_same_int = () => deser.Int.ShouldEqual(ser.Int);
        It should_have_the_same_string = () => deser.String.ShouldEqual(ser.String);
        It should_have_the_same_date = () => deser.Date.ShouldEqual(ser.Date);
        It should_have_the_same_date_kind = () => deser.Date.Kind.ShouldEqual(ser.Date.Kind);
        It should_have_the_same_date_as_utc = () => deser.Date.ToFileTimeUtc().ShouldEqual(ser.Date.ToFileTimeUtc());

        static SimpleType ser;
        static SimpleType deser;
        static Stream serStream;
        static Serializer serializer;
    }
}
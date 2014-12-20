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
    [Subject(typeof(Serializer))]
    public class Whem_serializing_nested_type_undefinded_interface_inheritance
    {

        Establish context = () =>
        {
            ser = new NestedTypeWithUndefinedInterfaceInheritance() { Int = 5, Date = DateTime.UtcNow.AddDays(1), String = "a", Nested = new UndefinedInterfaceInheritance() { Int = 4, Date = DateTime.UtcNow.AddDays(2), String = "b" } };
            serializer = new Serializer();
            serializer2 = new Serializer();
            serStream = new MemoryStream();
            serializer.SerializeWithHeaders(serStream, ser);
            serStream.Position = 0;
        };
        Because of_deserialization = () => { deser = (NestedTypeWithUndefinedInterfaceInheritance)serializer2.DeserializeWithHeaders(serStream); };


        It should_not_be_null = () => deser.ShouldNotBeNull();
        It should_have_the_same_int = () => deser.Int.ShouldEqual(ser.Int);
        It should_have_the_same_string = () => deser.String.ShouldEqual(ser.String);
        It should_have_the_same_date = () => deser.Date.ShouldEqual(ser.Date);
        It should_have_the_same_date_as_utc = () => deser.Date.ToFileTimeUtc().ShouldEqual(ser.Date.ToFileTimeUtc());

        It nested_object_should_not_be_null = () => deser.Nested.ShouldNotBeNull();
        It nested_object_should_be_of_the_right_type = () => deser.Nested.ShouldBeOfExactType(typeof(UndefinedInterfaceInheritance));



        static NestedTypeWithUndefinedInterfaceInheritance ser;
        static NestedTypeWithUndefinedInterfaceInheritance deser;
        static Stream serStream;
        static Serializer serializer;
        static Serializer serializer2;
    }
}
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
    public class Whem_serializing_nested_type_undefinded_baseclass_inheritance
    {

        Establish context = () =>
        {
            ser = new NestedTypeWithBaseClassInheritance() { Int = 5, Date = DateTime.UtcNow.AddDays(1), String = "a", Nested = new UndefinedBaseclassInheritance() { Int = 4, Date = DateTime.UtcNow.AddDays(2), String = "b" } };
            ser.Nested.CustomBaseClassString = "Custom string";
            ser.Nested.CustomBaseBaseClassString = "UHAAA";
            serializer = new Serializer(new GuidTypeIdentifier(typeof(NestedType).Assembly));
            serializer2 = new Serializer(new GuidTypeIdentifier(typeof(NestedType).Assembly));
            serStream = new MemoryStream();
            serializer.SerializeWithHeaders(serStream, ser);
            serStream.Position = 0;
        };
        Because of_deserialization = () => { deser = (NestedTypeWithBaseClassInheritance)serializer2.DeserializeWithHeaders(serStream); };


        It should_not_be_null = () => deser.ShouldNotBeNull();
        It should_have_the_same_int = () => deser.Int.ShouldEqual(ser.Int);
        It should_have_the_same_string = () => deser.String.ShouldEqual(ser.String);
        It should_have_the_same_date = () => deser.Date.ShouldEqual(ser.Date);
        It should_have_the_same_date_as_utc = () => deser.Date.ToFileTimeUtc().ShouldEqual(ser.Date.ToFileTimeUtc());

        It nested_object_should_not_be_null = () => deser.Nested.ShouldNotBeNull();
        It nested_object_should_be_of_the_right_type = () => deser.Nested.ShouldBeOfExactType(typeof(UndefinedBaseclassInheritance));
        It nested_objects_should_contain_base_properties = () => ser.Nested.CustomBaseClassString.ShouldEqual(deser.Nested.CustomBaseClassString);
        It nested_objects_should_contain_bases_base_properties = () => ser.Nested.CustomBaseBaseClassString.ShouldEqual(deser.Nested.CustomBaseBaseClassString);



        static NestedTypeWithBaseClassInheritance ser;
        static NestedTypeWithBaseClassInheritance deser;
        static Stream serStream;
        static Serializer serializer;
        static Serializer serializer2;
    }
}
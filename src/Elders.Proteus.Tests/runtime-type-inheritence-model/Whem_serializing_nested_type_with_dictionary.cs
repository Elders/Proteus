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
    public class Whem_serializing_nested_type_with_dictionary
    {

        Establish context = () =>
        {
            ser = new NestedTypeWithDctionaryInheritance() { Int = 5, Date = DateTime.UtcNow.AddDays(1), String = "a", Nested = new Dictionary<object, object>() };
            var key = new UndefinedDictionaryInheritance() { String = "key", Nested = new UndefinedDictionaryInheritance() { String = "Nested key" } };
            var value = new UndefinedDictionaryInheritance() { String = "value", Nested = new UndefinedDictionaryInheritance() { String = "Nested value" } };
            ser.Nested.Add(key, value);

            serializer = new Serializer();
            serializer2 = new Serializer();
            serStream = new MemoryStream();
            serializer.SerializeWithHeaders(serStream, ser);
            serStream.Position = 0;
        };
        Because of_deserialization = () => { deser = (NestedTypeWithDctionaryInheritance)serializer2.DeserializeWithHeaders(serStream); };


        It should_not_be_null = () => deser.ShouldNotBeNull();
        It should_have_the_same_int = () => deser.Int.ShouldEqual(ser.Int);
        It should_have_the_same_string = () => deser.String.ShouldEqual(ser.String);
        It should_have_the_same_date = () => deser.Date.ShouldEqual(ser.Date);
        It should_have_the_same_date_as_utc = () => deser.Date.ToFileTimeUtc().ShouldEqual(ser.Date.ToFileTimeUtc());

        It nested_object_should_not_be_null = () => deser.Nested.ShouldNotBeNull();
        It nested_object_should_be_of_the_right_type = () => deser.Nested.ShouldBeOfExactType(typeof(Dictionary<object, object>));
        It nested_object_key_should_be_of_the_right_type = () => deser.Nested.First().Key.ShouldBeOfExactType(typeof(UndefinedDictionaryInheritance));
        It nested_object_value_should_be_of_the_right_type = () => deser.Nested.First().Value.ShouldBeOfExactType(typeof(UndefinedDictionaryInheritance));


        static NestedTypeWithDctionaryInheritance ser;
        static NestedTypeWithDctionaryInheritance deser;
        static Stream serStream;
        static Serializer serializer;
        static Serializer serializer2;
    }
}
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
            serializer = new Serializer();
            serializer2 = new Serializer();
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

    [Subject(typeof(Serializer))]
    public class mynkow
    {
        //  Prigotvi si KBD, da na ostanesh iznenadan utre pak :) tuk? y
        //  Sega, minavam nabarzo da ti pokaza kakvo sam otkril
        //  1 log files, pri pravene na serializer puskam array ot assembly i garmi che ne moze da serializira Transport Message
        //  zashtoto nikoi ne mu puska ass na cronus, za tova razkarah assembly[] i polzvam empty ctor
        //  togava mi gramna RegisterAccount pri serializaciq, napravih copy/paste tuk da vidq dali shte mine
        //  tuk ne garmi obache ne moze da deserializira accountId
        //chakai, pokazvam log i te ostavqm da razgledash
        //{00000000-0000-0000-0000-000000000000} tova vika6 4e e problema nali?
        //  ne, parviq problem e che garmi i ne znam zashto, tova e vtoriq record v log-a
        //  a zashto tova e guid empty i dali shte opravi gorniq red ne znam.
        //problemite v loga sa ot tova 4e ne se vzimat pravilnite assemblita pri startutp
        //o4akvam 4e tam vzimam vsi4ki assemblita koito sa reference-nati v asselmblito koeto vika ctor-a
        //tova vazi li i za empty ctor da t.e. vzimat se vsi4ki assemblyta koito sa reference v Cronus men pove4e me pritesnqva Guid.Empty
        //sec

        Establish context = () =>

        {
            ser = new RegisterAccount(new AccountId(Guid.NewGuid()), "user", "pass", new Email("mynkow@gmail.com"));
            serializer = new Serializer(new GuidTypeIdentifier(typeof(mynkow).Assembly, typeof(Serializer).Assembly, typeof(AccountId).Assembly, typeof(Elders.Cronus.DomainModeling.GuidId).Assembly));
            serializer2 = new Serializer(new GuidTypeIdentifier(typeof(mynkow).Assembly, typeof(Serializer).Assembly, typeof(AccountId).Assembly, typeof(Elders.Cronus.DomainModeling.GuidId).Assembly));
            serStream = new MemoryStream();
            serializer.SerializeWithHeaders(serStream, ser);
            serStream.Position = 0;
        };
        Because of_deserialization = () => { deser = (RegisterAccount)serializer2.DeserializeWithHeaders(serStream); };


        It should_not_be_null = () => deser.ShouldNotBeNull();
        It should_have_the_same_int = () => deser.Email.ShouldEqual(ser.Email);

        static RegisterAccount ser;
        static RegisterAccount deser;
        static Stream serStream;
        static Serializer serializer;
        static Serializer serializer2;
    }
}
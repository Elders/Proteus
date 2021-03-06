﻿using System;
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
    public class Whem_serializing_nested_type_with_non_datacontract_types
    {

        Establish context = () =>
        {
            ser = new NestedTypeWithNonDataContractTypes() { Int = 5, Date = DateTime.UtcNow.AddDays(1), String = "a", Nested = "test" };

            serializer = new Serializer(new GuidTypeIdentifier(typeof(NestedType).Assembly));
            serializer2 = new Serializer(new GuidTypeIdentifier(typeof(NestedType).Assembly));
            serStream = new MemoryStream();
            //serializer.SerializeWithHeaders(serStream, ser);
            serStream.Position = 0;
        };
        Because of_deserialization = () => { ex = Catch.Exception(() => serializer.SerializeWithHeaders(serStream, ser)); };

#if DEBUG
        It invalid_operation_exception_should_be_thrown = () => ex.ShouldBeOfExactType(typeof(System.Reflection.TargetInvocationException));
#elif !DEBUG  
        It invalid_operation_exception_should_be_thrown = () => ex.ShouldBeOfExactType(typeof(InvalidOperationException));
#endif


        static Exception ex;
        static NestedTypeWithNonDataContractTypes ser;
        static NestedTypeWithNonDataContractTypes deser;
        static Stream serStream;
        static Serializer serializer;
        static Serializer serializer2;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Elders.Protoreg;
using ProtoBuf.Meta;

namespace Elders.Proteus.Performance
{
    [DataContract(Name = "099c2ee0-514f-4b73-b495-01341e3bef0f")]
    public class SimpleObject
    {
        [DataMember(Order = 1)]
        public string StringProp { get; set; }

        [DataMember(Order = 2)]
        public int IntProp { get; set; }
    }

    [DataContract(Name = "13184f27-ff1d-4085-a9a4-6356f808947a")]
    public class ComplexObjectGraph
    {
        [DataMember(Order = 1)]
        public string StringProp { get; set; }

        [DataMember(Order = 2)]
        public int IntProp { get; set; }

        [DataMember(Order = 3)]
        public object Nested { get; set; }
    }
    class Program
    {
        private static ProtoregSerializer protoreg;
        private static Serializer stringProteus = new Serializer(new DynamicStringTypeIdentifier(typeof(Program).Assembly));
        private static Serializer guidProteus = new Serializer();

        static void Main(string[] args)
        {
            var protoRegistration = new Protoreg.ProtoRegistration();
            protoRegistration.RegisterAssembly(typeof(SimpleObject));
            protoreg = new Protoreg.ProtoregSerializer(protoRegistration);
            protoreg.Build();
            var simpleObject = new SimpleObject() { IntProp = 1000, StringProp = "Test string" };
            var complex = new ComplexObjectGraph() { IntProp = 1001, StringProp = "test", Nested = simpleObject };
            //MeasureDeserialization("Deserialization Simple Object 1000000 times ", simpleObject, 1000000);
            //MeasureSerialization("Serializing Simple Object 1000000 times ", simpleObject, 1000000);
            RuntimeTypeModel.Default.Add(typeof(object), true).AddSubType(500, typeof(SimpleObject));
            MeasureDeserialization("Deserialization Complex Object 1000000 times ", complex, 100000);
            MeasureSerialization("Serializing Complex Object 1000000 times ", complex, 100000);

            Console.ReadLine();
        }

        public static void MeasureSerialization<T>(string header, T instance, int numberofTimes)
        {
            Console.WriteLine("============{0}============", header);
            guidProteus.SerializeWithHeaders(new MemoryStream(), instance);
            var proteusResult = MeasureExecutionTime.Start(() => guidProteus.SerializeWithHeaders(new MemoryStream(), instance), numberofTimes);
            Console.WriteLine("Guid Proteus: " + proteusResult);

            //stringProteus.SerializeWithHeaders(new MemoryStream(), instance);
            //var sproteusResult = MeasureExecutionTime.Start(() => stringProteus.SerializeWithHeaders(new MemoryStream(), instance), numberofTimes);
            //Console.WriteLine("String Proteus: " + sproteusResult);

            protoreg.Serialize(new MemoryStream(), instance);
            var protoregResult = MeasureExecutionTime.Start(() => protoreg.Serialize(new MemoryStream(), instance), numberofTimes);
            Console.WriteLine("Protoreg: " + protoregResult);
            ProtoBuf.Serializer.Serialize(new MemoryStream(), instance);
            var protobuffResult = MeasureExecutionTime.Start(() => ProtoBuf.Serializer.Serialize(new MemoryStream(), instance), numberofTimes);
            Console.WriteLine("Protobuff" + protobuffResult);

        }
        public static void MeasureDeserialization<T>(string header, T instance, int numberofTimes)
        {
            Console.WriteLine("============{0}============", header);


            var proteusStream = new MemoryStream();
            guidProteus.SerializeWithHeaders(proteusStream, instance);
            proteusStream.Position = 0;
            var proteusResult = MeasureExecutionTime.Start(() =>
            {
                proteusStream.Position = 0;
                var des = guidProteus.DeserializeWithHeaders(proteusStream);

            }, numberofTimes);
            Console.WriteLine("Guid Proteus: " + proteusResult);

            //var sproteusStream = new MemoryStream();
            //stringProteus.SerializeWithHeaders(sproteusStream, instance);
            //sproteusStream.Position = 0;
            //var sproteusResult = MeasureExecutionTime.Start(() =>
            //{
            //    sproteusStream.Position = 0;
            //    var des = stringProteus.DeserializeWithHeaders(sproteusStream);

            //}, numberofTimes);
            //Console.WriteLine("String Proteus: " + sproteusResult);

            var protoregStream = new MemoryStream();
            protoreg.Serialize(protoregStream, instance);
            var protoregResult = MeasureExecutionTime.Start(() =>
                {
                    protoregStream.Position = 0;
                    var des = protoreg.Deserialize(protoregStream);
                }, numberofTimes);
            Console.WriteLine("Protoreg: " + protoregResult);

            var protobuffStream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(protobuffStream, instance);
            var protobuffResult = MeasureExecutionTime.Start(() =>
                {
                    protobuffStream.Position = 0;
                    var des = ProtoBuf.Serializer.Deserialize<T>(protobuffStream);
                }, numberofTimes);
            Console.WriteLine("Protobuff" + protobuffResult);

        }
    }
}

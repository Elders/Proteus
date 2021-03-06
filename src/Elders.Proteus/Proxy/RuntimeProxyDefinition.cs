﻿using System;
using System.IO;
using System.Runtime.Serialization;

namespace Elders.Proteus.Conversion
{
    [DataContract(Name = "a8abe115-95c9-4ec6-83b0-01af6c762b17")]
    public class RuntimeProxy<T, V>
    {
        public RuntimeProxy()
        {

        }

        public RuntimeProxy(object value)
        {
            Id = new Guid(Identifier.GetTypeId(value.GetType()));
            var str = new MemoryStream();
            Model.Serialize(str, value);
            Wraper = str.ToArray();
        }

        [DataMember(Order = 3)]
        public byte[] Wraper { get; set; }

        [DataMember(Order = 2)]
        public Guid Id { get; set; }

        public static implicit operator T(RuntimeProxy<T, V> value)
        {
            return (value == null || value.Wraper == null || value.Wraper.Length == 0)
                ? default(T)
                : (T)Model.Deserialize(new MemoryStream(value.Wraper), null, Identifier.GetTypeById(value.Id.ToByteArray()));
        }

        public static implicit operator RuntimeProxy<T, V>(T value)
        {
            return value == null ? null : new RuntimeProxy<T, V>(value);
        }

        public static ITypeIdentifier Identifier;

        public static ProtoBuf.Meta.RuntimeTypeModel Model;
    }

    //[DataContract(Name = "a8abe115-95c9-4ec6-83b0-01af6c762b17")]
    //public class RuntimeProxy<T, V>
    //{
    //    static string ProxyName = typeof(V).Name;

    //    [DataMember(Order = 1)]
    //    public byte[] Wraper { get; set; }

    //    public static implicit operator T(RuntimeProxy<T, V> value)
    //    {
    //        var str = new MemoryStream();
    //        var restored = (T)Serializer.DeserializeWithHeaders(new MemoryStream(value.Wraper));

    //        return restored;
    //    }

    //    public static implicit operator RuntimeProxy<T, V>(T value)
    //    {
    //        if (value != null)
    //        {
    //            var str = new MemoryStream();
    //            Serializer.SerializeWithHeaders(str, value);
    //            str.Position = 0;

    //            return new RuntimeProxy<T, V>() { Wraper = str.ToArray() };
    //        }
    //        else return null;

    //    }

    //    public static Serializer Serializer;
    //}
}

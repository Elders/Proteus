using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ProtoBuf.Meta;
using System.Runtime.Serialization;
using Elders.Proteus.Conversion;
using System.Diagnostics;
using Elders.Proteus.Proxy;
namespace Elders.Proteus
{
    
    public class Serializer
    {
        private readonly ITypeIdentifier identifier;
        RuntimeTypeModel protobufTypeModel;
        ProteusRuntimeTypeModel proteusTypeModel;

        public Guid Id { get; private set; }
        public Serializer()
            : this(new GuidTypeIdentifier(Assembly.GetCallingAssembly()))
        {

        }

        public Serializer(ITypeIdentifier identifier)
        {
            Id = Guid.NewGuid();
            proteusTypeModel = new ProteusRuntimeTypeModel(identifier);
            this.identifier = identifier;
            protobufTypeModel = ModelBuilder.New();
            List<Type> builtProxies = new List<Type>();
            var factory = new ProxyFactory(this.identifier, Id);
            foreach (var item in identifier.GetAvailableTypesAndTheirSerializableParents())
            {
                if (item.IsGenericTypeDefinition)
                    continue;
                var proxy = factory.GetProxy(item);
                protobufTypeModel.Add(item, false).SetSurrogate(proxy);
                builtProxies.Add(proxy);

            }

           // builtProxies.Add(px);
        }

        public T Deserialize<T>(Stream source)
        {
            return (T)protobufTypeModel.Deserialize(source, null, typeof(T));
        }

        public void Serialize<T>(Stream destination, T instance)
        {
            if (instance != null)
            {
                protobufTypeModel.Serialize(destination, instance);
            }
        }

        public void SerializeWithHeaders(Stream destination, object instance)
        {
            if (instance != null)
            {
                proteusTypeModel.WriteType(destination, instance.GetType());
                protobufTypeModel.Serialize(destination, instance);
            }
        }

        public object DeserializeWithHeaders(Stream source)
        {
            var type = proteusTypeModel.ReadType(source);
            return protobufTypeModel.Deserialize(source, null, type);
        }
        


    }
}

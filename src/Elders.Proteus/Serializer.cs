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
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            CultureInfo culture = CultureInfo.InvariantCulture; // use InvariantCulture or other if you prefer
            protobufTypeModel = (RuntimeTypeModel)
              Activator.CreateInstance(typeof(RuntimeTypeModel), flags, null, new object[] { true }, culture);
            protobufTypeModel.AutoAddMissingTypes = true;
            
            List<Type> builtProxies = new List<Type>();
            foreach (var item in identifier.AvailableTypes.ToList())
            {
                var metaType = protobufTypeModel.Add(item, true);
                var fields = metaType.GetFields();
                IncludeParents(metaType);
                foreach (var field in fields)
                {
                    if (!builtProxies.Contains(field.MemberType))
                    {
                        bool shouldBuildProxy = identifier.AvailableTypes.Any(x => field.MemberType.IsAssignableFrom(x) && x != field.MemberType);
                        if (shouldBuildProxy)
                        {
                            var proxy = RuntimeProxyBuilder.BuildDynamicProxy(field.MemberType, this.Id, this);
                            protobufTypeModel.Add(field.MemberType, false).SetSurrogate(proxy);
                            builtProxies.Add(field.MemberType);
                        }

                    }
                }
            }
        }
        private void IncludeParents(MetaType type)
        {
            if (type.Type.BaseType == typeof(object))
                return;
            var metaBase = protobufTypeModel.Add(type.Type.BaseType, true);
            IncludeParents(metaBase);
            if (identifier.AvailableTypes.Contains(type.Type.BaseType))
            {
                foreach (var field in metaBase.GetFields())
                {
                    if (!type.GetFields().Select(x => x.FieldNumber).Contains(field.FieldNumber))
                        type.AddField(field.FieldNumber, field.Member.Name);
                }
            }
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

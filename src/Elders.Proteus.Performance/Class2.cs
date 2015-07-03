using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Elders.Cronus.DomainModeling;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Elders.Proteus.Performance
{
    internal class Json
    {
        JsonSerializerSettings settings;

        public Json(IContractsRepository contractRepository)
        {
            settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.ContractResolver = new DataMemberContractResolver();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            settings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            settings.Formatting = Formatting.Indented;
            settings.Binder = new TypeNameSerializationBinder(contractRepository);
            settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }

        public object Deserialize(string str)
        {
            return JsonConvert.DeserializeObject(str, settings);
        }

        public T Deserialize<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str, settings);
        }

        class DataMemberContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty customMember = base.CreateProperty(member, memberSerialization);
                if (member.HasAttribute<DataMemberAttribute>())
                    customMember.PropertyName = customMember.Order.ToString();

                return customMember;
            }
        }

        class TypeNameSerializationBinder : SerializationBinder
        {
            private readonly IContractsRepository contractRepository;

            public TypeNameSerializationBinder(IContractsRepository contractRepository)
            {
                this.contractRepository = contractRepository;
            }

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                string name;
                if (contractRepository.TryGet(serializedType, out name))
                {
                    assemblyName = null;
                    typeName = name;
                }
                else
                {
                    assemblyName = serializedType.Assembly.FullName;
                    typeName = serializedType.FullName;
                }
            }

            public override Type BindToType(string assemblyName, string typeName)
            {
                if (assemblyName == null)
                {
                    Type type;
                    if (contractRepository.TryGet(typeName, out type))
                        return type;
                }
                return Type.GetType(string.Format("{0}, {1}", typeName, assemblyName), true);
            }
        }
    }

    public interface IContractsRepository
    {
        IEnumerable<Type> Contracts { get; }

        bool TryGet(Type type, out string name);
        bool TryGet(string name, out Type type);
    }

    public class ContractsRepository : IContractsRepository
    {
        readonly Dictionary<Type, string> typeToName = new Dictionary<Type, string>();
        readonly Dictionary<string, Type> nameToType = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public ContractsRepository(IEnumerable<Type> contracts)
        {
            if (contracts != null)
            {
                foreach (var contract in contracts)
                {
                    //if (contract.HasAttribute<DataContractAttribute>())
                    Map(contract, contract.GetAttrubuteValue<DataContractAttribute, string>(x => x.Name));
                }
            }
        }

        public bool TryGet(Type type, out string name)
        {
            return typeToName.TryGetValue(type, out name);
        }

        public bool TryGet(string name, out Type type)
        {
            return nameToType.TryGetValue(name, out type);
        }

        public IEnumerable<Type> Contracts { get { return typeToName.Keys.ToList().AsReadOnly(); } }

        private void Map(Type type, string name)
        {
            typeToName.Add(type, name);
            nameToType.Add(name, type);
        }
    }
}

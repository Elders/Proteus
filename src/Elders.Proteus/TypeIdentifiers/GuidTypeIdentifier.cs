using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus
{
    public class GuidTypeIdentifier : ITypeIdentifier
    {
        HashSet<Assembly> assembliesWithDataContracts = new HashSet<Assembly>();
        private Dictionary<Guid, Type> types = new Dictionary<Guid, Type>();
        private Dictionary<Type, Guid> ids = new Dictionary<Type, Guid>();
        public IEnumerable<Type> AvailableTypes { get { return types.Select(x => x.Value); } }

        public GuidTypeIdentifier(params Assembly[] assemblies)
        {
            foreach (var hostAssembly in assemblies)
            {
                var refercendAssemblies = hostAssembly.GetReferencedAssemblies();
                assembliesWithDataContracts.Add(hostAssembly);
                foreach (var item in refercendAssemblies)
                {
                    assembliesWithDataContracts.Add(Assembly.Load(item));
                }
            }
            foreach (var item in assembliesWithDataContracts)
            {
                DiscoverDataContracts(item);
            }

        }

        public void DiscoverDataContracts(Assembly assembly)
        {
            var unknownTypes = assembly.GetTypes().Where(x => x.GetCustomAttributes(false).Any(y => y is DataContractAttribute));
            foreach (var item in unknownTypes)
            {
                var attribute = item.GetCustomAttributes(false).Where(x => x is DataContractAttribute).SingleOrDefault() as DataContractAttribute;
                if (String.IsNullOrEmpty(attribute.Name))
                {
                    System.Diagnostics.Trace.WriteLine(String.Format("WARNING::{0} has DataContractAttriubte, but it does not have Name.It won't be registered for serialization", item.FullName));
                    continue;
                }
                Guid gd;
                if (!Guid.TryParse(attribute.Name, out gd))
                    continue;
                if (types.ContainsKey(gd))
                {
                    var message = String.Format("DUPLOCATE CONTRACT NAME '{0}', Type 1 '{1}' Type 2 '{2}' ", item.FullName);
                    System.Diagnostics.Trace.WriteLine(message);
                    throw new InvalidDataContractException(message);
                }
                ids.Add(item, gd);
                types.Add(gd, item);
            }
        }


        public byte[] GetTypeId(Type type)
        {
            Guid id = default(Guid);
            if (ids.TryGetValue(type, out id))
                return id.ToByteArray();
            else
            {
                var msg = String.Format("Unknown-Unregistered type: {0}. All types that are using runtime type inheritance should have DataContract Attribute. If this is a value type please use it as is, don't wrap it into object. If you still want to wrap it into object declare a new class with DataContractAttrbute and use the concrete type there", type.FullName);
                throw new InvalidOperationException(msg);
            }

        }
        public Type GetTypeById(byte[] id)
        {
            Type type = null;
            var ids = new Guid(id);
            if (types.TryGetValue(ids, out type))
                return type;
            else
                throw new InvalidOperationException("Unknown-Unregistered type id: " + ids);
        }


        public bool IsDynamicLenght { get { return false; } }

        public int Lenght { get { return 16; } }
    }
}

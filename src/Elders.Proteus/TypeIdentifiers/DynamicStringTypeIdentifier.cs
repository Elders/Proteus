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
    public class DynamicStringTypeIdentifier : ITypeIdentifier
    {
        HashSet<Assembly> assembliesWithDataContracts = new HashSet<Assembly>();
        private Dictionary<string, Type> types = new Dictionary<string, Type>();
        private Dictionary<Type, string> ids = new Dictionary<Type, string>();
        public IEnumerable<Type> AvailableTypes { get { return types.Select(x => x.Value); } }

        public DynamicStringTypeIdentifier(params Assembly[] assemblies)
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
                if (types.ContainsKey(attribute.Name))
                {
                    var message = String.Format("DUPLOCATE CONTRACT NAME '{0}', Type 1 '{1}' Type 2 '{2}' ", item.FullName);
                    System.Diagnostics.Trace.WriteLine(message);
                    throw new InvalidDataContractException(message);
                }
                ids.Add(item, attribute.Name);
                types.Add(attribute.Name, item);
            }
        }


        public byte[] GetTypeId(Type type)
        {
            string id = null;
            if (ids.TryGetValue(type, out id))
                return Encoding.UTF8.GetBytes(id);
            else
            {
                var msg = String.Format("Unknown-Unregistered type: {0}. All types that are using runtime type inheritance should have DataContract Attribute. If this is a value type please use it as is, don't wrap it into object. If you still want to wrap it into object declare a new class with DataContractAttrbute and use the concrete type there", type.FullName);
                throw new InvalidOperationException(msg);
            }

        }
        public Type GetTypeById(byte[] id)
        {
            Type type = null;
            var ids = Encoding.UTF8.GetString(id, 0, id.Length);
            if (types.TryGetValue(ids, out type))
                return type;
            else
                throw new InvalidOperationException("Unknown-Unregistered type id: " + ids);
        }

        public bool IsDynamicLenght { get { return true; } }

        public int Lenght { get { throw new InvalidOperationException("This identifier is with dynamic lenght"); } }


        public Dictionary<string, Type> Types
        {
            get { return types; }
        }

        public Dictionary<Type, string> Ids
        {
            get { return ids; }
        }
    }
}

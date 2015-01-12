using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus.Proxy
{
    public class ProxyFactory
    {
        private readonly Guid creationContext;
        private Dictionary<Type, ProxyBuilder> builders = new Dictionary<Type, ProxyBuilder>();
        private readonly ITypeIdentifier identifier;
        private Dictionary<Type, Type> proxies = new Dictionary<Type, Type>();
        public ProxyFactory(ITypeIdentifier identifier, Guid creationContext)
        {
            this.identifier = identifier;
            this.creationContext = creationContext;
            foreach (var item in identifier.GetAvailableTypesAndTheirSerializableParents().ToList())
            {
                if (item.IsGenericTypeDefinition)
                    continue;
                builders.Add(item, new ProxyBuilder(item, identifier, creationContext));
            }

            foreach (var item in builders.ToList())
            {
                var proxy = item.Value.Build(builders.Select(x => x.Value).ToList());
                proxies.Add(item.Value.ProxiedType, proxy);
            }
        }

        public Type GetProxy(Type type)
        {
            return proxies[type];
        }
    }
}

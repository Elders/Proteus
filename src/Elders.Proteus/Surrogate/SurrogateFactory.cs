using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus.Surrogate
{
    public class SurrogateFactory
    {
        private readonly Guid creationContext;
        private Dictionary<Type, SurrogateBuilder> builders = new Dictionary<Type, SurrogateBuilder>();
        private readonly ITypeIdentifier identifier;
        private Dictionary<Type, Type> proxies = new Dictionary<Type, Type>();
        public SurrogateFactory(ITypeIdentifier identifier, Guid creationContext)
        {
            this.identifier = identifier;
            this.creationContext = creationContext;
            foreach (var item in identifier.GetAvailableTypesAndTheirSerializableParents().ToList())
            {
                if (item.IsGenericTypeDefinition)
                    continue;
                builders.Add(item, new SurrogateBuilder(item, identifier, creationContext));
            }

            foreach (var item in builders.ToList())
            {
                var Surrogate = item.Value.Build(builders.Select(x => x.Value).ToList());
                proxies.Add(item.Value.ProxiedType, Surrogate);
            }
        }

        public Type GetSurrogate(Type type)
        {
            return proxies[type];
        }
    }
}

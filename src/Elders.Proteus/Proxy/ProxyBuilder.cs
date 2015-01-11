using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Elders.Proteus.Conversion;
using ProtoBuf.Meta;

namespace Elders.Proteus.Proxy
{
    public class ProxyBuilder
    {
        private readonly ITypeIdentifier identifier;
        public ProxyBuilder(Type proxiedType, ITypeIdentifier identifier, Guid creationContext)
        {
            this.identifier = identifier;
            Model = ModelBuilder.New();
            Proxy = RuntimeProxyBuilder.BuildDynamicProxy(proxiedType, creationContext);

            ProxiedType = proxiedType;
        }
        bool built;
        public Type Build(List<ProxyBuilder> allProxies)
        {
            if (built)
                return Proxy;
            foreach (var item in allProxies)
            {
                if (item.ProxiedType == this.ProxiedType)
                    continue;
                Model.Add(item.ProxiedType, true).SetSurrogate(item.Proxy);
            }

            var field = Proxy.GetField("Model", BindingFlags.Static | BindingFlags.Public);
            field.SetValue(null, Model);
            var idenField = Proxy.GetField("Identifier", BindingFlags.Static | BindingFlags.Public);
            idenField.SetValue(null, identifier);
            
            built = true;
            return Proxy;
        }

        RuntimeTypeModel Model { get; set; }
        Type Proxy { get; set; }
        public Type ProxiedType { get; set; }
    }
}

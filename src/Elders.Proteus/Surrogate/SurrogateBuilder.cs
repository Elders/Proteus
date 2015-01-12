using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Elders.Proteus.Conversion;
using ProtoBuf.Meta;

namespace Elders.Proteus.Surrogate
{
    public class SurrogateBuilder
    {
        private readonly ITypeIdentifier identifier;
        public SurrogateBuilder(Type proxiedType, ITypeIdentifier identifier, Guid creationContext)
        {
            this.identifier = identifier;
            Model = ModelBuilder.New();
            Surrogate = RuntimeSurrogateBuilder.BuildDynamicSurrogate(proxiedType, creationContext);

            ProxiedType = proxiedType;
        }
        bool built;
        public Type Build(List<SurrogateBuilder> allProxies)
        {
            if (built)
                return Surrogate;
            foreach (var item in allProxies)
            {
                if (item.ProxiedType == this.ProxiedType)
                    continue;
                Model.Add(item.ProxiedType, true).SetSurrogate(item.Surrogate);
            }

            var field = Surrogate.GetField("Model", BindingFlags.Static | BindingFlags.Public);
            field.SetValue(null, Model);
            var idenField = Surrogate.GetField("Identifier", BindingFlags.Static | BindingFlags.Public);
            idenField.SetValue(null, identifier);
            
            built = true;
            return Surrogate;
        }

        RuntimeTypeModel Model { get; set; }
        Type Surrogate { get; set; }
        public Type ProxiedType { get; set; }
    }
}

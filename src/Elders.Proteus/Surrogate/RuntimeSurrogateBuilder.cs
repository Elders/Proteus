using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus.Conversion
{
    public static class RuntimeSurrogateBuilder
    {
        static string assemblyName = "Elders.Reflection.Runtime.Surrogate";
        static AssemblyBuilder assemblyBuilder;
        static ModuleBuilder moduleBuilder;
        static RuntimeSurrogateBuilder()
        {
            AssemblyName asmName = new AssemblyName(assemblyName);
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName + ".Module");
        }
        public static Type BuildDynamicSurrogate(Type actualType, Guid SurrogateCreatorId)
        {
            var genericType = typeof(RuntimeSurrogate<,>);
            string SurrogateTypeName = actualType.FullName + "_" + SurrogateCreatorId.ToString().Replace("-", "_");
            var factoryTypeBuilder = moduleBuilder.DefineType(SurrogateTypeName, TypeAttributes.Class | TypeAttributes.Public, null, new Type[] { });
            factoryTypeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            var Surrogate = factoryTypeBuilder.CreateType();
            var concreteType = genericType.MakeGenericType(actualType, Surrogate);

            
            return concreteType;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Elders.Proteus.Conversion;

namespace Elders.Proteus.Proxy
{
    public class Class1
    {
        static string assemblyName = "Elders.Reflection.Runtime";
        static AssemblyBuilder assemblyBuilder;
        static ModuleBuilder moduleBuilder;
        static Class1()
        {
            AssemblyName asmName = new AssemblyName(assemblyName);
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName + ".Module");
        }
        public static Type BuildDynamicProxy(Type actualType, Guid proxyCreatorId, Serializer serializer)
        {
            var genericType = typeof(RuntimeProxy<,>);
            string proxyTypeName = actualType.FullName + "_" + proxyCreatorId.ToString().Replace("-", "_");
            var factoryTypeBuilder = moduleBuilder.DefineType(proxyTypeName, TypeAttributes.Class | TypeAttributes.Public, null, new Type[] { });
            factoryTypeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            var proxy = factoryTypeBuilder.CreateType();
            var concreteType = genericType.MakeGenericType(actualType, proxy);

            var field = concreteType.GetField("Serializer", BindingFlags.Static | BindingFlags.Public);
            field.SetValue(null, serializer);
            return concreteType;
        }

        public static void Emit(List<PropertyInfo> props)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elders.Proteus.Surrogate
{
    public interface IDynamicProxy
    {
        void Store(object holder);
        object Restore();
    }

    public class BaseProxy
    {
        public void BuildDelegates(IEnumerable<PropertyInfo> info)
        {
            var delegates = this.GetType().GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(x => x.Name.StartsWith("delegate")).ToList();
            foreach (var item in info)
            {
                var getter = ProxyBuilder.BuildGetter(item);

                var @delegate = delegates.Where(x => x.Name == "delegate" + item.GetMethod.Name).Single();
                @delegate.SetValue(null, getter);

            }
        }
    }
    public delegate object PropertyDelegate(object p0);
    public class ProxyBuilder
    {
        static string assemblyName = "Elders.Reflection.Runtime.Proxy";
        static AssemblyBuilder assemblyBuilder;
        static ModuleBuilder moduleBuilder;
        static ProxyBuilder()
        {
            AssemblyName asmName = new AssemblyName(assemblyName);
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName + ".Module");
            
            
        }

        public static Type BuildProxy(Type actualType, Guid creatorContextId)
        {

            string SurrogateTypeName = actualType.FullName + "_" + creatorContextId.ToString().Replace("-", "_");
            var typeBuilder = moduleBuilder.DefineType(SurrogateTypeName, TypeAttributes.Class | TypeAttributes.Public, typeof(BaseProxy), new Type[] { });
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(IDynamicProxy));
            var fieldBuilder = typeBuilder.DefineField("holder", typeof(object), FieldAttributes.Public);

            //Add properties
            var props = GetAllProperties(actualType).ToList();
            EmitPropeties(props, typeBuilder, fieldBuilder, moduleBuilder);
            //End Add properties


            //Implement IDynamicProxy.Store
            var storeMethod = typeBuilder.DefineMethod("Store", MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[1] { typeof(object) });
            EmitStoreMethod(actualType, storeMethod.GetILGenerator(), fieldBuilder);
            typeBuilder.DefineMethodOverride(storeMethod, typeof(IDynamicProxy).GetMethods().Where(x => x.Name == "Store").Single());
            //End Implement IDynamicProxy.Store

            //Implement IDynamicProxy.Restore
            var factoryMethod = typeBuilder.DefineMethod("Restore", MethodAttributes.Public | MethodAttributes.Virtual, typeof(object), new Type[] { });
            var emitter = factoryMethod.GetILGenerator();
            EmitRestoreMethod(actualType, emitter, fieldBuilder);
            typeBuilder.DefineMethodOverride(factoryMethod, typeof(IDynamicProxy).GetMethods().Where(x => x.Name == "Restore").Single());
            //End Implement IDynamicProxy.Restore

            var type = typeBuilder.CreateType();
            var proxy = Activator.CreateInstance(type) as BaseProxy;
            proxy.BuildDelegates(props);
            return type;
        }

        public static void EmitStoreMethod(Type ActualType, ILGenerator emitter, FieldBuilder fieldBuilder)
        {
            emitter.BeginScope();
            emitter.Emit(OpCodes.Ldarg_0);
            emitter.Emit(OpCodes.Ldarg_1);
            emitter.Emit(OpCodes.Stfld, fieldBuilder);
            emitter.Emit(OpCodes.Ret);
            emitter.EndScope();
            //emitter.Emit(OpCodes.st)
        }
        public static void EmitRestoreMethod(Type ActualType, ILGenerator emitter, FieldBuilder fieldBuilder)
        {
            emitter.BeginScope();
            emitter.Emit(OpCodes.Ldarg_0);
            emitter.Emit(OpCodes.Ldfld, fieldBuilder);
            emitter.Emit(OpCodes.Ret);
            emitter.EndScope();
            //emitter.Emit(OpCodes.st)
        }

        public static void EmitPropeties(IEnumerable<PropertyInfo> properties, TypeBuilder builder, FieldBuilder field, Module module)
        {
            foreach (var item in properties)
            {
                new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Demand();
                var prop = builder.DefineProperty(item.Name, PropertyAttributes.None, item.PropertyType, null);
                //Emit Set Method
                var setter = builder.DefineMethod("set_" + item.Name,
                                       MethodAttributes.Public | MethodAttributes.SpecialName,
                                       null,
                                       new Type[1] { typeof(object) });
                var emitter = setter.GetILGenerator();

                emitter.BeginScope();
                emitter.Emit(OpCodes.Ldarg_0);
                emitter.Emit(OpCodes.Ldarg_1);
                emitter.Emit(OpCodes.Ldfld, field);
                emitter.Emit(OpCodes.Callvirt, item.GetSetMethod(true));
                emitter.Emit(OpCodes.Ret);
                emitter.EndScope();
                prop.SetSetMethod(setter);

                //Emit Get Method

                var getterDelegate = builder.DefineField("delegate" + item.GetMethod.Name, typeof(object), FieldAttributes.Static | FieldAttributes.Private);
                var getter = builder.DefineMethod("get_" + item.Name,
                                     MethodAttributes.Public | MethodAttributes.SpecialName,
                                     item.PropertyType,

                                     new Type[] { });
                var method = typeof(Func<object, object>).GetMethods().Where(x => x.Name == "Invoke").First();
                emitter = getter.GetILGenerator();
                emitter.BeginScope();
                emitter.Emit(OpCodes.Ldarg_0);
                emitter.Emit(OpCodes.Ldsfld, getterDelegate);
                emitter.Emit(OpCodes.Ldfld, field);
                emitter.Emit(OpCodes.Callvirt, method); //Get the property value
                
                emitter.Emit(OpCodes.Ret);
                emitter.EndScope();
                prop.SetGetMethod(getter);
            }
        }

        public static Func<object, object> BuildGetter(PropertyInfo propertyInfo)
        {
            var propertyGetter = CreatePropertyGetter(propertyInfo, typeof(object),
                         typeof(object));
            return x => propertyGetter.Invoke(x, null);
        }
        private static DynamicMethod CreatePropertyGetter(PropertyInfo propertyInfo,
                                  Type targetType, Type resultType)
        {
            //if (!propertyInfo.CanRead)
            //    throw new ArgumentException(string.Format("The property {0} in type {1} doesn't have getter.",
            //                                                propertyInfo.PropertyType, propertyInfo.DeclaringType));

            //ParameterExpression targetExp = Expression.Parameter(targetType, "target");
            //MemberExpression propertyExp;
            ////Checking if it's a static.
            //if (propertyInfo.GetGetMethod(true).IsStatic)
            //    propertyExp = Expression.Property(null, propertyInfo);
            //else
            //{
            //    //Checking if target type is not equals to DeclaringType, we need to convert type.
            //    if (targetType != propertyInfo.DeclaringType)
            //        propertyExp = Expression.Property(Expression.Convert(targetExp, propertyInfo.DeclaringType),
            //                                            propertyInfo);
            //    else
            //        propertyExp = Expression.Property(targetExp, propertyInfo.GetMethod);
            //}
            // return Expression.Lambda<TResult>(Expression.Convert(propertyExp, resultType), targetExp).Compile();
            DynamicMethod dynamicMethod = new DynamicMethod("runtimeDelegate" + propertyInfo.GetMethod.Name, typeof(object), new Type[] { typeof(object) }, true);
            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Callvirt, propertyInfo.GetMethod);
            generator.Emit(OpCodes.Ret);
            return dynamicMethod;

        }
        public static IEnumerable<PropertyInfo> GetAllProperties(Type type)
        {
            if (type == typeof(object) || type == null)
                yield break;
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in props)
            {
                if (item.DeclaringType == type)
                    yield return item;
            }
            var baseP = GetAllProperties(type.BaseType);
            foreach (var item in baseP)
            {
                yield return item;
            }
        }
    }
}

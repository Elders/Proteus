using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ProtoBuf.Meta;
using System.Runtime.Serialization;
using Elders.Proteus.Conversion;
using System.Diagnostics;
namespace Elders.Proteus
{

    public class Serializer
    {
        private readonly ITypeIdentifier identifier;
        RuntimeTypeModel protobufTypeModel;
        ProteusRuntimeTypeModel proteusTypeModel;
        public Guid Id { get; private set; }
        //tezi ctors shte gi promenim, os tadvaa daaa :) t
        // ostava samo tova s private, taka li? da
        // moze li s protected? za modmaenta?dakkaide da se biem na HS 1 igra 4e mn mi s spi k?kk mynkow@gmail.com
        public Serializer()
            : this(new GuidTypeIdentifier(Assembly.GetCallingAssembly()))
        {

        }
        public Serializer(ITypeIdentifier identifier)
        {
            Id = Guid.NewGuid();
            proteusTypeModel = new ProteusRuntimeTypeModel(identifier);
            this.identifier = identifier;
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            CultureInfo culture = CultureInfo.InvariantCulture; // use InvariantCulture or other if you prefer
            protobufTypeModel = (RuntimeTypeModel)
              Activator.CreateInstance(typeof(RuntimeTypeModel), flags, null, new object[] { true }, culture);
            protobufTypeModel.AutoAddMissingTypes = true;

            List<Type> builtProxies = new List<Type>();
            foreach (var item in identifier.AvailableTypes.ToList())
            {
                var metaType = protobufTypeModel.Add(item, true);
                var fields = metaType.GetFields();
                IncludeParents(metaType);
                foreach (var field in fields)
                {
                    if (!builtProxies.Contains(field.MemberType))
                    {
                        bool shouldBuildProxy = identifier.AvailableTypes.Any(x => field.MemberType.IsAssignableFrom(x) && x != field.MemberType);
                        if (shouldBuildProxy)
                        {
                            var proxy = RuntimeProxyBuilder.BuildDynamicProxy(field.MemberType, this.Id, this);
                            protobufTypeModel.Add(field.MemberType, false).SetSurrogate(proxy);
                            builtProxies.Add(field.MemberType);
                        }

                    }
                }
            }
        }



        private void IncludeParents(MetaType type)
        {
            if (type.Type.BaseType == typeof(object))
                return;
            var metaBase = protobufTypeModel.Add(type.Type.BaseType, true);
            IncludeParents(metaBase);
            if (identifier.AvailableTypes.Contains(type.Type.BaseType))
            {
                foreach (var field in metaBase.GetFields())
                {

                    if (!type.GetFields().Select(x => x.FieldNumber).Contains(field.FieldNumber))
                    {
                        //mi shte gi pishem protected dai 6te go pogledna da go vidq kag go e napisal onq oligofren
                        //kk mislish li che tova shte opravi greshkite koito sa v log-a, edni i sashti li sa origins gre6kite v loga sa ot tova 4e ne se registrirat pravilno asemblitata
                        //  znachi ti registrirash implicit cronus assembly-to, drugoto explicit, taka li
                        // Constructora na Serializer se vika v Cronus, az Registriram Cronus+ vsi4ki koito cronus refference t.e. i domain modelling
                        //  ami kato puskam assemly-ta taka, taka pyk ne vliza cronus + domain modeling :D
                        // qsno

                        // type.BaseType.AddField(field.FieldNumber, field.Member.Name);
                        var asd = type.AddField(field.FieldNumber, field.Member.Name);
                        //asd.
                        //asd.SetSpecified(field.ParentType.GetProperty(field.Member.Name).GetMethod, field.ParentType.GetProperty(field.Member.Name).SetMethod);
                        //Trace.Write(asd);
                    }
                }
            }
        }
        public T Deserialize<T>(Stream source)
        {
            return (T)protobufTypeModel.Deserialize(source, null, typeof(T));
        }

        public void Serialize<T>(Stream destination, T instance)
        {
            if (instance != null)
            {
                protobufTypeModel.Serialize(destination, instance);
            }
        }

        public void SerializeWithHeaders(Stream destination, object instance)
        {
            if (instance != null)
            {
                proteusTypeModel.WriteType(destination, instance.GetType());
                protobufTypeModel.Serialize(destination, instance);
            }
        }

        public object DeserializeWithHeaders(Stream source)
        {
            var type = proteusTypeModel.ReadType(source);
            return protobufTypeModel.Deserialize(source, null, type);
        }



    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf.Meta;

namespace Elders.Proteus
{
    public static class ModelBuilder
    {

        public static RuntimeTypeModel New()
        {
            CultureInfo culture = CultureInfo.InvariantCulture; // use InvariantCulture or other if you prefer
            var instance = (RuntimeTypeModel)
              Activator.CreateInstance(typeof(RuntimeTypeModel), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { true }, culture);
            instance.AutoAddMissingTypes = true;
            return instance;
        }

        public static void IncludeParents(this RuntimeTypeModel model, ITypeIdentifier typeIdentifier, MetaType type)
        {
            if (type.Type.BaseType == typeof(object) || type.Type.BaseType == null)
                return;
            var metaBase = model.Add(type.Type.BaseType, true);
            model.IncludeParents(typeIdentifier, metaBase);
            if (typeIdentifier.AvailableTypes.Contains(type.Type.BaseType))
                foreach (var field in metaBase.GetFields())
                {
                    if (!type.GetFields().Select(x => x.FieldNumber).Contains(field.FieldNumber))
                        type.AddField(field.FieldNumber, field.Member.Name);
                }
        }

    }
}

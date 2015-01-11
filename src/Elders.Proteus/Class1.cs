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
    }
}

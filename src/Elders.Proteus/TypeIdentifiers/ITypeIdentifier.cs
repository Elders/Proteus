using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus
{
    public interface ITypeIdentifier
    {
        byte[] GetTypeId(Type type);
        Type GetTypeById(byte[] id);
        bool IsDynamicLenght { get; }
        int Lenght { get; }

        IEnumerable<Type> AvailableTypes { get; }

        Dictionary<string, Type> Types { get; }

        Dictionary<Type, string> Ids { get; }

    }
}

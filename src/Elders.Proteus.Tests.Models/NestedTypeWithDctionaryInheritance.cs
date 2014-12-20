using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus.Tests
{
    [DataContract(Name = "0ad646dc-4a23-4c82-b909-46f6dea7c55e")]
    public class NestedTypeWithDctionaryInheritance
    {
        [DataMember(Order = 1)]
        public string String { get; set; }

        [DataMember(Order = 2)]
        public int Int { get; set; }

        [DataMember(Order = 3)]
        public DateTime Date { get; set; }

        [DataMember(Order = 4)]
        public Dictionary<object, object> Nested { get; set; }
    }

    [DataContract(Name = "035b6264-b09c-42e3-bac9-f69d8f1bb643")]
    public class UndefinedDictionaryInheritance
    {
        [DataMember(Order = 1)]
        public string String { get; set; }
        [DataMember(Order = 2)]
        public int Int { get; set; }

        [DataMember(Order = 3)]
        public DateTime Date { get; set; }

        [DataMember(Order = 4)]
        public object Nested { get; set; }
    }
}

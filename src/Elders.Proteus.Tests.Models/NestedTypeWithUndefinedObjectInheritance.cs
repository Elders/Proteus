using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus.Tests
{
    [DataContract(Name = "cc5167d2-b68d-45cc-802d-c011d39bc332")]
    public class NestedTypeWithUndefinedObjectInheritance
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

    [DataContract(Name = "89578728-ba95-4c5a-864d-9ecefcb7b1b6")]
    public class UndefinedObjectInheritance
    {
        [DataMember(Order = 1)]
        public string String { get; set; }
        [DataMember(Order = 2)]
        public int Int { get; set; }

        [DataMember(Order = 3)]
        public DateTime Date { get; set; }

        [DataMember(Order = 4)]
        public SimpleType Nested { get; set; }
    }
}

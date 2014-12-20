using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus.Tests
{
    [DataContract(Name = "5976ccac-ec1f-42c2-8631-b42f25ab82c6")]
    public class UndefinedBaseClass : UndefinedBaseBaseClass
    {
        [DataMember(Order = 22)]
        public string CustomBaseClassString { get; set; }
    }


    [DataContract(Name = "7cc8b57e-d49e-413a-ac7d-da97b2b5ff08")]
    public class UndefinedBaseBaseClass
    {
        [DataMember(Order = 23)]
        public string CustomBaseBaseClassString { get; set; }
    }

    [DataContract(Name = "512bb870-5134-4122-803e-e7d386127826")]
    public class NestedTypeWithBaseClassInheritance
    {
        [DataMember(Order = 1)]
        public string String { get; set; }

        [DataMember(Order = 2)]
        public int Int { get; set; }

        [DataMember(Order = 3)]
        public DateTime Date { get; set; }

        [DataMember(Order = 4)]
        public UndefinedBaseClass Nested { get; set; }
    }

    [DataContract(Name = "75438806-19a6-4178-804b-9b232be95a4a")]
    public class UndefinedBaseclassInheritance : UndefinedBaseClass
    {
        public UndefinedBaseclassInheritance()
        {

        }

        [DataMember(Order = 1)]
        public string String { get; set; }
        [DataMember(Order = 2)]
        public int Int { get; set; }

        [DataMember(Order = 3)]
        public DateTime Date { get; set; }

        [DataMember(Order = 4)]
        public UndefinedBaseClass BaseClass { get; set; }


    }
}

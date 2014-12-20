using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus.Tests
{
    public interface IUndefinedInterface
    {

    }


    [DataContract(Name = "e85cd0f8-2da8-45d9-bb50-07dc887d9ee9")]
    public class NestedTypeWithUndefinedInterfaceInheritance
    {
        [DataMember(Order = 1)]
        public string String { get; set; }

        [DataMember(Order = 2)]
        public int Int { get; set; }

        [DataMember(Order = 3)]
        public DateTime Date { get; set; }

        [DataMember(Order = 4)]
        public IUndefinedInterface Nested { get; set; }
    }

    [DataContract(Name = "1270ddf4-e985-4f21-b31e-3c9650d8b47d")]
    public class UndefinedInterfaceInheritance : IUndefinedInterface
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

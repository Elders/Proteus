using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elders.Proteus.Tests.Models;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace Elders.Proteus.Tests
{
    [Subject(typeof(DynamicStringTypeIdentifier))]
    public class When_making_an_instance_of_DefaultTypeIdentifier
    {
        static DynamicStringTypeIdentifier identifier;
        Establish context = () =>
        {
        };
        Because of = () => { identifier = new DynamicStringTypeIdentifier(typeof(When_making_an_instance_of_DefaultTypeIdentifier).Assembly); };

        It should_find_all_types_with_data_contract_attribute = () => identifier.AvailableTypes.ShouldContain(typeof(ExtenralReferencedType));
    }
}

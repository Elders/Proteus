using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus.Proxy
{
    public static class ITypeIdentifierExtensions
    {
        public static IEnumerable<Type> GetAvailableTypesAndTheirSerializableParents(this ITypeIdentifier identifier)
        {
            var search = identifier.AvailableTypes.ToList();
            var returned = new List<Type>();
            foreach (var item in identifier.AvailableTypes.ToList())
            {
                if (!returned.Contains(item))
                {
                    returned.Add(item);
                    yield return item;

                }
                var props = item.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var pr in props)
                {
                    if (search.Any(x => pr.PropertyType.IsAssignableFrom(x)))
                    {
                        if (!returned.Contains(pr.PropertyType))
                        {
                            returned.Add(pr.PropertyType);
                            yield return pr.PropertyType;

                        }
                    }
                }
            }
        }
    }
}

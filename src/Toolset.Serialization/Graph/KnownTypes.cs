using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Graph
{
  internal sealed class KnownTypes : IEnumerable<Type>
  {
    private readonly List<Type> knownTypes;

    public KnownTypes(Type type, IEnumerable<Type> otherTypes)
    {
      this.knownTypes = new List<Type>();

      var types = new[] { type }.Union(otherTypes ?? Enumerable.Empty<Type>());
      Enumerate(types);
    }

    public IEnumerator<Type> GetEnumerator()
    {
      return knownTypes.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private void Enumerate(IEnumerable<Type> types)
    {
      foreach (var type in types.Where(t => t != null))
      {
        if (!type.Namespace.Equals("System")
        && !type.Namespace.StartsWith("System.")
        && !knownTypes.Contains(type))
        {
          knownTypes.Add(type);

          var otherTypes =
            // tipo da propriedade
            type.GetProperties().Select(p => p.PropertyType)
            // tipo do array
            .Union(type.GetProperties().Select(p => p.PropertyType.GetElementType()))
            // tipo do parâmetro de tipo genérico
            .Union(type.GetProperties().SelectMany(p => p.PropertyType.GetGenericArguments()));

          Enumerate(otherTypes.Distinct());
        }
      }
    }
  }
}

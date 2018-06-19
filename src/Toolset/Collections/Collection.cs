using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolset.Collections;

namespace Toolset.Collections
{
  /// <summary>
  /// Implementação de uma coleção de objetos com possibilidade de herança e sobreposição de métodos.
  /// </summary>
  /// <seealso cref="Toolset.Collections.Collection{System.Object}" />
  public class Collection : Collection<object>
  {
    public Collection()
    {
    }

    public Collection(int capacity)
      : base(capacity)
    {
    }

    public Collection(IEnumerable items)
      : base(items.Cast<object>())
    {
    }
  }
}
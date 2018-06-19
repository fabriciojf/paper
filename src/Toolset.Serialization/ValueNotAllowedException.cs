using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public class ValueNotAllowedException : ValidationException
  {
    public ValueNotAllowedException(Node parentNode, Node invalidNode)
      : base(CreateMessage(parentNode, invalidNode))
    {
    }

    private static string CreateMessage(Node parentNode, Node invalidNode)
    {
      var text = "O token não aceita valor: " + invalidNode;
      if (parentNode != null)
      {
        text += " (Próximo de: " + parentNode + ")";
      }
      return text;
    }
  }
}
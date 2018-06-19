using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public class ChildCountException : ValidationException
  {
    public ChildCountException(Node parentNode, Node invalidNode, int minimum, int maximum, int actualCount)
      : base(CreateMessage(parentNode, invalidNode, minimum, maximum, actualCount))
    {
    }

    private static string CreateMessage(Node parentNode, Node invalidNode, int minimum, int maximum, int actualCount)
    {
      var text = "";
      
      if (minimum == 0 && maximum == 0)
      {
        text += "O token não deve ter tokens filhos: " + invalidNode;
      }
      else if (minimum == 1 && maximum == 1)
      {
        text += "O token aceita um e somente um token filho: " + invalidNode;
      }
      else
      {
        text += "O token tem uma quantidade inválida de tokens filho."
             +  " Era esperado uma quantidade entre " + minimum + " e " + minimum
             +  " mas " + actualCount + " foi observado.";
      }

      if (parentNode != null)
      {
        text += " (Próximo de: " + parentNode + ")";
      }
      return text;
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public class EndOfStreamException : ValidationException
  {
    public EndOfStreamException(Node parentNode, IEnumerable<NodeType> expectation)
      : base(CreateMessage(parentNode, expectation))
    {
    }

    private static string CreateMessage(Node parentNode, IEnumerable<NodeType> expectation)
    {
      var text = "Fim prematuro do Stream.";
      if (parentNode != null)
      {
        text += " (Próximo de: " + parentNode + ")";
      }
      if (expectation != null && expectation.Any())
      {
        text += " Tokens esperados: " + string.Join(", ", expectation) + ".";
      }
      return text;
    }
  }
}
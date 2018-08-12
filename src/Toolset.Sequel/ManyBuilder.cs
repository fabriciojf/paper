using System;
using System.Collections.Generic;
using System.Text;
using Toolset.Collections;

namespace Toolset.Sequel
{
  public class ManyBuilder : IParameterMap
  {
    internal IDictionary<string, object> Parameters { get; } = new HashMap();

    IDictionary<string, object> IParameterMap.Parameters => Parameters;

    internal ManyBuilder()
    {
    }
  }
}

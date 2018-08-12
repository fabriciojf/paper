using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Sequel
{
  public interface IParameterMap
  {
    IDictionary<string, object> Parameters { get; }
  }
}
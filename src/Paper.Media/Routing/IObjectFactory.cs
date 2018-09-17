using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public interface IObjectFactory
  {
    object CreateInstance(Type type, params object[] args);
  }
}
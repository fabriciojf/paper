using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public class DefaultObjectFactory : IObjectFactory
  {
    public void Add(object instance)
    {
      Add(instance.GetType(), instance);
    }

    public void Add(Type type, object instance)
    {

    }

    public object CreateInstance(Type type, params object[] args)
    {
      return null;
    }
  }
}
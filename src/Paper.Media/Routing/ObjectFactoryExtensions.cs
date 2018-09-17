using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public static class ObjectFactoryExtensions
  {
    public static T CreateInstance<T>(this IObjectFactory factory, params object[] args)
    {
      return (T)factory.CreateInstance(typeof(T), args);
    }
  }
}
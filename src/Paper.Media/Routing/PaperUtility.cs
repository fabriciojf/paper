using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public class PaperUtility
  {
    public static Entity GetEntity(string route, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static Entity GetEntity(string route, IArgs args)
    {
      throw new NotImplementedException();
    }

    public static Entity GetEntity(Type paperType, params object[] args)
    {
      throw new NotImplementedException();
    }

    public static Entity GetEntity(Type paperType, IArgs args)
    {
      throw new NotImplementedException();
    }

    public static Entity GetEntity<T>(Action<T> setup, params object[] args)
      where T : IPaper
    {
      throw new NotImplementedException();
    }

    public static Entity GetEntity<T>(Action<T> setup, IArgs args)
      where T : IPaper
    {
      throw new NotImplementedException();
    }

    public static Entity GetEntity<T>(Action<T> setup)
      where T : IPaper
    {
      throw new NotImplementedException();
    }

    public static IArgs GetEntityArgs(string route)
    {
      throw new NotImplementedException();
    }

    public static IArgs GetEntityArgs(Type paperType)
    {
      throw new NotImplementedException();
    }

    public static IArgs GetEntityArgs<T>()
      where T : IPaper
    {
      throw new NotImplementedException();
    }
  }
}
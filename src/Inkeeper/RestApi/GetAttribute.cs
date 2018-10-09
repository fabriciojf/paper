using System;
using System.Collections.Generic;
using System.Text;

namespace Inkeeper.RestApi
{
  [AttributeUsage(AttributeTargets.Method)]
  public class GetAttribute : WebMethodAttribute
  {
    public GetAttribute()
      : base("GET")
    {
    }

    public GetAttribute(string route)
      : base("GET", route)
    {
    }
  }
}

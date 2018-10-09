using System;
using System.Collections.Generic;
using System.Text;

namespace Inkeeper.RestApi
{
  [AttributeUsage(AttributeTargets.Method)]
  public class PutAttribute : WebMethodAttribute
  {
    public PutAttribute()
      : base("PUT")
    {
    }

    public PutAttribute(string route)
      : base("PUT", route)
    {
    }
  }
}

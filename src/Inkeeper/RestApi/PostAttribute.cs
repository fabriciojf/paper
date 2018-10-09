using System;
using System.Collections.Generic;
using System.Text;

namespace Inkeeper.RestApi
{
  [AttributeUsage(AttributeTargets.Method)]
  public class PostAttribute : WebMethodAttribute
  {
    public PostAttribute()
      : base("POST")
    {
    }

    public PostAttribute(string route)
      : base("POST", route)
    {
    }
  }
}

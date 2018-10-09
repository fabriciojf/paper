using System;
using System.Collections.Generic;
using System.Text;

namespace Inkeeper.RestApi
{
  [AttributeUsage(AttributeTargets.Method)]
  public class DeleteAttribute : WebMethodAttribute
  {
    public DeleteAttribute()
      : base("DELETE")
    {
    }

    public DeleteAttribute(string route)
      : base("DELETE", route)
    {
    }
  }
}
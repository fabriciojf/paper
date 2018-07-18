using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paper.Media.Papers
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class PaperAttribute : Attribute
  {
    public PaperAttribute(string uriTemplate)
    {
      this.UriTemplate = uriTemplate;
    }

    public string UriTemplate { get; }

    public static PaperAttribute GetFrom(object typeOrObject)
    {
      var type = typeOrObject as Type ?? typeOrObject?.GetType();
      if (type == null)
        return null;

      return type
        .GetCustomAttributes(true)
        .OfType<PaperAttribute>()
        .FirstOrDefault();
    }
  }
}
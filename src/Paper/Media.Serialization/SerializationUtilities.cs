using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Paper.Media.Serialization
{
  static class SerializationUtilities
  {
    public static bool IsStringCompatible(object value)
    {
      return
           value is string
        || value is Uri
        || value is PathString
        || value is CaseVariantString;
    }
  }
}

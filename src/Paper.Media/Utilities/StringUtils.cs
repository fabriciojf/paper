using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Utilities
{
  /// <summary>
  /// Utilitários para strings.
  /// </summary>
  static class StringUtils
  {
    /// <summary>
    /// Determina se o tipo deve ser considerado compatível com string.
    /// </summary>
    /// <param name="value">O valor a ser verificado.</param>
    /// <returns>Verdadeiro se o tipo pode ser considerado string; Falso caso contrário.</returns>
    public static bool IsStringCompatible(object value)
    {
      if (value == null)
        return false;

      return value is string
          || value is Uri
          || value is CaseVariantString
          || value.GetType().FullName == "Microsoft.AspNetCore.Http.PathString";
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Toolset.Serialization
{
  public static class ValueConventions
  {
    public static string EscapeLiterals(string sequence)
    {
      //
      // Referência:
      // - 2.4.4.5 String literals
      //   https://msdn.microsoft.com/en-us/library/aa691090%28v=vs.71%29.aspx
      //
      return
        sequence
          .Replace("\\", "\\\\")
          .Replace("\"", "\\\"")
          .Replace("\0", "\\0")
          .Replace("\a", "\\a")
          .Replace("\b", "\\b")
          .Replace("\f", "\\f")
          .Replace("\n", "\\n")
          .Replace("\r", "\\r")
          .Replace("\t", "\\t")
          .Replace("\v", "\\v");
    }

    public static string UnescapeLiterals(string sequence)
    {
      return Regex.Unescape(sequence);
    }

    public static string CreateQuotedText(object value, SerializationSettings settings, Func<string, string> escapeMethod = null)
    {
      if (value == null)
        return "null";

      if (escapeMethod == null)
        escapeMethod = EscapeLiterals;

      var text = CreateText(value, settings);

      bool isNumeric = false;
      // Quando um número extrapola o limite da capacidade deve
      // então ser tratado como texto.
      // Mas como determinar se o número extrapola o limite?
      // De qualquer forma qualquer manutenção futura deve
      // considerar mais de 15 caracteres como texto, para evitar
      // problemas de compatibilidade com o uso atual da API.
      //
      // Mar/2017
      // Guga Coder
      if (text.Length <= 15) {
        isNumeric = Regex.IsMatch(text, @"^\d+([.]\d+)?$", RegexOptions.CultureInvariant);
      }
      if (!isNumeric)
      {
        text = '"' + escapeMethod.Invoke(text) + '"';
      }

      return text;
    }

    public static string CreateText(object value, SerializationSettings settings)
    {
      if (value == null)
        return "null";

      if (value is bool)
        return ((bool)value) ? "1" : "0";

      if (value is Enum)
        return ((int)value).ToString();

      if (value is DateTime)
        return ((DateTime)value).ToString(settings.DateTimeFormat);

      var formatter = value.GetType().GetMethod("ToString", new[] { typeof(IFormatProvider) });
      if (formatter != null)
        return (string)formatter.Invoke(value, new[] { settings.CultureInfo });

      return value.ToString();
    }

    public static object CreateValue(string text, SerializationSettings settings, Func<string, string> unescapeMethod = null)
    {
      if ((text == null) || (text == "null"))
        return null;

      if (unescapeMethod == null)
        unescapeMethod = UnescapeLiterals;

      try
      {
        text = unescapeMethod.Invoke(text);

        var isNumeric =
          !text.StartsWith("\"")
          && Regex.IsMatch(text, @"^\d+([.]\d+)?$", RegexOptions.CultureInvariant);

        if (isNumeric)
        {
          if (text.Contains('.'))
          {
            return double.Parse(text, settings.CultureInfo);
          }
          else
          {
            var value = long.Parse(text);
            if (value >= int.MinValue && value <= int.MaxValue)
            {
              return (int)value;
            }
            return value;
          }
        }
        else
        {
          if (text.StartsWith("\""))
            text = text.Substring(1, text.Length - 2);

          var format = settings.DateTimeFormat;
          var culture = settings.CultureInfo;
          var dateStyle = DateTimeStyles.None;
          var dateTime = DateTime.MinValue;
          var ok = DateTime.TryParseExact(text, format, culture, dateStyle, out dateTime);
          if (ok)
            return dateTime;

          return text;
        }
      }
      catch
      {
        return text;
      }
    }

    public static string CreateName(object text, SerializationSettings settings, TextCase defaultCase)
    {
      if (text == null)
        return null;

      return CreateName(text.ToString(), settings, defaultCase);
    }

    public static string CreateName(string text, SerializationSettings settings, TextCase defaultCase)
    {
      if (text == null)
        return null;

      if (text.StartsWith("\"") || text.StartsWith("'"))
        text = text.Substring(1, text.Length - 2);

      var textCase = (settings.TextCase == TextCase.Default) ? defaultCase : settings.TextCase;
      return text.ChangeCase(textCase);
    }

  }
}

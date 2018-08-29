using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Paper.Media;
using Toolset;

namespace Paper.Core.Utilities
{
  public static class HttpNegotiation
  {
    public static string SelectContentType(HttpRequest req)
    {
      if (req.Query["f"] == "siren" || req.Query["out"] == "siren")
        return "application/vnd.siren+json";

      if (req.Query["f"] == "json" || req.Query["out"] == "json")
        return "application/json";

      if (req.Query["f"] == "xml" || req.Query["out"] == "xml")
        return "application/xml";

      var acceptHeader = (string)req.Headers[HeaderNames.Accept] ?? "";

      if (acceptHeader.Contains("application/json") || acceptHeader.Contains("*/json"))
        return "application/json";
      if (acceptHeader.Contains("text/json"))
        return "text/json";

      if (acceptHeader.Contains("application/xml") || acceptHeader.Contains("*/xml"))
        return "application/xml";
      if (acceptHeader.Contains("text/xml"))
        return "text/xml";

      return "application/vnd.siren+json";
    }

    public static Encoding SelectEncoding(HttpRequest req)
    {
      var charset = SelectOption(req.Headers[HeaderNames.AcceptCharset], "UTF-8");
      try
      {
        return Encoding.GetEncoding(charset);
      }
      catch
      {
        return Encoding.UTF8;
      }
    }

    public static string SelectOption(string acceptHeader, string defaultOption)
    {
      if (acceptHeader == null)
        return defaultOption;

      // Aplicando QualityValue para selecionar a melhor opção
      // https://developer.mozilla.org/en-US/docs/Glossary/Quality_values
      var choices =
        from option in acceptHeader.Split(',')
        let tokens = option.Split(':')
        let value = tokens.First().Replace("*", defaultOption)
        let priority = ToDouble(tokens.Skip(1).FirstOrDefault(), 1, 0)
        orderby priority descending
        select option;
      var choice = choices.FirstOrDefault();
      return choice ?? defaultOption;
    }

    private static double ToDouble(string text, double defaultValue, double errorValue)
    {
      if (string.IsNullOrWhiteSpace(text))
        return defaultValue;

      double number;
      var ok = double.TryParse(text.Trim(), out number);
      return ok ? number : errorValue;
    }
  }
}
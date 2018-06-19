using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Toolset.Text.Template
{
  class FormatExpression : Expression
  {
    private string format;

    public FormatExpression(string format)
    {
      var groupRegex = new Regex("{([^{]*)}");
      var validRegex = new Regex("^(0|[1-9][0-9]*)(?:,[0-9]+)?(?::.+)?$");

      var matches = groupRegex.Matches(format);
      foreach (var match in matches.Cast<Match>())
      {
        var innerText = match.Groups[1].Value;
        if (innerText == "")
        {
          format = format.Replace(match.Value, "{0}");
        }
        else if (!validRegex.IsMatch(innerText))
        {
          var validValue = "{0:" + innerText + "}";
          format = format.Replace(match.Value, validValue);
        }
      }

      this.format = format;
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      try
      {

        var arguments = new object[] { input.Value };
        if (input.Array != null)
        {
          arguments = arguments.Concat(input.Array).ToArray();
        }

        var result = string.Format(format, arguments);
        return new Pipe(result);

      }
      catch (Exception ex)
      {
        ex.Report("O formato não é válido: " + format);
        return input;
      }
    }
  }
}

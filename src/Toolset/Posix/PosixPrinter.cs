using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using static Toolset.Posix.Prompt;

namespace Toolset.Posix
{
  /// <summary>
  /// Interpretador de opções.
  /// </summary>
  public class PosixPrinter
  {
    /// <summary>
    /// Imprime o texto de ajuda.
    /// </summary>
    /// <param name="target">O objeto que declara as opções.</param>
    /// <returns>Um status de saída indicando sucesso.</returns>
    public static int PrintHelp(object target)
    {
      if (target is Type)
        throw new InvalidUsageException($"Era esperado um objeto mas foi recebido um tipo: {((Type)target).FullName}");

      var type = (target is Type) ? (Type)target : target.GetType();

      var appName = GetAppName(target);
      var appTitle = GetAppTitle(target);

      var header = (
        from a in type.GetCustomAttributes(true).OfType<HelpHeaderAttribute>()
        select a.Text
      ).SingleOrDefault();

      var classSections =
        from a in type.GetCustomAttributes(true).OfType<HelpSectionAttribute>()
        select new { Attribute = a, Method = (MethodInfo)null };

      var propertySections =
        from p in type.GetProperties()
        from a in p.GetCustomAttributes(true).OfType<HelpSectionAttribute>()
        select new { Attribute = a, Method = (MethodInfo)null };

      var methodSections =
        from m in type.GetMethods()
        from a in m.GetCustomAttributes(true).OfType<HelpSectionAttribute>()
        select new { Attribute = a, Method = m };

      var sections = (
        from x in classSections.Concat(propertySections).Concat(methodSections)
        select new
        {
          x.Attribute.Name,
          AfterUsage = x.Attribute.Order == SectionOrder.AfterUsage,
          Text =
            (x.Method != null)
              ? GetString(target, x.Method)
              : x.Attribute.Text
        }
      ).ToArray();

      var footer = (
        from a in type.GetCustomAttributes(true).OfType<HelpFooterAttribute>()
        select a.Text
      ).FirstOrDefault();

      var args = PosixParser.DescribeProperties(target).Values.Distinct();
      var operand = args.FirstOrDefault(p => p.IsOperand);
      var opts =
        from arg in args
        where !arg.IsOperand
        orderby arg.Category, arg.Order
        group arg by arg.Category into g
        select new { Category = g.Key, Opts = g.ToArray() };

      WriteInfoLine(appTitle);
      if (header != null)
      {
        WriteInfoLine(header);
      }

      // Uso
      var usages = type.GetCustomAttributes(true).OfType<HelpUsageAttribute>();
      if (usages.Any())
      {
        WriteInfoLine("Uso:");
        foreach (var usage in usages)
        {
          WriteInfo("  ");
          WriteInfo(appName);
          WriteInfo(" ");
          WriteInfoLine(usage.Text, appName);
        }
      }
      else
      {
        WriteInfoLine("Uso:");
        WriteInfo("  ");
        WriteInfo(appName);
        WriteInfo(" ");

        if (opts.Any())
          WriteInfo("Opções ");

        if (operand != null)
          WriteInfo(operand.ArgName, appName);

        WriteInfoLine("");
      }

      // Seções adicionais
      foreach (var section in sections.Where(s => s.AfterUsage))
      {
        WriteInfoLine("");
        WriteInfo(section.Name);
        WriteInfoLine(":");
        foreach (var line in section.Text.Split('\n', '\r').Where(x => !string.IsNullOrEmpty(x)))
        {
          WriteInfo("  ");
          WriteInfoLine(line, appName);
        }
      }

      // Operando
      if (operand != null)
      {
        if (!string.IsNullOrEmpty(operand.Help))
        {
          WriteInfoLine("");
          WriteInfoLine("Operandos:");
          WriteInfo("  ");
          WriteInfoLine(operand.ArgName);
          foreach (var line in operand.Help.Split('\n', '\r').Where(x => !string.IsNullOrEmpty(x)))
          {
            WriteInfo("          ");
            WriteInfoLine(line, appName);
          }
        }
      }

      // Opções
      if (opts.Any())
      {
        if (opts.Any(o => string.IsNullOrEmpty(o.Category)))
        {
          WriteInfoLine("");
          WriteInfoLine("Opções:");
        }

        foreach (var category in opts)
        {
          if (!string.IsNullOrEmpty(category.Category))
          {
            WriteInfoLine("");
            WriteInfo(category.Category);
            WriteInfoLine(":");
          }
          foreach (var opt in category.Opts)
          {
            WriteInfo("  ");
            if (opt.Flag != null)
            {
              WriteInfo("-");
              WriteInfo(opt.Flag.ToString());
              WriteInfo(", ");
            }
            WriteInfo("--");
            WriteInfo(opt.Name);
            if (opt.HasArg)
            {
              WriteInfo(" ");
              WriteInfo(opt.ArgName, appName);
            }

            WriteInfoLine("");

            if (!string.IsNullOrEmpty(opt.Help))
            {
              foreach (var line in opt.Help.Split('\n', '\r').Where(x => !string.IsNullOrEmpty(x)))
              {
                WriteInfo("          ");
                WriteInfoLine(line, appName);
              }
            }
          }
        }
      }

      // Seçõs adicionais
      foreach (var section in sections.Where(s => !s.AfterUsage))
      {
        WriteInfoLine("");
        WriteInfo(section.Name);
        WriteInfoLine(":");
        foreach (var line in section.Text.Split('\n', '\r').Where(x => !string.IsNullOrEmpty(x)))
        {
          WriteInfo("  ");
          WriteInfoLine(line, appName);
        }
      }

      WriteInfoLine(footer);

      return 0;
    }

    private static string GetString(object host, MethodInfo method)
    {
      var value = method.Invoke(host, null);
      if (value is string)
        return (string)value;

      if (value is IEnumerable)
      {
        var lines = ((IEnumerable)value).Cast<string>();
        var text = string.Join(Environment.NewLine, lines);
        return text;
      }

      return value?.ToString();
    }
  }
}

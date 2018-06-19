using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;

namespace Toolset.Posix
{
  /// <summary>
  /// Interpretador de opções.
  /// </summary>
  public class PosixParser
  {
    /// <summary>
    /// Interpreta os argumentos de linha de comando e constrói o objeto de opções
    /// indicado de acordo com seus atributos.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto de opções.</typeparam>
    /// <param name="args">Os argumentos de linha de comando.</param>
    /// <returns>A instância do objeto de opções preenchido.</returns>
    public static T ParseArgs<T>(params string[] args)
      where T : new()
    {
      var target = new T();
      ParseArgs(target, args, false);
      return target;
    }

    /// <summary>
    /// Interpreta os argumentos de linha de comando e constrói o objeto de opções
    /// indicado de acordo com seus atributos.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto de opções.</typeparam>
    /// <param name="args">Os argumentos de linha de comando.</param>
    /// <param name="lenient">Ignora argumentos não esperados.</param>
    /// <returns>A instância do objeto de opções preenchido.</returns>
    public static T ParseArgs<T>(IEnumerable<string> args, bool lenient = false)
      where T : new()
    {
      var target = new T();
      ParseArgs(target, args, lenient);
      return target;
    }

    /// <summary>
    /// Interpreta os argumentos de linha de comando e constrói o objeto de opções
    /// indicado de acordo com seus atributos.
    /// </summary>
    /// <param name="target">A instância do objeto de opções preenchido.</param>
    /// <param name="args">Os argumentos de linha de comando.</param>
    /// <returns>A instância do objeto de opções preenchido.</returns>
    public static bool ParseArgs(object target, params string[] args)
    {
      return ParseArgs(target, args, false);
    }

    /// <summary>
    /// Interpreta os argumentos de linha de comando e constrói o objeto de opções
    /// indicado de acordo com seus atributos.
    /// </summary>
    /// <param name="target">A instância do objeto de opções preenchido.</param>
    /// <param name="args">Os argumentos de linha de comando.</param>
    /// <param name="lenient">Ignora argumentos não esperados.</param>
    public static bool ParseArgs(object target, IEnumerable<string> args, bool lenient = false)
    {
      var operands = args.SkipWhile(x => x != "--").Skip(1).ToArray();

      // nomes curtos aninhados devem ser quebrados
      // como em:
      //   -xyz
      // se torna:
      //   -x -y -z
      var opts = (
        from arg in args.TakeWhile(x => x != "--")
        let isGrouped = arg.Length > 2 && arg.StartsWith("-") && !arg.StartsWith("--")
        select isGrouped ? arg.Skip(1).Select(c => "-" + c) : arg.AsSingle()
      ).SelectMany(x => x).ToArray();

      var map = DescribeProperties(target);
      map.Values.ForEach(prop => prop.Reset());

      var optEnumerator = opts.Cast<string>().GetEnumerator();
      while (optEnumerator.MoveNext())
      {
        ParseOptOrOperand(map, optEnumerator, lenient);
      }

      var operandEnumerator = operands.Cast<string>().GetEnumerator();
      while (operandEnumerator.MoveNext())
      {
        ParseOperand(map, operandEnumerator, lenient);
      }

      return true;
    }

    public static Dictionary<string, Argument> DescribeProperties(object target)
    {
      if (target is Type)
        throw new InvalidUsageException($"Era esperado um objeto mas foi recebido um tipo: {((Type)target).FullName}");

      var type = target.GetType();
      var props = type.GetProperties();

      var map = new Dictionary<string, Argument>();
      foreach (var prop in props)
      {
        var propertyType = prop.PropertyType;
        if (!typeof(Opt).IsAssignableFrom(propertyType)) // O tipo deve ser Opt ou derivado
          continue;

        var info = new Argument();
        info.Order = int.MaxValue;
        info.Host = target;
        info.Accessor = prop;

        info.HasArg = typeof(OptArg).IsAssignableFrom(propertyType);

        var opt = prop.GetCustomAttributes(true).OfType<OptAttribute>().FirstOrDefault();
        if (opt != null)
        {
          info.Order = opt.Order;
          info.Flag = opt.ShortName;
          info.Name = opt.LongName;
          info.Category = opt.Category ?? string.Empty;
        }
        else if (prop.Name == "Help")
        {
          info.Flag = 'h';
          info.Name = "help";
        }
        info.Name = info.Name ?? prop.Name.Hyphenate();

        var arg = prop.GetCustomAttributes(true).OfType<OptArgAttribute>().FirstOrDefault();
        if (arg != null)
        {
          info.ArgName = arg.Name;
        }
        else if (info.HasArg)
        {
          info.ArgName = "VALOR";
        }

        var operand = prop.GetCustomAttributes(true).OfType<OperandAttribute>().FirstOrDefault();
        if (operand != null)
        {
          info.IsOperand = true;
          info.ArgName = string.IsNullOrEmpty(operand.Name) ? prop.Name : operand.Name;
        }

        var help = prop.GetCustomAttributes(true).OfType<HelpAttribute>().FirstOrDefault();
        if (help != null)
        {
          info.Help = help.Text;
        }
        if (info.Name == "help" && string.IsNullOrWhiteSpace(info.Help))
        {
          info.Help = "Imprime esta ajuda.";
        }

        if (info.Flag != null) map["-" + info.Flag] = info;
        map["--" + info.Name] = info;
      }
      return map;
    }

    private static void ParseOptOrOperand(Dictionary<string, Argument> map, IEnumerator<string> args, bool lenient)
    {
      var arg = args.Current;
      if (arg.StartsWith("-"))
      {
        var opt = map.ContainsKey(arg) ? map[arg] : null;
        if (opt == null)
        {
          if (!lenient)
            throw new InvalidUsageException($"Argumento não esperado: {arg}", null);
          return;
        }

        string value = null;
        if (opt.HasArg)
        {
          if (!args.MoveNext())
            throw new InvalidUsageException($"A opção espera um argumento: {arg}", null);

          value = args.Current;
          if (opt.ExpandEnvironmentVariables)
            value = Environment.ExpandEnvironmentVariables(value);
        }
        Set(opt, value);
      }
      else
      {
        ParseOperand(map, args, lenient);
      }
    }

    private static void ParseOperand(Dictionary<string, Argument> map, IEnumerator<string> args, bool lenient)
    {
      var operand = map.Values.Where(prop => prop.IsOperand).FirstOrDefault();
      if (operand == null)
      {
        if (!lenient)
          throw new InvalidUsageException($"Argumento não esperado: {args.Current}", null);
        return;
      }

      if (operand.Value.On && !operand.HasManyArgs)
      {
        if (!lenient)
          throw new InvalidUsageException($"Argumento não esperado: {args.Current}", null);
        return;
      }

      var value = args.Current;
      if (operand.ExpandEnvironmentVariables)
        value = Environment.ExpandEnvironmentVariables(value);

      Set(operand, args.Current);
    }

    private static void Set(Argument argument, string value)
    {
      var opt = argument.Value as Opt;
      var arg = argument.Value as OptArg;
      var array = argument.Value as OptArgArray;

      opt.On = true;
      if (arg != null)
        arg.Text = string.Join(" ", arg.Text.AsSingle().Where(x => x != null).Concat(value.AsSingle()));
      if (array != null)
        array.Items = (array.Items.Cast<string>() ?? Enumerable.Empty<string>()).Concat(value.Split(',')).ToArray();
    }
  }
}

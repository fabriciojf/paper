using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Specialized;

namespace Toolset.Text.Template
{
  internal static class ExpressionParser
  {
    private static readonly Regex bracesRegex = new Regex(
      @"
        {(                      # Captura {
          (
              [^{}]+            # Captura tudo exceto {}
              | (?<nivel>{)     # Se capturar { sobe um nivel.
              | (?<-nivel>})    # Se capturar } desce um nivel.
          )+                    # Repita
          (?(nivel)(?!))        # Enquanto nivel for diferente de zero
        )}                      # Captura }
        "
      , RegexOptions.IgnorePatternWhitespace
    );
    private static readonly Regex parenthesesRegex = new Regex(
      @"
        \((                     # Captura (
          (
              [^()]+            # Captura tudo exceto ()
              | (?<nivel>\()    # Se capturar ( sobe um nivel.
              | (?<-nivel>\))   # Se capturar ) desce um nivel.
          )+                    # Repita
          (?(nivel)(?!))        # Enquanto nivel for diferente de zero
        )\)                     # Captura )
        "
      , RegexOptions.IgnorePatternWhitespace
    );

    public static Expression Parse(string expression, ParseOptions options = ParseOptions.None)
    {
      if (options.HasFlag(ParseOptions.IgnoreWhitespace))
        expression = Regex.Replace(expression, @"\s", "");

      var nestMap = new Dictionary<char, string>();
      expression = MapTopLevelStatements(expression, nestMap);

      var instance = CreatePipeline(expression, nestMap);

      return instance;
    }

    private static Expression CreatePipeline(string expression, Dictionary<char, string> nestMap)
    {
      var pipeline = new Pipeline();

      var commands = (expression ?? "").Split('|');
      foreach (var command in commands)
      {
        var tokens = command.Split(';');
        var commandName = tokens.First();

        //
        // Mid
        //
        if (commandName.StartsWith("%["))
        {
          // formas:
          //   [1]
          //   [1;2]
          //   [-1]
          //   [-1;2]
          var text = command.Replace("%", "").Replace("[", "").Replace("]", "");
          var parts = text.Split(';');
          var start = parts.FirstOrDefault();
          var count = parts.Skip(1).FirstOrDefault();

          var startExpression = CreatePipeline(start, nestMap);
          var countExpression = CreatePipeline(count, nestMap);

          pipeline.Add(new MidExpression(startExpression, countExpression));
          continue;
        }

        //
        // Comandos com parametros
        //
        switch (commandName)
        {
          case "?":  // Ternary
            {
              var parameterTokens = tokens.Skip(1);
              var trueToken = parameterTokens.FirstOrDefault();
              var falseToken = parameterTokens.Skip(1).FirstOrDefault();

              var truePath = CreatePipeline(trueToken, nestMap);
              var falsePath = CreatePipeline(falseToken, nestMap);

              var expr = new TernaryExpression(truePath, falsePath);
              pipeline.Add(expr);
              continue;
            }

          case "%&":  // Concat
            {
              var concat = new ConcatExpression();
              var parameterTokens = tokens.Skip(1);
              foreach (var parameterToken in parameterTokens)
              {
                var parameter = CreatePipeline(parameterToken, nestMap);
                concat.Add(parameter);
              }
              pipeline.Add(concat);
              continue;
            }

          case "%r":  // Replace
            {
              var parameterTokens = tokens.Skip(1);
              var searchToken = parameterTokens.FirstOrDefault();
              var replacementToken = parameterTokens.Skip(1).FirstOrDefault();

              var search = CreateLiteral(searchToken, nestMap);
              var replacement = CreateLiteral(replacementToken, nestMap);

              var expr = new ReplaceExpression(search, replacement);
              pipeline.Add(expr);
              continue;
            }

          case "%s":  // Search
            {
              var parameterTokens = tokens.Skip(1);
              var searchToken = parameterTokens.FirstOrDefault();
              var replacementToken = parameterTokens.Skip(1).FirstOrDefault();

              var search = CreateLiteral(searchToken, nestMap);
              var replacement = 
                (replacementToken != null)
                  ? CreateLiteral(replacementToken, nestMap)
                  : null;

              var expr = new SearchExpression(search, replacement);
              pipeline.Add(expr);
              continue;
            }
        }

        //
        // Comparison
        //
        if (ComparisonExpression.KnownOperations.Contains(commandName))
        {
          var parameterTokens = tokens.Skip(1);
          var criteriaToken = parameterTokens.FirstOrDefault();
          var trueToken = parameterTokens.Skip(1).FirstOrDefault();
          var falseToken = parameterTokens.Skip(2).FirstOrDefault();

          var criteriaExpr = CreatePipeline(criteriaToken, nestMap);
          var trueExpr = CreatePipeline(trueToken, nestMap);
          var falseExpr = CreatePipeline(falseToken, nestMap);

          var expr = new ComparisonExpression(commandName, criteriaExpr, trueExpr, falseExpr);
          pipeline.Add(expr);
          continue;
        }

        //
        // Array e Coalesce
        //
        if (tokens.Length >= 2)
        {
          var coalesce = new CoalesceExpression();
          foreach (var token in tokens)
          {
            var parameter = CreatePipeline(token, nestMap);
            coalesce.Add(parameter);
          }
          pipeline.Add(coalesce);
          continue;
        }

        //
        // Expressão aninhada
        //
        if (Regex.IsMatch(commandName, "^§.$"))
        {
          var nestedIndex = commandName.Skip(1).FirstOrDefault();
          var nestedToken = nestMap.Get(nestedIndex);
          var nestedExpression = CreatePipeline(nestedToken, nestMap);
          pipeline.Add(nestedExpression);
          continue;
        }

        //
        // Property
        //
        if (commandName.StartsWith("$"))
        {
          var path = command.Substring(1);
          var getter = new GetterExpression(path);
          pipeline.Add(getter);
          continue;
        }

        //
        // Comandos sem parâmetros
        //
        switch (commandName)
        {
          case "%d":  // DataHora
            {
              pipeline.Add(new CastExpression(typeof(DateTime)));
              continue;
            }

          case "%b":  // Booliano
            {
              pipeline.Add(new CastExpression(typeof(bool)));
              continue;
            }

          case "%i":  // Inteiro
            {
              pipeline.Add(new CastExpression(typeof(int)));
              continue;
            }

          case "%f":  // Boolean
            {
              pipeline.Add(new CastExpression(typeof(float)));
              continue;
            }
        }

        //
        // Format
        //
        if (command.Contains("{"))
        {
          pipeline.Add(new FormatExpression(command));
          continue;
        }

        //
        // Literal
        //
        var literal = CreateLiteral(command, nestMap);
        pipeline.Add(literal);
      }

      return pipeline;
    }

    /// <summary>
    /// Cria uma expressão literal simples ou com expressões aninhadas.
    /// </summary>
    /// <param name="token">A expressão a ser avaliada.</param>
    /// <param name="nestMap">O mapa de expressões referenciadas.</param>
    /// <returns>A expressão literal</returns>
    private static Expression CreateLiteral(string token, Dictionary<char, string> nestMap)
    {
      //
      // Literal com expressão aninhada
      //
      if (token.Contains("§"))
      {
        // Temos uma expressão aninha dentro do literal
        //
        var parts = token.Split('§');
        var concat = new ConcatExpression();

        var expr = CreatePipeline(parts.First(), nestMap);
        concat.Add(expr);

        foreach (var part in parts.Skip(1))
        {
          var nestedIndex = part.FirstOrDefault();
          var nestedToken = nestMap.Get(nestedIndex);
          var nestedExpression = CreatePipeline(nestedToken, nestMap);
          concat.Add(nestedExpression);

          var text = new string(part.Skip(1).ToArray());
          concat.Add(new LiteralExpression(text));
        }

        return concat;
      }

      //
      // Literal
      //
      return new LiteralExpression(token);
    }

    private static string MapTopLevelStatements(string expression, Dictionary<char, string> nestMap)
    {
      var ingexGenerator = 'A';
      var matches = bracesRegex.Matches(expression);

      foreach (var match in matches.Cast<Match>().Reverse())
      {
        var currentIndex = ingexGenerator++;

        var nestedExpression = match.Groups[1].Value;
        nestedExpression = MapNestedStatements(nestedExpression, nestMap, ref ingexGenerator);

        nestMap.Add(currentIndex, nestedExpression);

        var replacement = (";§" + currentIndex + ";");
        expression = expression.Stuff(match.Index, match.Length, replacement);
      }

      expression = "%&;" + expression; // %& => concatenacao

      return expression;
    }

    private static string MapNestedStatements(string expression, Dictionary<char, string> nestMap, ref char ingexGenerator)
    {
      var matches = parenthesesRegex.Matches(expression);

      foreach (var match in matches.Cast<Match>().Reverse())
      {
        var index = ingexGenerator++;

        var nestedExpression = match.Groups[1].Value;
        nestedExpression = MapNestedStatements(nestedExpression, nestMap, ref ingexGenerator);

        nestMap.Add(index, nestedExpression);

        var replacement = ("§" + index);
        expression = expression.Stuff(match.Index, match.Length, replacement);
      }

      return expression;
    }

  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Toolset
{
  public static class StringExtensions
  {
    private static string delimiters = "_.:-";
    private static char[] delimitersChars = delimiters.ToArray();

    #region Conversions

    public static string RemoveDiacritics(this string text)
    {
      if (!string.IsNullOrWhiteSpace(text))
      {
        string stFormD = text.Normalize(NormalizationForm.FormD);
        int len = stFormD.Length;
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < len; i++)
        {
          System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[i]);
          if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
          {
            sb.Append(stFormD[i]);
          }
        }

        return (sb.ToString().Normalize(NormalizationForm.FormC));
      }
      else
      {
        return text;
      }
    }

    public static T To<T>(this string text)
    {
      return Change.To<T>(text);
    }

    public static T ToOrDefault<T>(this string text, T defaultValue = default(T))
    {
      try
      {
        return Change.To<T>(text);
      }
      catch
      {
        return defaultValue;
      }
    }

    public static MemoryStream ToStream(this string text)
    {
      var stream = new MemoryStream();
      using (var writer = new StreamWriter(stream, Encoding.UTF8, 512, true))
      {
        writer.Write(text);
        writer.Flush();
      }
      stream.Position = 0;
      return stream;
    }

    #endregion

    #region Name Conventions

    public static string ChangeCase(this string sentence, TextCase textCase)
    {
      if (sentence == null)
        return null;

      if (textCase == TextCase.Default || textCase == TextCase.KeepOriginal)
        return sentence;

      var canPrefix = !textCase.HasFlag(TextCase.NoPrefix);
      textCase &= ~TextCase.NoPrefix;

      var isPreserveData = textCase.HasFlag(TextCase.PreserveSpecialCharacters);
      textCase &= ~TextCase.PreserveSpecialCharacters;

      var isProperCase = (textCase == TextCase.ProperCase);
      if (isProperCase)
        textCase |= TextCase.Spaced;

      var preserved = isPreserveData ? @"?!#$%&@*+/\;<>=^~()[]{}" : "";

      sentence = Regex.Replace(sentence, $@"\s", " ");
      sentence = Regex.Replace(sentence, $@"[^\s0-9a-zA-ZÀ-ÿ{delimiters}{preserved}]", " ");
      if (isPreserveData)
        sentence = Regex.Replace(sentence, $@"([{preserved}])", " $1 ");
      if (!isProperCase)
        sentence = sentence.RemoveDiacritics();

      string prefix = null;
      string suffix = null;
      bool hasPrefix = false;
      if (canPrefix && !isProperCase)
      {
        prefix = string.Concat(sentence.TakeWhile(delimitersChars.Contains));
        suffix = string.Concat(sentence.Reverse().TakeWhile(delimitersChars.Contains).Reverse());
        hasPrefix = (prefix.Length > 0 || suffix.Length > 0);
      }

      var words = EnumerateWords(sentence);
      if (!words.Any())
        return string.Empty;

      var wordCaseMask = TextCase.UpperCase | TextCase.LowerCase | TextCase.ProperCase;
      var wordCase = textCase & wordCaseMask;

      var delimiterMask = TextCase.Hyphenated | TextCase.Underscore | TextCase.Dotted | TextCase.Spaced | TextCase.Joined;
      var delimiter = textCase & delimiterMask;

      // camel case recebe tratamento especial por este algoritmo
      var camelCaseMask = (int)(TextCase.CamelCase ^ TextCase.ProperCase ^ TextCase.Joined);
      var isCamelCase = (((int)textCase) & camelCaseMask) != 0;

      switch (wordCase)
      {
        case TextCase.UpperCase:
          words = words.Select(word => word.ToUpper());
          break;

        case TextCase.ProperCase:
          words = words.Select(word => char.ToUpper(word[0]) + word.Substring(1));
          break;
      }

      if (isCamelCase)
      {
        var firstWord = words.Take(1).Select(x => x.ToLower());
        var otherWords = words.Skip(1);
        words = firstWord.Concat(otherWords);
      }

      string text = null;

      if (delimiter.HasFlag(TextCase.Spaced))
      {
        text = string.Join(" ", words);
      }
      else if (delimiter.HasFlag(TextCase.Joined))
      {
        text = string.Concat(words);
      }
      else if (delimiter.HasFlag(TextCase.Hyphenated))
      {
        text = string.Join("-", words);
      }
      else if (delimiter.HasFlag(TextCase.Underscore))
      {
        text = string.Join("_", words);
      }
      else if (delimiter.HasFlag(TextCase.Dotted))
      {
        text = string.Join(".", words);
      }
      else 
      {
        text = string.Concat(words);
      }

      if (hasPrefix)
      {
        if (delimiter.HasFlag(TextCase.Hyphenated))
        {
          return '-'.Replicate(prefix.Length) + text + '-'.Replicate(suffix.Length);
        }
        else if (delimiter.HasFlag(TextCase.Underscore))
        {
          return '_'.Replicate(prefix.Length) + text + '_'.Replicate(suffix.Length);
        }
        else
        {
          return prefix + text + suffix;
        }
      }

      return text;
    }

    public static IEnumerable<string> EnumerateWords(this string sentence)
    {
      var characters = ReplaceDelimiters(sentence);
      var phrase = new string(characters.ToArray());
      return phrase.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x));
    }

    private static IEnumerable<char> ReplaceDelimiters(string sentence)
    {
      var previous = '\x0';
      foreach (var current in sentence.Trim())
      {
        if (delimitersChars.Contains(current))
        {
          yield return ' ';
        }
        else if (char.IsNumber(current))
        {
          if (!char.IsNumber(previous))
          {
            yield return ' ';
          }
          yield return current;
        }
        else if (char.IsUpper(current))
        {
          if (!char.IsUpper(previous))
          {
            yield return ' ';
          }
          yield return char.ToLower(current);
        }
        else
        {
          yield return current;
        }
        previous = current;
      }
    }

    #endregion

    #region Operations

    public static string Replicate(this char character, int times)
    {
      return new string(character, times);
    }

    public static string Replicate(this string text, int times)
    {
      return string.Concat(Enumerable.Range(0, times).Select(x => text));
    }

    public static string[] Split(this string text, string token)
    {
      var split = text.Split(new[] { token }, StringSplitOptions.RemoveEmptyEntries);
      return split;
    }

    public static string[] Split(this string text, params string[] tokens)
    {
      var split = text.Split(tokens, StringSplitOptions.RemoveEmptyEntries);
      return split;
    }

    /// <summary>
    /// Aplica uma substituição no texto usando regras de expressão regular.
    /// </summary>
    /// <param name="text">O texto a ser modificado.</param>
    /// <param name="regex">A expressão regular aplicada.</param>
    /// <param name="replacement">
    /// O texto substituto segundo as regeras de <see cref="Regex.Replace(string, string, string, RegexOptions)"/>
    /// </param>
    /// <param name="options">Opções de aplicação da expressão regular.</param>
    /// <returns>O texto com a substituição aplicada.</returns>
    public static string ReplacePattern(this string text, string regex, string replacement, RegexOptions options = default(RegexOptions))
    {
      return Regex.Replace(text, regex, replacement, options);
    }

    /// <summary>
    /// Extrai um trecho de uma sentença pela aplicação de uma expressão regular.
    /// 
    /// Suporta todas as substituições de Regex.Replace:
    /// -   $número     Grupo pelo índice.
    /// -   ${name}     Grupo pelo nome.
    /// -   $$          Literal "$".
    /// -   $&          Texto capturado inteiro.
    /// -   $`          Texto antes da captura.
    /// -   $'          Texto depois da captura.
    /// -   $+          Último grupo capturado.
    /// 
    /// Mais detalhes em:
    ///   https://msdn.microsoft.com/pt-br/library/ewy2t5e0(v=vs.110).aspx
    /// </summary>
    /// <param name="text">O texto a ser pesquisado./param>
    /// <param name="regex">A expressão regular.</param>
    /// <param name="replacement">
    /// Uma substituição opcional. De acordo com as regras de Regex.Replace.
    /// </param>
    /// <param name="options">Opções de aplicação da expressão regular.</param>
    /// <returns>O trecho extraído da string.</returns>
    public static string Strip(this string text, string regex, string replacement = null, RegexOptions options = default(RegexOptions))
    {
      options |= System.Text.RegularExpressions.RegexOptions.Multiline;

      text = text.Replace("\r", "");

      var match = System.Text.RegularExpressions.Regex.Match(text, regex, (RegexOptions)options);
      if (!match.Success)
        return null;

      text = match.Captures[0].Value;

      if (replacement == null) replacement = "$&";
      return System.Text.RegularExpressions.Regex.Replace(text, regex, replacement, (RegexOptions)options);
    }

    /// <summary>
    /// Substitui um invervalo de caracteres pelo texto indicado.
    /// </summary>
    /// <param name="text">O texto a ser modificado.</param>
    /// <param name="index">A posição inicial para mudança.</param>
    /// <param name="length">A quantidade de caracteres no invervalo.</param>
    /// <param name="replacement">O texto que será posto no lugar do invervalo.</param>
    /// <returns>O texto modificado.</returns>
    public static string Stuff(this string text, int index, int length, string replacement)
    {
      text = text.Substring(0, index) + replacement + text.Substring(index + length);
      return text;
    }

    #endregion

    #region Comparisons

    /// <summary>
    /// Implementação de comparação similar ao comando LIKE do SQL.
    /// Os caracteres tem significado especial no padrão comparado:
    /// - "%", qualquer quantidade de caracteres na posição.
    /// - "_", um único caractere qualquer na posição.
    /// </summary>
    /// <param name="text">O texto comparado.</param>
    /// <param name="pattern">O padrão pesquisado.</param>
    /// <returns>Verdadeiro se o padrão corresponde ao texto, falso caso contrário.</returns>
    public static bool Like(this string text, string pattern)
    {
      pattern = $"^{Regex.Escape(pattern).Replace("%", ".*").Replace("_", ".")}$";
      return Regex.IsMatch(text, pattern);
    }

    public static bool LikeIgnoreCase(this string text, string pattern)
    {
      pattern = $"^{Regex.Escape(pattern).Replace("%", ".*").Replace("_", ".")}$";
      return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
    }

    public static bool EqualsAny(this string text, params string[] terms)
    {
      return terms.Contains(text);
    }

    public static bool EqualsAnyIgnoreCase(this string text, params string[] terms)
    {
      return terms.Any(x => x.EqualsIgnoreCase(text));
    }

    public static bool EqualsAnyIgnoreCase(this string text, IEnumerable<string> terms)
    {
      return terms.Any(x => x.EqualsIgnoreCase(text));
    }

    public static bool EqualsIgnoreCase(this string text, string other)
    {
      if (text == null)
        return false;

      if (other == null)
        return false;

      return text.Equals(other, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool ContainsIgnoreCase(this string text, string pattern)
    {
      if (text == null)
        return false;

      var invariant = System.Globalization.CultureInfo.InvariantCulture;
      return invariant.CompareInfo.IndexOf(text, pattern) >= 0;
    }

    public static int IndexOfIgnoreCase(this string text, string pattern)
    {
      if (text == null)
        return -1;

      var invariant = System.Globalization.CultureInfo.InvariantCulture;
      return invariant.CompareInfo.IndexOf(text, pattern);
    }

    public static bool StartsWithIgnoreCase(this string text, string other)
    {
      if (text == null)
        return false;

      return text.StartsWith(other, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool EndsWithIgnoreCase(this string text, string other)
    {
      if (text == null)
        return false;

      return text.EndsWith(other, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool IsNullOrEmpty(this string text)
    {
      return string.IsNullOrEmpty(text);
    }

    public static bool IsNullOrWhiteSpace(this string text)
    {
      return string.IsNullOrWhiteSpace(text);
    }

    #endregion

  }
}

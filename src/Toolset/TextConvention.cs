using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Toolset
{
  /// <summary>
  /// Implementação das convenções de formatação de texto do Toolset.
  /// As convenções determinam as formas de conversão entre texto e valores.
  /// </summary>
  public static class TextConvention
  {
    [Flags]
    public enum Options
    {
      None,

      /// <summary>
      /// Ignora as convenções de encapsulamento de textos, datas e horas
      /// entre sinais de aspas e outros.
      /// </summary>
      /// <remarks>
      /// Por padrão, o algoritmo encapsula texto, data e hora entre aspas.
      /// Quando esta opção é ativada as aspas são omitidas.
      /// </remarks>
      IgnoreQuotes = 1,

      /// <summary>
      /// Desativa as convenções para valores nulo.
      /// </summary>
      /// <remarks>
      /// Por padrão, o algoritmo representa valores nulos como: null
      /// Quando esta opção é ativada, valores nulos não são representados como vazio.
      /// </remarks>
      IgnoreNull = 2,

      /// <summary>
      /// Desativa as convenções para valores booleanos.
      /// </summary>
      /// <remarks>
      /// Por padrão, o algoritmo representa valores boolianos como: true ou false
      /// Quando esta opção é ativada, valores nulos não são representados com o booliano
      /// convertido para string com ToString() e entre aspas.
      /// </remarks>
      IgnoreBool = 4,
    };

    /// <summary>
    /// Menor número possível.
    /// </summary>
    public const int MinNumber = -2000000000;

    /// <summary>
    /// Maior número possível.
    /// </summary>
    public const int MaxNumber = 2000000000;

    /// <summary>
    /// Convenção para número.
    /// -   Produz
    ///     -   Int32 para número inteiro.
    ///     -   Float para número decimal.
    ///     -   Precisão:
    ///         -   -2000000000 a 2000000000
    /// -   Formato
    ///     -   Até 10 casas decimais.
    ///     -   Separador de decimal pode ser ponto ou vírgula.
    ///         -   A função de formatação emite ponto por padrão.
    ///     -   O separador de decimal deve ter pelo menos um número antes e depois.
    ///         Portanto, valores como estes não são permitidos:
    ///         -   15.
    ///         -   .15
    ///     -   Separador de milhar não é permitido.
    /// </summary>
    private static readonly Regex NumberConvention =
      new Regex("^-?(?:20{9}|[0-1]?[0-9]{1,9})([.,][0-9]{1,10})?$");

    /// <summary>
    /// Convenção para booliano.
    /// -   Produz
    ///     -   Boolean
    /// -   Formato
    ///     -   true
    ///     -   false
    /// </summary>
    private static readonly Regex BooleanConvention =
      new Regex("^true|false$");

    /// <summary>
    /// Convenção para nulo.
    /// -   Produz
    ///     -   Um valor nulo.
    /// -   Formato
    ///     -   null
    ///     -   (null)
    /// </summary>
    private static readonly Regex NullConvention =
      new Regex("^null$");

    /// <summary>
    /// Convenção para datas.
    /// -   Produz
    ///     -   DateTime
    /// -   Formato
    ///     -   yyyy-MM-ddTHH:mm:sszzz
    ///         -   "T" separa a porção da data da porção da hora.
    ///         -   "zzz" corresponde ao timezone.
    ///         -   Exemplo:
    ///             -   2018-12-31
    ///             -   2018-12-31T12:59
    ///             -   2018-12-31T12:59:59
    ///             -   2018-12-31T12:59:59-03:00
    ///             -   2018-12-31T12:59:59+05:00
    ///     -   Segundos podem ser omitidos.
    ///     -   Timezone pode ser omitido.
    ///     -   Segundos não podem ser omitidos quando timezone está presente
    ///     -   A data pode existir isoladamente.
    /// </summary>
    private static readonly Regex DateTimeConvention =
      new Regex("^([0-9]{4}-[0-9]{2}-[0-9]{2})T?([0-9]{2}:[0-9]{2}(?::[0-9]{2}(?:[+-][0-9]{2}:[0-9]{2})?)?)?$");

    /// <summary>
    /// Convenção para TimeSpan.
    /// -   Produz
    ///     -   TimeSpan
    /// -   Formato
    ///     -   d.HH:mm:ss.mmm
    ///         -   "mmm" representa milisegundos com precisão de 3 a 7 casas.
    ///     -   A hora deve estar no formato de 24horas.
    ///     -   Os dias podem ser omitidos.
    ///     -   Os segundos podem ser omitidos.
    ///     -   Os milisegundos podem ser omitidos.
    ///     -   Se os milisegundos estiverem presentes os segundos também devem estar.
    ///     -   Exemplos:
    ///         -   1.23:59:59.999
    ///         -   1.23:59:59
    ///         -   1.23:59
    ///         -   23:59:59.999
    ///         -   23:59:59
    ///         -   23:59
    /// </summary>
    private static readonly Regex TimeSpanConvention =
      new Regex("^(?:[0-9]+[.])?(?:[0-9]{2}:[0-9]{2})(?::[0-9]{2}(?:[.][0-9]{1,7})?)?$");

    /// <summary>
    /// Emite o valor indicado no formato de texto convencionado.
    /// Textos, datas e horas são colocados entre aspas.
    /// </summary>
    /// <param name="value">O valor a ser formatado.</param>
    /// <returns>O valor formatado para string conforme a convenção.</returns>
    public static string ToString(object value)
    {
      return ToString(value, '"', '"', Options.None);
    }

    /// <summary>
    /// Emite o valor indicado no formato de texto convencionado.
    /// Textos, datas e horas são colocados entre aspas, a menos que desativado
    /// com a opção Options.NoQuote.
    /// </summary>
    /// <param name="value">O valor a ser formatado.</param>
    /// <param name="options">Opções de formatação do texto.</param>
    /// <returns>O valor formatado para string conforme a convenção.</returns>
    public static string ToString(object value, Options options)
    {
      return ToString(value, '"', '"', options);
    }

    /// <summary>
    /// Emite o valor indicado no formato de texto convencionado.
    /// Textos, datas e horas são colocados entre sinais do tipo indicado.
    /// </summary>
    /// <param name="value">O valor a ser formatado.</param>
    /// <param name="quoteSign">Marca de início e término de texto.</param>
    /// <returns>O valor formatado para string conforme a convenção.</returns>
    public static string ToString(object value, char quoteSign)
    {
      return ToString(value, quoteSign, quoteSign, Options.None);
    }

    /// <summary>
    /// Emite o valor indicado no formato de texto convencionado.
    /// Textos, datas e horas são colocados entre sinais do tipo indicado, a menos que desativado
    /// com a opção Options.NoQuote.
    /// </summary>
    /// <param name="value">O valor a ser formatado.</param>
    /// <param name="quoteSign">Marca de início e término de texto.</param>
    /// <param name="options">Opções de formatação do texto.</param>
    /// <returns>O valor formatado para string conforme a convenção.</returns>
    public static string ToString(object value, char quoteSign, Options options)
    {
      return ToString(value, quoteSign, quoteSign, options);
    }

    /// <summary>
    /// Emite o valor indicado no formato de texto convencionado.
    /// </summary>
    /// <param name="value">O valor a ser formatado.</param>
    /// <param name="startQuoteSign">Marca de início de texto.</param>
    /// <param name="endQuoteSign">Marca de término de texto.</param>
    /// <returns>O valor formatado para string conforme a convenção.</returns>
    public static string ToString(object value, char startQuoteSign, char endQuoteSign)
    {
      return ToString(value, startQuoteSign, endQuoteSign, Options.None);
    }

    /// <summary>
    /// Emite o valor indicado no formato de texto convencionado.
    /// Textos, datas e horas são colocados entre sinais dos tipos indicados, a menos que desativado
    /// com a opção Options.NoQuote.
    /// </summary>
    /// <param name="value">O valor a ser formatado.</param>
    /// <param name="startQuoteSign">Marca de início de texto.</param>
    /// <param name="endQuoteSign">Marca de término de texto.</param>
    /// <param name="options">Opções de formatação do texto.</param>
    /// <returns>O valor formatado para string conforme a convenção.</returns>
    public static string ToString(object value, char startQuoteSign, char endQuoteSign, Options options)
    {
      var noQuote = options.HasFlag(Options.IgnoreQuotes);
      var ignoreNull = options.HasFlag(Options.IgnoreNull);
      var ignoreBool = options.HasFlag(Options.IgnoreBool);

      if (value == null)
        return ignoreNull ? null : "null";

      if (value is bool)
        if (!ignoreBool)
          return ((bool)value) ? "true" : "false";

      if (value is short
       || value is int
       || value is float
       || value is double
       || value is decimal
       || value is decimal)
      {
        return value.ToString();
      }

      // o tipo 'long' quando maior que o maximo inteiro suportado pela convenção
      // deve ser renderizado como texto.
      if (value is long)
      {
        var number = (long)value;
        if (number >= MinNumber && number <= MaxNumber)
          return value.ToString();
      }

      var start = noQuote ? "" : startQuoteSign.ToString();
      var end = noQuote ? "" : endQuoteSign.ToString();

      if (value is DateTime)
        return start + ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:sszzz") + end;

      return start + value.ToString() + end;
    }

    /// <summary>
    /// Obtém o valor apropriado para o texto representado segundo a convenção.
    /// 
    /// Textos encapsulados entre aspas ou apóstrofos são traduzidos como string e
    /// tem as aspas ou apóstrofos removidos.
    /// Esta forma pode ser usada, por exemplo, para forçar a tradução de números
    /// como string.
    /// 
    /// Por exemplo, este texto é traduzir para inteiro:
    /// 
    ///   var value = ToValue("123");
    ///   // value é int
    ///   
    /// Para forçar a tradução do texto como string aspas ou apóstrofos podem ser
    /// usados desta forma:
    /// 
    ///   var value = ToValue("'123'");
    ///   // value é string
    ///   
    /// </summary>
    /// <param name="text">O texto a ser processado.</param>
    /// <returns>O valor obtido do texto segundo a convenção.</returns>
    public static object ToValue(string text)
    {
      return ToValue(text, Options.None);
    }

    /// <summary>
    /// Obtém o valor apropriado para o texto representado segundo a convenção.
    /// 
    /// Textos encapsulados entre aspas ou apóstrofos são traduzidos como string e
    /// tem as aspas ou apóstrofos removidos.
    /// Esta forma pode ser usada, por exemplo, para forçar a tradução de números
    /// como string.
    /// 
    /// Por exemplo, este texto é traduzir para inteiro:
    /// 
    ///   var value = ToValue("123");
    ///   // value é int
    ///   
    /// Para forçar a tradução do texto como string aspas ou apóstrofos podem ser
    /// usados desta forma:
    /// 
    ///   var value = ToValue("'123'");
    ///   // value é string
    ///   
    /// </summary>
    /// <param name="text">O texto a ser processado.</param>
    /// <param name="options">Opções de interpretação do texto.</param>
    /// <returns>O valor obtido do texto segundo a convenção.</returns>
    public static object ToValue(string text, Options options)
    {
      if (text == null)
        return null;

      Match match;

      var noQuote = options.HasFlag(Options.IgnoreQuotes);
      var ignoreNull = options.HasFlag(Options.IgnoreNull);
      var ignoreBool = options.HasFlag(Options.IgnoreBool);

      //
      // Conversões para textos NÃO entre aspas
      //

      if (!ignoreNull)
      {
        match = NullConvention.Match(text);
        if (match.Success)
          return null;
      }

      if (!ignoreBool)
      {
        match = BooleanConvention.Match(text);
        if (match.Success)
          return text == "true";
      }

      match = NumberConvention.Match(text);
      if (match.Success)
      {
        var hasDecimal = match.Groups[1].Length > 0;
        if (hasDecimal)
          return float.Parse(text);
        else
          return int.Parse(text);
      }

      //
      // Retirando o texto das aspas
      //

      if (!noQuote)
      {
        if ((text.First() == '"' && text.Last() == '"') || (text.First() == '\'' && text.Last() == '\''))
          text = text.Substring(1, text.Length - 2);
      }

      //
      // Conversões para textos quem podem existir entre aspas
      //

      match = DateTimeConvention.Match(text);
      if (match.Success)
        return DateTime.Parse(text);

      match = TimeSpanConvention.Match(text);
      if (match.Success)
        return TimeSpan.Parse(text);

      return text;
    }

    /// <summary>
    /// Obtém o valor apropriado para o texto representado segundo a convenção.
    /// Em caso de falha retorna o valor como recebido.
    /// 
    /// Textos encapsulados entre aspas ou apóstrofos são traduzidos como string e
    /// tem as aspas ou apóstrofos removidos.
    /// Esta forma pode ser usada, por exemplo, para forçar a tradução de números
    /// como string.
    /// 
    /// Por exemplo, este texto é traduzir para inteiro:
    /// 
    ///   var value = ToValue("123");
    ///   // value é int
    ///   
    /// Para forçar a tradução do texto como string aspas ou apóstrofos podem ser
    /// usados desta forma:
    /// 
    ///   var value = ToValue("'123'");
    ///   // value é string
    ///   
    /// </summary>
    /// <param name="text">O texto a ser processado.</param>
    /// <returns>O valor obtido do texto segundo a convenção.</returns>
    public static object ToValueOrDefault(string text)
    {
      try
      {
        return ToValue(text, Options.None);
      }
      catch
      {
        return text;
      }
    }

    /// <summary>
    /// Obtém o valor apropriado para o texto representado segundo a convenção.
    /// Em caso de falha retorna o valor como recebido.
    /// 
    /// Textos encapsulados entre aspas ou apóstrofos são traduzidos como string e
    /// tem as aspas ou apóstrofos removidos.
    /// Esta forma pode ser usada, por exemplo, para forçar a tradução de números
    /// como string.
    /// 
    /// Por exemplo, este texto é traduzir para inteiro:
    /// 
    ///   var value = ToValue("123");
    ///   // value é int
    ///   
    /// Para forçar a tradução do texto como string aspas ou apóstrofos podem ser
    /// usados desta forma:
    /// 
    ///   var value = ToValue("'123'");
    ///   // value é string
    ///   
    /// </summary>
    /// <param name="text">O texto a ser processado.</param>
    /// <param name="options">Opções de interpretação do texto.</param>
    /// <returns>O valor obtido do texto segundo a convenção.</returns>
    public static object ToValueOrDefault(string text, Options options)
    {
      try
      {
        return ToValue(text, options);
      }
      catch
      {
        return text;
      }
    }
  }
}

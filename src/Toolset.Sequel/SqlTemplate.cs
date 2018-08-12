using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Collections;

namespace Toolset.Sequel
{
  /// <summary>
  /// Algoritmos para aplicação de templates na string SQL.
  /// </summary>
  public static class SqlTemplate
  {
    /// <summary>
    /// Realiza a aplicação do template.
    /// 
    /// Por padrão, todos os parâmetros atribuídos são estocados na instância
    /// de Sql e aplicados à string SQL apenas no ato da execução da SQL.
    /// 
    /// Este método aplica o template imediatamente, modificando a SQL original,
    /// a menos que desativado pela configuração SequelSettings.QueryTemplateEnabled.
    /// 
    /// Os parâmetros são aplicados ao template de duas formas:
    /// 
    /// 1.  Posicional
    ///     Com ouso de String.Format parâmetros podem ser adicionados à string
    ///     na forma {0}, {1}, etc.
    ///     
    /// 2.  Nomeado
    ///     Parâmetros nomeados podem aparecer na string na forma @{nome}.
    ///     
    /// Exemplo:
    ///     var texto = "select * from {0} where @{campo} = @valor";
    ///     var sql = texto.AsSql();
    ///     sql.Format("usuario");
    ///     sql.Set("campo", "login");
    ///     sql.Set("valor", "Fulano");
    ///     sql.ApplyTemplate();
    ///     
    ///     Resultado:
    ///       select * from usuario where login = @valor
    ///     
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static Sql ApplyTemplate(this Sql sql)
    {
      return ApplyTemplate(sql, false);
    }

    /// <summary>
    /// Realiza a aplicação do template.
    /// 
    /// Por padrão, todos os parâmetros atribuídos são estocados na instância
    /// de Sql e aplicados à string SQL apenas no ato da execução da SQL.
    /// 
    /// Este método aplica o template imediatamente, modificando a SQL original,
    /// a menos que desativado pela configuração SequelSettings.QueryTemplateEnabled.
    /// 
    /// Os parâmetros são aplicados ao template de duas formas:
    /// 
    /// 1.  Posicional
    ///     Com ouso de String.Format parâmetros podem ser adicionados à string
    ///     na forma {0}, {1}, etc.
    ///     
    /// 2.  Nomeado
    ///     Parâmetros nomeados podem aparecer na string na forma @{nome}.
    ///     
    /// Exemplo:
    ///     var texto = "select * from {0} where @{campo} = @valor";
    ///     var sql = texto.AsSql();
    ///     sql.Format("usuario");
    ///     sql.Set("campo", "login");
    ///     sql.Set("valor", "Fulano");
    ///     sql.ApplyTemplate();
    ///     
    ///     Resultado:
    ///       select * from usuario where login = @valor
    ///     
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="force">
    /// Força a aplicação do template mesmo que esteja desabilitado na configuração
    /// SequelSettings.QueryTemplateEnabled.
    /// </param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static Sql ApplyTemplate(this Sql sql, bool force)
    {
      if (!force && !SequelSettings.QueryTemplateEnabled)
        return sql;

      if (SequelSettings.StandardTemplateEnabled)
        ApplyStandardTemplate(sql);

      if (SequelSettings.ExtendedTemplateEnabled)
        ApplyExtendedTemplate(sql);

      return sql;
    }

    /// <summary>
    /// Aplica o template simplificado de SQL.
    /// O template suporta a sintaxe:
    /// 
    /// -   @{parametro}
    /// 
    /// Para realizar substituição no texto.
    /// 
    /// Por exemplo, um nome de tabela pode ser definido dinamicamente e declarado
    /// no template como:
    /// 
    ///     SELECT * FROM @{nome_tabela} WHERE id = @id_tabela
    /// 
    /// </summary>
    /// <param name="sql">The SQL.</param>
    public static void ApplyStandardTemplate(Sql sql)
    {
      var text = sql.ToString();

      var names =
        from name in sql.ParameterNames
        orderby name.Length descending, name
        select name;
      foreach (var name in names)
      {
        var value = Commander.CreateSqlCompatibleValue(sql[name]);
        var key = "@{" + name + "}";
        var keyValue = value.ToString();
        text = text.Replace(key, keyValue);
      }

      sql.Text = text;
    }

    /// <summary>
    /// Aplicação do template estendido.
    /// 
    /// 
    /// Visão Geral
    /// -----------
    /// 
    /// Template estendido é uma reescrita da cláusula WHERE de uma SQL
    /// com base no tipo do dado passado para o parâmetro.
    /// 
    /// O tipo do dado passado para o parâmetro é classificado como:
    /// 
    /// -   SmartString
    ///     -   Quando o valor corresponde a um texto.
    /// -   SmartArray
    ///     -   Quando o valor corresponde a uma coleção de qualquer tipo.
    /// -   SmartRange
    ///     -   Quando o valor corresponde a um objeto com as propriedades:
    ///         -   From e To
    ///         -   Start e End
    ///         -   Min e Max
    ///     -   Qualquer objeto passado para o parâmetro que contenha pelo menos
    ///         uma das propriedades acima é considerado SmartRange
    ///     -   Quando o objeto contém apenas a propriedade menor, isto é,
    ///         From, Start ou Min, considera-se um limite "maior ou igual a".
    ///     -   Quando o objeto contém apenas a propriedade maior, isto é,
    ///         To, End ou Max, considera-se um limite "menor ou igual a".
    ///     -   O nome da propriedade é insensível a caso.
    ///     -   Objeto anônimo também pode ser usado.
    /// -   Original
    ///     -   Quando o valor não se encaixa nos tipos acima.
    /// 
    /// O template estendido oferece dois operadores especiais que, quando usados,
    /// produzem uma reescrita da condição. São eles:
    /// 
    /// -   O operator "IS SET"
    /// -   O operador "MATCHES"
    ///     
    /// O operador "is set" é usado para determinar se um valor foi atribuído 
    /// a um parâmetro da SQL, permitindo a SQL se comportar de forma diferente
    /// quando o parâmetro é passado e quando não.
    /// 
    /// O operador "matches" permite a comparação com os diferentes tipos de dados
    /// pela reescrita da SQL. Isto evita a construção dinâmica da SQL para refletir
    /// a escolha de parâmetros.
    /// 
    /// Por exemplo: 
    /// -   Se o valor do parâmetro for um texto uma comparação de igualdade é aplicada.
    /// -   Se o valor contém os curingas "*" ou "?" uma comparação com "LIKE" é aplicada.
    /// -   Se o valor for um vetor de textos uma comparação com "IN" é aplicada.
    /// 
    /// 
    /// Sintaxe: IS SET
    /// ---------------
    /// 
    /// Forma geral:
    /// 
    /// -   @parametro IS [NOT] SET
    ///     
    /// Checa se um valor foi passado para um parâmetro.
    /// 
    /// Exemplos:
    ///     
    ///     --  Se o parametro "id" for informado apenas este usuário será retornado,
    ///     --  esteja ele ativo ou não, caso contrário todos os usuarios ativos serão
    ///     --  retornados.
    ///     SELECT *
    ///       FROM usuario
    ///      WHERE (@id IS SET AND id = @id)
    ///         OR (@id IS NOT SET AND ativo = 1)
    /// 
    /// 
    /// Sintaxe: MATCHES
    /// ----------------
    /// 
    /// Forma geral:
    /// 
    /// -   alvo [NOT] MATCHES [IF SET] @parametro
    /// 
    /// O operador "MATCHES" é um operador especial implementado pelo template para a
    /// reescrita da condição de acordo com o tipo do parâmetro.
    /// 
    /// A correspondência entre tipo e reescrita segue a seguinte forma:
    /// 
    /// -   Para SmartString
    /// 
    ///     -   Quando contém os curingas "*" e "?":
    ///         -   [NOT] alvo LIKE @parametro
    ///         -   O curinga "*" é substituído para "%".
    ///         -   O curinga "?" é substituído para "_".
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = "*FILOP?TOR"
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE nome LIKE @nome
    ///                 // sendo @nome = '%FILOP_TOR'
    /// 
    ///     -   Para os demais casos:
    ///         -   [NOT] alvo = @parametro
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = "TEA FILOPÁTOR"
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE nome = @nome
    /// 
    /// -   Para SmartArray
    ///     -   [NOT] alvo IN (valor1, valor2, ...)
    ///     -   O array é transformado em uma lista.
    ///     -   Em caso de array de strings cada string é encapsulada entre apóstrofos.
    ///     -   Exemplo:
    ///     
    ///             SELECT * FROM usuario WHERE nome MATCHES @nome
    ///             // sendo @nome = new [] { "TEA FILOPÁTOR", "FIDÍPEDES" }
    ///             // produz:
    ///             SELECT * FROM usuario WHERE nome IN ('TEA FILOPÁTOR','FIDÍPEDES')
    /// 
    /// -   Para SmartRange
    /// 
    ///     -   Quando o menor e o maior valor são indicados:
    ///         -   [NOT] alvo BETWEEN @min AND @max
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = new { min = "TEA FILOPÁTOR", max = "FIDÍPEDES" }
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE nome BETWEEN @nome_1 AND @nome_2
    ///             
    ///     -   Quando apenas o menor valor é indicado:
    ///         -   [NOT] alvo &gt;= @min
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = new { min = "TEA FILOPÁTOR" }
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE nome &gt;= @nome_min
    ///                 
    ///     -   Quando apenas o maior valor é indicado:
    ///         -   [NOT] alvo &lt;= @max
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = new { max = "TEA FILOPÁTOR" }
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE nome &lt;= @nome_max
    ///     
    /// -   Para valores nulo:
    /// 
    ///     -   Quando "IF SET" é indicado:
    ///         -   1=1
    ///         -   Isto faz com que a condição seja simplesmente ignorada.
    ///         -   Ou seja, ou um valor é indicado e a comparação é executada ou um 
    ///             valor não é indicado e a comparação é ignorada.
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES IF SET @nome
    ///                 // sendo @nome = null
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE 1=1
    ///             
    ///     -   Quando "IF SET" não é indicado:
    ///         -   1=0
    ///         -   Isto faz com que a condição sempre falhe.
    ///         -   Ou seja, ou um valor é indicado e a comparação é executada ou um
    ///             valor não é indicado e a comparação é considerada falsa.
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = null
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE 1=0
    /// 
    /// A cláusula "IF SET" determina o comportamento da comparação quando o valor do
    /// parâmetro não é atribuído:
    /// 
    /// -   Se "IF SET" está ausente a condição é considerada obrigatória e será resolvida como
    ///     falso caso nenhum valor seja atribuído ao parâmetro.
    ///     
    ///     Por exemplo, a instrução:
    ///     -   id MATCHES @id
    ///
    ///     Corresponde a uma condição como:
    ///     -   id = @id
    /// 
    /// -   Se "IF SET" está presente a condição é considerada opcional e será resolvida como
    ///     verdadeiro caso nenhum valor seja atribuído ao parâmetro.
    ///     
    ///     Por exemplo, a instrução:
    ///     -   id MATCHES IF SET @id
    ///
    ///     Corresponde a uma condição parecida com:
    ///     -   (@id IS NULL OR id = @id)
    /// 
    /// 
    /// Sintaxe: MANY MATCHES
    /// ---------------------
    /// 
    /// Forma geral:
    /// 
    /// -   MANY [@parametro] [NOT] MATCHES [IF SET] ( ... )
    /// 
    /// O operador "MANY MATCHES" é um operador especial implementado pelo template para a
    /// reescrita de um bloco com conjuntos diferentes de parâmetros.
    /// 
    /// O parâmetro deve ser uma coleção de coleção de argumentos, isto é:
    /// -   Cada entrada da coleção definida para o parâmetro deve haver uma coleção de argumentos.
    /// 
    /// O algoritmo reescreve o bloco para cada entrada encontrada no mapa, repassando para o
    /// construtor do bloco os parâmetros obtidos daquela entrada.
    /// 
    /// Por exemplo, considere a instrução
    /// 
    ///     WHERE MANY @usuarios MATCHES (nome = @nome AND sobrenome = @sobrenome)
    ///     
    /// E considere a seguinte coleção definida para o parâmetro @usuarios:
    /// 
    ///     object[] mapa =
    ///     {
    ///       new { nome = "fulano", sobrenome = "silva" },
    ///       new { nome = "beltrano", sobrenome = "silva" }
    ///     };
    /// 
    /// A instrução seria reescrita como
    /// 
    ///     WHERE (
    ///       (nome = 'fulano' AND sobrenome = 'silva')
    ///       OR
    ///       (nome = 'beltrano' AND sobrenome = 'silva')
    ///     )
    ///     
    /// A cláusula "IF SET" determina o comportamento da comparação quando o valor do
    /// parâmetro não é atribuído:
    /// 
    /// -   Se "IF SET" está ausente a condição é considerada obrigatória e será resolvida como
    ///     falso caso nenhum valor seja atribuído ao parâmetro.
    ///     
    /// -   Se "IF SET" está presente a condição é considerada opcional e será resolvida como
    ///     verdadeiro caso nenhum valor seja atribuído ao parâmetro.
    ///     
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    public static void ApplyExtendedTemplate(Sql sql)
    {
      sql.Text = ReplaceTemplates(sql.Text, sql.Parameters, new KeyGen());
    }

    /// <summary>
    /// Aplica a substituição dos parâmetros para construção da instrução SQL definitiva.
    /// </summary>
    /// <param name="text">O texto que sofrerá a substituição.</param>
    /// <param name="args">Argumentos disponíveis.</param>
    /// <param name="keyGen">Algoritmo de geração de nomes de parâmetros.</param>
    /// <returns></returns>
    private static string ReplaceTemplates(string text, IDictionary<string, object> args, KeyGen keyGen)
    {
      text = ReplaceManyMatches(text, args, keyGen);

      var extendedParameters = ExtractKnownExtendedParameters(text).ToArray();
      var names =
        from name in extendedParameters
        orderby name.Length descending, name
        select name;

      foreach (var name in names)
      {
        var value = args.Get(name);
        var criteria = CreateCriteria(name, value, args, keyGen);
        text = ReplaceMatches(text, name, criteria);
      }
      return text;
    }

    /// <summary>
    /// Aplicação do template estendido para a sintaxe MANY MATCHES:
    /// -   MANY [@parametro] [NOT] MATCHES [IF SET] ( ... )
    /// </summary>
    /// <param name="text">O texto que sofrerá a substituição.</param>
    /// <param name="args">Argumentos disponíveis.</param>
    /// <param name="keyGen">Algoritmo de geração de nomes de parâmetros.</param>
    /// <returns></returns>
    private static string ReplaceManyMatches(string text, IDictionary<string, object> args, KeyGen keyGen)
    {
      // Aplicando a substituição da instrução:
      //   many [target] [not] matches [if set] ()
      var regex = new Regex(@"[(\s]many\s+(?:@([\w]+)\s+)?(?:(not)\s+)?matches(?:\s+(if\s+set))?\s*?(?:([a-zA-Z_.]+)|([a-zA-Z0-9_]*[(](?>[(](?<c>)|[^()]+|[)](?<-c>))*(?(c)(?!))[)]))");
      var matches = regex.Matches(text);
      foreach (Match match in matches.Reverse())
      {
        string replacement = "";

        // Exemplos de textos extraidos
        //
        // " many matches ( ... ) "
        //   [1] ""
        //   [2] ""
        //   [3] ""
        //   -inutil-
        //   [5] "( ... )"
        //
        // " many @parametro not matches if set ( ... ) "
        //   [1] "parametro"
        //   [2] "not"
        //   [3] "if set"
        //   -inutil-
        //   [5] "( ... )"

        var bagName = match.Groups[1].Value;
        var isNot = (match.Groups[2].Length > 0);
        var isOptional = (match.Groups[3].Length > 0);
        var body = match.Groups[5].Value;
        body = body.Substring(1, body.Length - 2);

        var candidate = args.Get(bagName);
        var enumerable = (candidate as IEnumerable) ?? ((candidate as Any)?.Value as IEnumerable);
        var bags = enumerable?.OfType<IDictionary<string, object>>();
        if (bags?.Any() != true)
        {
          replacement = isOptional ? "1=1" : "1=0";
        }
        else
        {
          var sentences = new List<string>();
          foreach (var bag in bags)
          {
            var sentence = body;

            var nestGen = keyGen.Derive();
            foreach (var key in bag.Keys)
            {
              var matches2 = Regex.Matches(sentence, $@"@({key}\w?)");
              foreach (Match match2 in matches2.Reverse())
              {
                var matchKey = match2.Groups[1].Value;
                if (matchKey != key)
                  continue;

                var index = match2.Groups[1].Index;
                var count = match2.Groups[1].Length;

                var keyName = nestGen.DeriveName(key);
                var keyValue = bag[key];

                args[keyName] = keyValue;
                sentence = sentence.Stuff(index, count, keyName);
              }
            }

            sentence = ReplaceTemplates(sentence, args, nestGen);
            sentence = $"{(isNot ? " not " : "")}({sentence})";
            sentences.Add(sentence);
          }

          replacement = $" ({string.Join(" or ", sentences)})";
        }

        text = text.Stuff(match.Index, match.Length, $" {replacement}");
      }
      return text;
    }

    /// <summary>
    /// Constrói a condição que substituirá o template baseado na instrução:
    /// - target [always] matches @parametro
    /// 
    /// Se o valor do parâmetro não tiver sido definido o critério retornado será nulo.
    /// 
    /// O critério criado contém o termo "{0}" que deve ser substituído pela
    /// parte "target" à direta da instrução matches.
    /// 
    /// Por exemplo, em:
    ///   Campo matches @nome
    ///   
    /// O critério produzir pode ser algo como:
    ///   {0} like @nome
    /// 
    /// Que deve então ser substituído com String.Format() pela parte à direta
    /// da instrução, neste caso "Campo":
    ///   String.Format("{0} like @nome", "Campo");
    ///   
    /// Produzindo como resultado:
    ///   Campo like @nome
    /// </summary>
    /// <param name="parameter">Nome do parâmetro que será substituído.</param>
    /// <param name="value">Valor atribuído ao parâmetro.</param>
    /// <param name="args">Argumentos disponíveis.</param>
    /// <param name="keyGen">Algoritmo de geração de nomes de parâmetros.</param>
    /// <returns>A condição que substituirá o template.</returns>
    private static string CreateCriteria(string parameter, object value, IDictionary<string, object> args, KeyGen keyGen)
    {
      if (value == null)
      {
        return null;
      }

      var any = value as Any;
      var raw = (any != null) ? any.Raw : value;

      if (raw is Sql)
      {
        var nestSql = (value as Sql) ?? (any?.Value as Sql);
        var nestGen = keyGen.Derive();
        var nestArgs = nestSql.Parameters;
        var nestText = nestSql.Text + "\n";

        // cada parametro recebera um sufixo para diferenciacao de parametros
        // que já existem na instância de Sql.
        foreach (var key in nestArgs.Keys)
        {
          var matches = Regex.Matches(nestText, $@"@({key}\w?)");
          foreach (Match match in matches.Reverse())
          {
            var matchKey = match.Groups[1].Value;
            if (matchKey != key)
              continue;

            var index = match.Groups[1].Index;
            var count = match.Groups[1].Length;

            var keyName = nestGen.DeriveName(key);
            var keyValue = nestArgs[key];

            args[keyName] = keyValue;
            nestText = nestText.Stuff(index, count, keyName);
          }
        }

        return "{0} in (" + nestText  + ")";
      }

      if (raw != null)
      {
        if (raw.GetType().IsValueType || raw is string)
        {
          args[parameter] = value;
          return "{0} = @" + parameter;
        }
        else
        {
          return null;
        }
      }

      if (any.IsList)
      {
        var items = Commander.CreateSqlCompatibleValue(any.List);
        var values = string.Join(",", (IEnumerable)items);
        return "{0} in (" + values + ")";
      }

      if (any.IsRange)
      {
        var range = any.Range;

        if (range.Min != null && range.Max != null)
        {
          var minArg = keyGen.DeriveName(parameter);
          var maxArg = keyGen.DeriveName(parameter);
          args[minArg] = range.Min;
          args[maxArg] = range.Max;
          return "{0} between @" + minArg + " and @" + maxArg;
        }

        if (range.Min != null)
        {
          var minArg = keyGen.DeriveName(parameter);
          args[minArg] = range.Min;
          return "{0} >= @" + minArg;
        }

        if (range.Max != null)
        {
          var name = keyGen.DeriveName(parameter);
          args[name] = range.Max;
          return "{0} <= @" + name;
        }

        args[parameter] = value;
        return "{0} = @" + parameter;
      }

      if (any.IsText == true)
      {
        if (any.TextHasWildcard)
        {
          args[parameter] = any.Text;
          return "{0} like @" + parameter;
        }
        else
        {
          args[parameter] = any.Text;
          return "{0} = @" + parameter;
        }
      }

      args[parameter] = any.Value;
      return "{0} = @" + parameter;
    }

    /// <summary>
    /// Aplica uma substituição de parâmetro estendido no texto indicado.
    /// 
    /// A função é complemento da função ApplyExtendedTemplate com a responsabilidade
    /// de localizar no texto as ocorrências do padrão "campo = {parametro}" realizando
    /// a substituição pela instrução comparadora definitiva.
    /// 
    /// A porção "{parametro}" pode conter um valor booliano na forma: {parametro|valor}.
    /// Por padrão, quando o valor de um parâmetro não é definido o parâmetro é considerado
    /// bem sucedido pra qualquer registro, tornando nulo o parâmetro para fins de filtro.
    /// Este comportamento pode ser invertido pelo uso de "{parametro|0}" ou "{parametro|false}".
    /// Neste caso, o parâmetro é considerado mal sucedido e todos os registros passam a
    /// ser rejeitados nesta condição.
    /// </summary>
    /// <param name="text">O texto que sofrerá a substituição.</param>
    /// <param name="parameter">Nome do parametro procurado.</param>
    /// <param name="replacement">Texto que substituirá o padrão "campo = {parametro}"</param>
    /// <returns>O texto com a substituição efetuada.</returns>
    private static string ReplaceMatches(string text, string parameter, string replacement)
    {
      Regex regex;
      MatchCollection matches;

      // Aplicando a substituição da instrução:
      //   @param is [not] set
      // Que deve ser tornar:
      //   1=1 quando verdadeiro
      //   1=0 quando falso
      regex = new Regex("(@" + parameter + @"\s+is(\s+not)?\s+set)");
      matches = regex.Matches(text);
      foreach (var match in matches.Cast<Match>())
      {
        var isSet = replacement != null;
        var negate = match.Groups[2].Value.Contains("not");
        if (negate)
          isSet = !isSet;
        var criteria = isSet ? "1=1" : "1=0";
        text = text.Replace(match.Value, criteria);
      }

      // Aplicando a substituição da instrução:
      //   target [not] matches [if set] @param
      // Que deve ser substituído por "replacement", produzindo por exemplo:
      //   target = valor
      //   target >= valor
      //   target <= valor
      //   target in (valor, ...)
      //   target like valor
      //   target between valor and valor
      // Sendo que:
      //   quando "not" está presente a condição é invertida.
      //   quando "if set" está presente, se a condicao for nula, o resultado é verdadeiro:
      //     1=1
      //   quando "if set" não está presente, se a condicao for nula, o resultado é falso:
      //     1=0
      regex = new Regex(@"(?:([a-zA-Z_.]+)|([a-zA-Z0-9_]*[(](?>[(](?<c>)|[^()]+|[)](?<-c>))*(?(c)(?!))[)]))\s+(not\s+)?matches\s+(if\s+set\s+)?@" + parameter);
      matches = regex.Matches(text);
      foreach (var match in matches.Cast<Match>())
      {
        string newSentence = null;

        if (replacement == null)
        {
          // quando "if set" está presente, se a condicao for nula, o resultado é verdadeiro:
          //   1=1
          // quando "if set" não está presente, se a condicao for nula, o resultado é falso:
          //   1=0
          var ifSet = match.Groups[4].Value.Contains("set");
          newSentence = ifSet ? "1=1" : "1=0";
        }
        else
        {
          var leftSide = match.Groups[1].Value + match.Groups[2].Value;
          newSentence = string.Format(replacement, leftSide);

          var negate = match.Groups[3].Value.Contains("not");
          if (negate)
            newSentence = "not " + newSentence;
        }

        text = text.Replace(match.Value, newSentence);
      }

      return text;
    }

    /// <summary>
    /// Extrai todos os nomes de parâmetros na forma "campo = {parametro}" encontrados
    /// no texto.
    /// 
    /// O nome do parâmetro deve ser composto apenas dos caracteres:
    /// -   letras
    /// -   numeros
    /// -   sublinhas
    /// </summary>
    /// <param name="text">O texto a ser processado.</param>
    /// <returns>Os nomes de parâmetros encontrados.</returns>
    private static IEnumerable<string> ExtractKnownExtendedParameters(string text)
    {
      var regex = new Regex(@"matches\s+(?:if\s+set\s+)?@([a-zA-Z0-9_]+)|@([a-zA-Z0-9_]+)\s+is(?:\s+not)?\s+set");
      var matches = regex.Matches(text);
      foreach (var match in matches.Cast<Match>())
      {
        yield return match.Groups[1].Value + match.Groups[2].Value;
      }
    }
  }
}

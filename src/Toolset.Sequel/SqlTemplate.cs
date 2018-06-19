using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
    /// Força a aplicação do template mesmo que a desabilitada na configuração
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

      if (SequelSettings.StandardTemplateEnabled && sql.Text.Contains("@{"))
        SqlTemplate.ApplyStandardTemplate(sql);

      if (SequelSettings.ExtendedTemplateEnabled && sql.Text.Contains("matches"))
        SqlTemplate.ApplyExtendedTemplate(sql);

      return sql;
    }

    /// <summary>
    /// Aplica o template simples de string SQL.
    /// 
    /// Por padrão, todos os parâmetros atribuídos são estocados na instância
    /// de Sql e aplicados à string SQL apenas no ato da execução da SQL.
    /// 
    /// Este método força a aplicação do template imediatamente, modificando
    /// a SQL original.
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
    /// -   Se "IF SET" está ausente a condição falha quando um valor
    ///     não é atribuído ao parâmetro.
    ///     
    ///     Por exemplo, a instrução:
    ///     -   id MATCHES @id
    ///
    ///     Corresponde a uma condição como:
    ///     -   id = @id
    /// 
    /// -   Se "IF SET" está presente a condição é simplesmente ignorada quando um valor
    ///     não é atribuído ao parâmetro.
    ///     
    ///     Por exemplo, a instrução:
    ///     -   id MATCHES IF SET @id
    ///
    ///     Corresponde a uma condição parecida com:
    ///     -   (@id IS NULL OR id = @id)
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    public static void ApplyExtendedTemplate(Sql sql)
    {
      var extendedParameters = ExtractKnownExtendedParameters(sql.Text).ToArray();
      var names =
        from name in extendedParameters
        orderby name.Length descending, name
        select name;
      foreach (var name in names)
      {
        var value = sql[name];
        var criteria = CreateCriteria(sql, name, value);
        var text = ReplaceTemplate(sql, name, criteria);

        sql.Text = text;
      }
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
    /// <param name="newParameters">
    /// Coleção dos parâmetros conhecidos.
    /// Novos parâmetros podem ser criados nesta coleção para suporte ao critério criado.
    /// </param>
    /// <returns>A condição que substituirá o template.</returns>
    private static string CreateCriteria(Sql sql, string parameter, object value)
    {
      if (value == null || (value as Values)?.IsNull == true)
      {
        return null;
      }

      if ((value as Values)?.IsValue == true)
      {
        value = ((Values)value).Value;
      }

      string criteria = null;

      if (value is Sql)
      {
        var innerSql = (Sql)value;
        innerSql.ApplyTemplate(false);

        var innerText = innerSql.Text + "\n";

        // cada parametro recebera um sufixo para diferenciacao de parametros
        // que já existem na instância de Sql.
        foreach (var parameterName in innerSql.ParameterNames)
        {
          var localName = sql.KeyGen.Rename(parameterName);

          sql[localName] = innerSql[parameterName];

          string pattern;
          string replacement;

          pattern = "@" + parameterName + "([^0-9A-Za-z_])";
          replacement = "@" + localName + "$1";
          innerText = Regex.Replace(innerText, pattern, replacement);

          pattern = "@{" + parameterName + "}";
          replacement = "@{" + localName + "}";
          innerText = innerText.Replace(pattern, replacement);
        }

        criteria = "{0} in (" + innerText + ")";
      }
      else if ((value as Values)?.IsArray == true)
      {
        var items = Commander.CreateSqlCompatibleValue(value);
        var values = string.Join(",", items);

        criteria = "{0} in (" + values + ")";
      }
      else if ((value as Values)?.IsRange == true)
      {
        var range = (Values)value;
        if (range.Min != null && range.Max != null)
        {
          var min = sql.KeyGen.Rename(parameter);
          var max = sql.KeyGen.Rename(parameter);

          criteria = "{0} between @" + min + " and @" + max;
          sql[min] = range.Min;
          sql[max] = range.Max;
        }
        else if (range.Min != null)
        {
          var name = sql.KeyGen.Rename(parameter);
          criteria = "{0} >= @" + name;
          sql[name] = range.Min;
        }
        else if (range.Max != null)
        {
          var name = sql.KeyGen.Rename(parameter);
          criteria = "{0} <= @" + name;
          sql[name] = range.Max;
        }
        else
        {
          criteria = "{0} = @" + parameter;
          sql[parameter] = value;
        }
      }
      else if ((value as Values)?.IsText == true)
      {
        var text = (Values)value;
        if (text.HasWildcard)
        {
          criteria = "{0} like @" + parameter;
          sql[parameter] = text.Text;
        }
        else
        {
          criteria = "{0} = @" + parameter;
          sql[parameter] = text.Text;
        }
      }
      else
      {
        criteria = "{0} = @" + parameter;
        sql[parameter] = value;
      }

      return criteria;
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
    private static string ReplaceTemplate(Sql sql, string parameter, string replacement)
    {
      Regex regex;
      MatchCollection matches;

      var text = sql.ToString();

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

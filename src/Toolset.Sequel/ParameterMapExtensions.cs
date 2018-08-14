using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Toolset.Collections;
using Toolset.Data;

namespace Toolset.Sequel
{
  /// <summary>
  /// Extensões de processamento de parâmetros de SQL.
  /// </summary>
  public static class ParameterMapExtensions
  {
    /// <summary>
    /// Atribui de uma só vez uma série de parâmetros definidos no mapa
    /// de chave/valor indicado.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameters">Os parâmetros a ser atribuídos.</param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static T Set<T>(this T sql, NameValueCollection parameters)
      where T : IParameterMap
    {
      foreach (var key in parameters.AllKeys)
      {
        sql.Parameters[key] = parameters[key];
      }
      return sql;
    }

    /// <summary>
    /// Atribui de uma só vez uma série de parâmetros definidos no mapa
    /// de chave/valor indicado.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameters">Os parâmetros a ser atribuídos.</param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static T Set<T>(this T sql, IEnumerable<KeyValuePair<string, object>> parameters)
      where T : IParameterMap
    {
      foreach (var parameter in parameters)
      {
        sql.Parameters[parameter.Key] = parameter.Value;
      }
      return sql;
    }

    /// <summary>
    /// Atribui de uma só vez uma série de parâmetros definidos no vetor indicado.
    /// O vetor é interpretado em pares de chave valor.
    /// Iniciando em zero, os termos pares são considerados como chaves e os termos
    /// ímpares são considerados como valores destas chaves.
    /// Exemplo:
    ///     var texto = "select * from usuario where nome = @nome, ativo = @ativo";
    ///     var sql = texto.AsSql();
    ///     sql.Set("nome", "Fulano", "ativo", 1);
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameters">Os parâmetros a ser atribuídos.</param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static T Set<T>(this T sql, params object[] parameters)
      where T : IParameterMap
    {
      var isGraph = parameters.NonNull().FirstOrDefault().IsGraph();
      if (isGraph)
      {
        var invalid = parameters.NonNull().FirstOrDefault(x => !x.IsGraph());
        if (invalid != null)
        {
          throw new Exception("Todos os parâmetros deveriam ser objetos para extração de propriedads mas foi contrado: " + invalid.GetType().FullName);
        }

        foreach (var graph in parameters)
        {
          graph.UnwrapGraph(sql.Parameters);
        }
      }
      else
      {
        var invalid = parameters.NonNull().FirstOrDefault(x => x.IsGraph());
        if (invalid != null)
        {
          throw new Exception("Era esperado uma lista de parâmetros simples na forma chave/valor mas foi encontrado: " + invalid.GetType().FullName);
        }

        UnwrapParameters(sql.Parameters, parameters);
      }
      return sql;
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="map">O mapa de parâmetros.</param>
    /// <param name="otherMaps">
    /// Outros mapas de parâmetros.
    /// Cada mapa de parâmetros irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static Sql SetMany(this Sql sql, NameValueCollection map, params NameValueCollection[] otherMaps)
    {
      // Os itens serão postos na cesta padrão. O nome da cestra padrão é uma string vazia.
      return SetMany(sql, map, otherMaps);
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// A cesta é usada pela instrução "MANY @bagName MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="bagName">Nome da cesta de parâmetros.</param>
    /// <param name="map">O mapa de parâmetros.</param>
    /// <param name="otherMaps">
    /// Outros mapas de parâmetros.
    /// Cada mapa de parâmetros irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static Sql SetMany(this Sql sql, string bagName, NameValueCollection map, params NameValueCollection[] otherMaps)
    {
      var bag = (sql[bagName] as IVar)?.Value as List<IDictionary<string, object>>;
      if (bag == null)
      {
        sql[bagName] = bag = new List<IDictionary<string, object>>();
      }

      var maps = map.AsSingle().Concat(otherMaps).Select(CreateMap).ToArray();
      bag.AddRange(maps);

      return sql;
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="map">O mapa de parâmetros.</param>
    /// <param name="otherMaps">
    /// Outros mapas de parâmetros.
    /// Cada mapa de parâmetros irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static Sql SetMany(this Sql sql, IDictionary map, params IDictionary[] otherMaps)
    {
      // Os itens serão postos na cesta padrão. O nome da cestra padrão é uma string vazia.
      return SetMany(sql, map, otherMaps);
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// A cesta é usada pela instrução "MANY @bagName MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="bagName">Nome da cesta de parâmetros.</param>
    /// <param name="map">O mapa de parâmetros.</param>
    /// <param name="otherMaps">
    /// Outros mapas de parâmetros.
    /// Cada mapa de parâmetros irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static Sql SetMany(this Sql sql, string bagName, IDictionary map, params IDictionary[] otherMaps)
    {
      var bag = (sql[bagName] as IVar)?.Value as List<IDictionary<string, object>>;
      if (bag == null)
      {
        sql[bagName] = bag = new List<IDictionary<string, object>>();
      }

      var maps = map.AsSingle().Concat(otherMaps).Select(CreateMap).ToArray();
      bag.AddRange(maps);

      return sql;
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// Os parâmetros são obtidos das propriedades dos grafos indicados.
    /// 
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameter">O objeto que será desmontado para obtenção dos parâmetros.</param>
    /// <param name="otherParameters">
    /// Outros objetos que serão desmontados para obtenção de parâmetros.
    /// Cada objeto desmontado irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static Sql SetMany(this Sql sql, object parameter, params object[] otherParameters)
    {
      // Os itens serão postos na cesta padrão. O nome da cestra padrão é uma string vazia.
      return SetMany(sql, "", parameter, otherParameters);
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// Os parâmetros são obtidos das propriedades dos grafos indicados.
    /// 
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="bagName">Nome da cesta de parâmetros.</param>
    /// <param name="graph">O objeto que será desmontado para obtenção dos parâmetros.</param>
    /// <param name="otherGraphs">
    /// Outros objetos que serão desmontados para obtenção de parâmetros.
    /// Cada objeto desmontado irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static Sql SetMany(this Sql sql, string bagName, object graph, params object[] otherGraphs)
    {
      var graphs = graph.AsSingle().Concat(otherGraphs);

      var invalid = graphs.NonNull().FirstOrDefault(x => !x.IsGraph());
      if (invalid != null)
        throw new Exception("Todos os parâmetros deveriam ser objetos para extração de propriedads mas foi contrado: " + invalid.GetType().FullName);

      var bag = (sql[bagName] as IVar)?.Value as List<IDictionary<string, object>>;
      if (bag == null)
      {
        sql[bagName] = bag = new List<IDictionary<string, object>>();
      }
      
      foreach (var item in graphs)
      {
        var map = item.UnwrapGraph();
        bag.Add(map);
      }

      return sql;
    }

    /// <summary>
    /// Adiciona entradas na cesta de parâmetros e repassa um construtor para definição
    /// dos parâmetros.
    /// 
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="graph">O objeto que será desmontado para obtenção dos parâmetros.</param>
    /// <param name="otherGraphs">
    /// Outros objetos que serão desmontados para obtenção de parâmetros.
    /// Cada objeto desmontado irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static Sql SetMany(this Sql sql, Action<ManyBuilder> builder, params Action<ManyBuilder>[] otherBuilder)
    {
      // Os itens serão postos na cesta padrão. O nome da cestra padrão é uma string vazia.
      return SetMany(sql, "", builder, otherBuilder);
    }

    /// <summary>
    /// Adiciona entradas na cesta de parâmetros e repassa um construtor para definição
    /// dos parâmetros.
    /// 
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="bagName">Nome da cesta de parâmetros.</param>
    /// <param name="graph">O objeto que será desmontado para obtenção dos parâmetros.</param>
    /// <param name="otherGraphs">
    /// Outros objetos que serão desmontados para obtenção de parâmetros.
    /// Cada objeto desmontado irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static Sql SetMany(this Sql sql, string bagName, Action<ManyBuilder> builder, params Action<ManyBuilder>[] otherBuilder)
    {
      var bag = (sql[bagName] as IVar)?.Value as List<IDictionary<string, object>>;
      if (bag == null)
      {
        sql[bagName] = bag = new List<IDictionary<string, object>>();
      }

      var builders = builder.AsSingle().Concat(otherBuilder);
      foreach (var instance in builders)
      {
        var many = new ManyBuilder();
        instance.Invoke(many);
        bag.Add(many.Parameters);
      }

      return sql;
    }

    /// <summary>
    /// Converte o dicionario em uma coleção de parâmetros nome/valor.
    /// </summary>
    /// <param name="dictionary">O dicionário a ser convertido.</param>
    /// <returns>O mapa de parâmetros obtido.</returns>
    private static IDictionary<string, object> CreateMap(IDictionary dictionary)
    {
      var map = dictionary as IDictionary<string, object>;
      if (map == null)
      {
        var entries =
          dictionary
            .Cast<DictionaryEntry>()
            .Select(e => KeyValuePair.Create(e.Key.ToString(), e.Value));
        map = new HashMap(entries);
      }
      return map;
    }

    /// <summary>
    /// Converte o dicionario em uma coleção de parâmetros nome/valor.
    /// </summary>
    /// <param name="dictionary">O dicionário a ser convertido.</param>
    /// <returns>O mapa de parâmetros obtido.</returns>
    private static IDictionary<string, object> CreateMap(NameValueCollection dictionary)
    {
      var map = new HashMap();
      foreach (var key in dictionary.AllKeys)
      {
        map[key] = dictionary[key];
      }
      return map;
    }

    /// <summary>
    /// Aplica uma formatação posicional usando String.Format do DotNet.
    /// Todas as capacidades do String.Format são suportadas.
    /// Exemplo:
    ///     var texto = "select * from {0} where {1} = {2}";
    ///     var sql = texto.AsSql();
    ///     sql.Format("usuario", "nome", "Fulano");
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameters">
    /// A coleção de parâmetros posicionais para substituição.
    /// </param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static Sql Format(this Sql sql, params object[] parameters)
    {
      var text = sql.ToString();

      // Escapando ocorrencias de { e }
      // - Caracteres { e } sao duplicados
      // - Em seguida o padrao {{numero}} é retornado para {numero}
      text =
        Regex.Replace(
          text.Replace("{", "{{").Replace("}", "}}"),
          "[{]([{][\\d]+[}])[}]",
          "$1"
        );

      sql.Text = string.Format(text, parameters);
      return sql;
    }

    /// <summary>
    /// Copia os parâmetros da lista indicada para um mapa.
    /// A lista de parâmetros deve ter um número par de argumentos.
    /// Cada argumento par, iniciando em zero, é considerado um nome de chave.
    /// E cada argumento impar é considerado o valor dessa chave.
    /// </summary>
    /// <param name="map">O dicionário a ser modificado.</param>
    /// <param name="parameters">A coleção de parâmetros.</param>
    private static void UnwrapParameters(IDictionary<string, object> map, IEnumerable<object> parameters)
    {
      var enumerator = parameters.GetEnumerator();
      while (enumerator.MoveNext())
      {
        if (enumerator.Current is string)
        {
          var token = (string)enumerator.Current;
          if (token.Contains("="))
          {
            var statements =
              from statement in token.Split(',')
              let tokens = statement.Split('=')
              select new
              {
                key = tokens.First(),
                value = string.Join("=", tokens.Skip(1))
              };

            foreach (var statement in statements)
            {
              var key = statement.key.Trim();
              map[key] = statement.value.Trim();
            }
          }
          else
          {
            var key = token.Trim();

            if (!enumerator.MoveNext())
              throw new SequelException("Faltou definir o valor de: " + key);

            map[key] = enumerator.Current;
          }
        }
        else
        {
          throw new SequelException(
            "Era esperado um nome de parâmetro ou uma atribuição (\"parametro=valor\") mas "
          + "foi encontrado: " + enumerator.Current
          );
        }
      }
    }

  }
}

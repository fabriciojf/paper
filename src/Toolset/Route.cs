using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Toolset;
using Toolset.Collections;

namespace Toolset
{
  /// <summary>
  /// Utilitário para construção de URI.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Route constrói uma URI pela combinação de partes de URI e definição de argumentos de
  /// URI (QueryString).
  /// Uma parte de URI combinada pode reescrever completamente a URI, reescrever apenas parte
  /// ou apenas acrescentar uma parte à frente da URI, de acordo com o padrão de nome adotado
  /// pela parte combinada.
  /// </para>
  /// <para>
  /// Uma parte iniciada pelo protocolo provoca a reescrita completa da URI.<br/>
  /// Por exemplo:
  /// <pre>
  ///       http://host.com/minha/api
  ///     + http://web.br/meu/site
  ///     = http://web.br/meu/site
  /// </pre>
  /// Uma parte iniciada por barra (/) provoca a reescrita completa do caminho da URI.<br/>
  /// Por exemplo:
  /// <pre>
  ///       http://host.com/minha/api
  ///     + /meu/site
  ///     = http://host.com/meu/site
  /// </pre>
  /// Nos demais casos a parte é adicionada ao caminho da URI.<br/>
  /// Por exemplo:
  /// <pre>
  ///       http://host.com/minha/api
  ///     + meu/site
  ///     = http://host.com/minha/api/meu/site
  /// </pre>
  /// Se a parte contém argumentos de URI, os argumentos são combinados com os
  /// argumentos já existentes. O argumento novo prevalesce sobre o antigo se coincidirem
  /// em nome. A comparação de nome de argumento é sensível a caso.
  /// Por exemplo:
  /// <pre>
  ///       http://host.com/?f=json
  ///     + site?<strong>p=10</strong>
  ///     = http://host.com/site?f=json&amp;<strong>p=10</strong>
  /// </pre>
  /// Parâmetros coincidentes são sobrepostos:
  /// <pre>
  ///       http://host.com/?<strong>f=json</strong>
  ///     + site?p=10&amp;<strong>f=xml</strong>
  ///     = http://host.com/site?<strong>f=xml</strong>&amp;p=10&amp;
  /// </pre>
  /// A combinação de nomes é sensível a caso:
  /// <pre>
  ///       http://host.com/?<strong>f</strong>=json
  ///     + site?p=10&amp;<strong>F=20</strong>
  ///     = http://host.com/site?<strong>f</strong>=json&amp;p=10&amp;<strong>F=20</strong>
  /// </pre>
  /// </para>
  /// <para>
  /// Route não faz distinção entre URI de pasta e URI de arquivo.
  /// A combinação de uma URI terminada em barra (/) ou de uma URI não terminada em barra
  /// produz o mesmo efeito.
  /// A notação de ponto (.) pode ser usada para referenciar o caminho corrente ou o caminho
  /// anterior, explicitamente substituindo um nome de arquivo.
  /// <para>
  /// URI terminada ou não em barra produz o mesmo efeito:
  /// <pre>
  /// 
  ///       /pasta/arquivo.md
  ///     + <strong>outro.txt</strong>
  ///     = /pasta/arquivo.md/<strong>outro.txt</strong>
  ///     
  ///       /pasta/arquivo.md/
  ///     + outro.txt
  ///     = /pasta/arquivo.md/outro.txt
  /// </pre>
  /// A notação de ponto pode ser usada para navegação para trás:
  /// <pre>
  ///       /pasta/arquivo.md
  ///     + ../<strong>outro.txt</strong>
  ///     = /pasta/<strong>outro.txt</strong>
  ///     
  ///       /pasta/arquivo.md/
  ///     + ../<strong>outro.txt</strong>
  ///     = /pasta/<strong>outro.txt</strong>
  /// </pre>
  /// </para>
  /// </para>
  /// <para>
  /// Route é imutável. Qualquer combinação ou modificação produz uma nova cópia de Route.
  /// <code language="C#">
  ///     var raiz = new Route("http://localhost/");
  ///     var rota1 = raiz.Combine("rota");
  ///     var rota2 = raiz.Combine("rota");
  ///     
  ///     Debug.WriteLine(rota1 == rota2);
  ///     
  ///     // Imprime:
  ///     //    false
  /// </code>
  /// </para>
  /// </remarks>
  /// <example>
  /// <code language="c#">
  /// var raiz = new Route("http://localhost/?v=2");
  /// 
  /// // usuario como subrota de raiz
  /// var usuario = raiz.Combine("site/usuario");
  /// 
  /// // subrotas de usuario
  /// var login = usuario.Combine("login");
  /// var logout = usuario.Combine("logout?redirect=/index.html");
  /// 
  /// // index.html como outra subrota de raiz
  /// var indice = raiz.Combine("index.html");
  /// 
  /// Console.WriteLine(usuario);
  /// Console.WriteLine(login);
  /// Console.WriteLine(logout);
  /// Console.WriteLine(indice);
  /// 
  /// // Imprime:
  /// //     http://localhost/site/usuario?v=2
  /// //     http://localhost/site/usuario/login?v=2
  /// //     http://localhost/site/usuario/logout?v=2&amp;redirect=/index.html
  /// //     http://localhost/index.html?v=2
  /// </code>
  /// </example>
  public class Route
  {
    // Marca para remoção de host, porta e protocolo de uma URI.
    // Durante a construção da rota se esta macro for interceptada as informações
    // de host, porta e protocolo são removidas da URI tornando-a relativa.
    private const string MakeRelativeMacro = "://";

    private readonly Route parent;
    private readonly string[] paths;
    private readonly Arg[] args;

    // Cache do texto produzido
    private string _text;

    private Route(Route parent, IEnumerable<string> paths, IEnumerable<Arg> args)
    {
      this.parent = parent;
      this.paths = paths?.ToArray();
      this.args = args?.ToArray();
    }

    private string Text
      => _text ?? (_text = CreateText());

    /// <summary>
    /// Cria uma <see cref="Route"/> vazia.
    /// </summary>
    public Route()
    {
    }

    /// <summary>
    /// Cria uma <see cref="Route"/> com uma rota inicial.
    /// </summary>
    /// <param name="address">A rota inicial.</param>
    public Route(Uri address)
      : this(address?.ToString())
    {
    }

    /// <summary>
    /// Cria uma <see cref="Route"/> com uma rota inicial.
    /// </summary>
    /// <param name="address">A rota inicial.</param>
    public Route(string address)
    {
      if (address != null)
      {
        var parts = EnumerateParts(address).ToArray();
        this.paths = parts.OfType<string>().ToArray();
        this.args = parts.OfType<Arg>().ToArray();
      }
    }

    /// <summary>
    /// Instância inicial de <see cref="Route"/> a partir da qual a instância corrente
    /// foi derivada.
    /// </summary>
    /// <example>
    /// <code language="c#">
    /// var rota = new Route("http://localhost/site");
    /// var site = rota.Combine("/meu/site");
    /// var perfil = site.Combine("perfil.html");
    /// 
    /// Debug.WriteLine(perfil);
    /// Debug.WriteLine(perfil.InitialRoute);
    /// Debug.WriteLine(rota == perfil.InitialRoute);
    /// 
    /// // Imprime:
    /// //    http://localhost/meu/site/perfil.html
    /// //    http://localhost/site
    /// //    true
    /// </code>
    /// </example>
    public Route InitialRoute
    {
      get
      {
        var route = this;
        while (route.parent != null)
          route = route.parent;
        return route;
      }
    }

    /// <summary>
    /// Remove a informação de protocolo, host e porta tornando a URI relativa.
    /// </summary>
    /// <returns>Uma nova instância de <see cref="Route"/> relativa.</returns>
    public Route MakeRelative()
    {
      // Adicionando uma informação de protocolo e host vazia provoca a remocação desta parte.
      return new Route(this, new[] { MakeRelativeMacro }, null);
    }

    /// <summary>
    /// Remove a informação de protocolo, host e porta tornando a URI relativa.
    /// </summary>
    /// <remarks>
    /// A realitivização se dá apenas pela comparação dos caminhos de recurso entre as URIs.
    /// A porção de protocolo, host e porta são ignoradas.
    /// <br/>
    /// Exemplo:
    /// <list type="table">
	  ///   <listHeader>
	  ///   	<term>URI</term>
	  ///   	<term>Relativa a</term>
	  ///   	<term>Produz</term>
	  ///   </listHeader>
    ///   <item>
    ///     <description>http://localhost/site/users/10</description>
    ///     <description>http://localhost/site</description>
    ///     <description>/users/10</description>
    ///   </item>
    ///   <item>
    ///     <description>http://localhost/site/users/10</description>
    ///     <description>ftp://server.com/site</description>
    ///     <description>/users/10</description>
    ///   </item>
    ///   <item>
    ///     <description>http://localhost/site/users/10</description>
    ///     <description>/site</description>
    ///     <description>/users/10</description>
    ///   </item>
    ///   <item>
    ///     <description>http://localhost/site/users/10</description>
    ///     <description>ftp://localhost/api/1</description>
    ///     <description>/site/users/10</description>
    ///   </item>
    ///   <item>
    ///     <description>http://localhost/site/users/10</description>
    ///     <description>/api/1</description>
    ///     <description>/site/users/10</description>
    ///   </item>
    ///   <item>
    ///     <description>/site/users/10</description>
    ///     <description>http://localhost/site</description>
    ///     <description>/users/10</description>
    ///   </item>
    ///   <item>
    ///     <description>/site/users/10</description>
    ///     <description>ftp://server.com/site</description>
    ///     <description>/users/10</description>
    ///   </item>
    ///   <item>
    ///     <description>/site/users/10</description>
    ///     <description>/site</description>
    ///     <description>/users/10</description>
    ///   </item>
    ///   <item>
    ///     <description>/site/users/10</description>
    ///     <description>ftp://localhost/api/1</description>
    ///     <description>/site/users/10</description>
    ///   </item>
    ///   <item>
    ///     <description>/site/users/10</description>
    ///     <description>/api/1</description>
    ///     <description>/site/users/10</description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <param name="relativeTo">Caminho para o qual a rota será relativizada.</param>
    /// <returns>Uma nova instância de <see cref="Route"/> relativa.</returns>
    public Route MakeRelative(string relativeTo)
    {
      var currentRoute = MakeRelative().ToString();
      var relativeRoute = new Route(relativeTo).UnsetAllArgs().MakeRelative().ToString();

      if (currentRoute.StartsWith(relativeRoute))
        currentRoute = currentRoute.Substring(relativeRoute.Length);

      return new Route(currentRoute);
    }

    /// <summary>
    /// Combina os caminhos indicados com a rota atual.
    /// Os argumentos encontrados em cada caminho são mesclados com os argumentos da
    /// rota atual.
    /// </summary>
    /// <remarks>
    /// Embora os argumentos sejam sempre mesclados durante uma combinação, as porções da URI
    /// relativas ao domínio e ao caminho são combinadas segundo regras específicas.
    /// 
    /// Quando uma parte combinada contém informação de domínio, como em
    /// <c>http://localhost/</c>, o domínio e o caminho são completamente substituídos por
    /// esta nova parte.
    /// 
    /// Quando uma parte combinada se inicia por barra o caminho é completamente substituído
    /// por esta nova parte.
    /// 
    /// Nos demais casos a nova parte é acrescentada no fim do caminho atual.
    /// </remarks>
    /// <param name="path">Um primeiro caminho a ser combinado.</param>
    /// <param name="others">Outros caminhos a serem combinados.</param>
    /// <returns>Uma nova URI com a combinação dos caminhos.</returns>
    public Route Combine(string path, params string[] others)
    {
      return Combine(path.AsSingle().Concat(others));
    }

    /// <summary>
    /// Combina os caminhos indicados com a rota atual.
    /// Os argumentos encontrados em cada caminho são mesclados com os argumentos da
    /// rota atual.
    /// </summary>
    /// <remarks>
    /// Embora os argumentos sejam sempre mesclados durante uma combinação, as porções da URI
    /// relativas ao domínio e ao caminho são combinadas segundo regras específicas.
    /// 
    /// Quando uma parte combinada contém informação de domínio, como em
    /// <c>http://localhost/</c>, o domínio e o caminho são completamente substituídos por
    /// esta nova parte.
    /// 
    /// Quando uma parte combinada se inicia por barra o caminho é completamente substituído
    /// por esta nova parte.
    /// 
    /// Nos demais casos a nova parte é acrescentada no fim do caminho atual.
    /// </remarks>
    /// <param name="paths">Os caminhos a serem combinados.</param>
    /// <returns>Uma nova URI com a combinação dos caminhos.</returns>
    public Route Combine(IEnumerable<string> paths)
    {
      var tokens =
        paths
          .SelectMany(EnumerateParts)
          .NonNull()
          .ToArray();
      var allPath = tokens.OfType<string>().ToArray();
      var allArg = tokens.OfType<Arg>().ToArray();
      return new Route(this, allPath, allArg);
    }

    /// <summary>
    /// Força a adição sos caminhos à frente da rota atual.
    /// Os argumentos encontrados em cada caminho são mesclados com os argumentos da rota atual.
    /// </summary>
    /// <remarks>
    /// Embora os argumentos sejam sempre mesclados durante uma combinação, as porções da URI
    /// relativas ao domínio e ao caminho são combinadas segundo regras específicas.
    /// 
    /// Quando uma parte combinada contém informação de domínio, como em
    /// <c>http://localhost/</c>, uma exceção é lançada.
    /// 
    /// Mesmo que uma parte combinada se inicie por barra o caminho será sempre combinado
    /// à frente da rota atual.
    /// </remarks>
    /// <param name="path">Um primeiro caminho a ser combinado.</param>
    /// <param name="others">Outros caminhos a serem combinados.</param>
    /// <returns>Uma nova URI com a combinação dos caminhos.</returns>
    public Route Append(string path, params string[] others)
    {
      return Append(path.AsSingle().Concat(others));
    }

    /// <summary>
    /// Força a adição sos caminhos à frente da rota atual.
    /// Os argumentos encontrados em cada caminho são mesclados com os argumentos da rota atual.
    /// </summary>
    /// <remarks>
    /// Embora os argumentos sejam sempre mesclados durante uma combinação, as porções da URI
    /// relativas ao domínio e ao caminho são combinadas segundo regras específicas.
    /// 
    /// Quando uma parte combinada contém informação de domínio, como em
    /// <c>http://localhost/</c>, uma exceção é lançada.
    /// 
    /// Mesmo que uma parte combinada se inicie por barra o caminho será sempre combinado
    /// à frente da rota atual.
    /// </remarks>
    /// <param name="paths">Os caminhos a serem combinados.</param>
    /// <returns>Uma nova URI com a combinação dos caminhos.</returns>
    public Route Append(IEnumerable<string> paths)
    {
      var tokens =
        paths
          .SelectMany(EnumerateParts)
          .NonNull()
          .ToArray();

      var allArg = tokens.OfType<Arg>().ToArray();
      var allPath =
        tokens
          .OfType<string>()
          .Select(x => Regex.Replace(x, "^/+", ""))
          .ToArray();

      var absolutePath = allPath.FirstOrDefault(x => x.Contains("://"));
      if (absolutePath != null)
        throw new ArgumentException("Não é possível adicionar um caminho absoluto ao fim da rota: " + absolutePath);

      return new Route(this, allPath, allArg);
    }

    /// <summary>
    /// Sobe um nível no caminho da URI.
    /// Se não houver mais caminho para subir permanece na rota atual.
    /// </summary>
    /// <remarks>
    /// Subir um nível na rota causa o mesmo efeito que combinar a rota corrente com a notação
    /// de pontos "..".
    /// Quando a raiz do caminho é atingida a rota permace a mesma.
    /// <br/>
    /// Por exemplo:
    /// <pre>
    ///     var rota1 = new Route("http://localhost/").Up();
    ///     var rota2 = new Route("http://localhost/meu").Up();
    ///     var rota3 = new Route("http://localhost/meu/site").Up();
    ///     
    ///     Debug.WriteLine(rota1);
    ///     Debug.WriteLine(rota2);
    ///     Debug.WriteLine(rota3);
    ///     
    ///     // Imprime:
    ///     //   http://localhost/
    ///     //   http://localhost/
    ///     //   http://localhost/meu
    /// </pre>
    /// </remarks>
    /// <returns>Uma nova instância de <see cref="Route"/> um nível acima na rota.</returns>
    public Route Up()
    {
      return this.Combine("..");
    }

    /// <summary>
    /// Combina as propriedades de um objeto com os argumentos de URI.
    /// </summary>
    /// <remarks>
    /// <para>
    /// O algoritmo analisa apenas propriedades dos tipos: primitivos, como <c>int</c>,
    /// <c>bool</c> e <c>float</c>, <c>string</c>, <c>DateTime</c>, <c>TimeSpan</c> e
    /// arrays destes tipos e cujo valor seja diferente de nulo.
    /// <para>
    /// Seja "tipo" o tipo da propriedade e "valor" o valor da propriedade, então:
    /// <pre>
    ///    where valor != null
    ///       || tipo.IsArray
    ///       || tipo.IsPrimitive
    ///       || tipo == typeof(string)
    ///       || tipo == typeof(DateTime)
    ///       || tipo == typeof(TimeSpan)
    /// </pre>
    /// </para>
    /// </para>
    /// </remarks>
    /// <param name="graph">O objeto que contém as propriedades a serem atribuídas.</param>
    /// <returns>
    /// Uma nova instância de <see cref="Route"/>com os argumentos atribuídos.
    /// </returns>
    /// <example>
    /// <code language="C#">
    /// var args = new {
    ///   f = "json",
    ///   id = new[] { 1, 2 }
    /// };
    /// 
    /// var rota = new Route("http://localhost/").Combine(args);
    /// Debug.WriteLine(rota);
    /// 
    /// // Imprime:
    /// //    http://localhost/?f=json&amp;id[]=1&amp;id[]=2
    /// </code>
    /// </example>
    public Route SetArg(object graph)
    {
      if (graph == null)
        return this;

      if (graph.GetType().IsPrimitive || graph is string || graph is DateTime || graph is TimeSpan)
        throw new InvalidOperationException("Era esperado um objeto mas foi encontrado um: " + graph.GetType().FullName);

      if (graph is IEnumerable)
      {
        var enumerable = ((IEnumerable)graph).Cast<object>();
        return SetArg(enumerable);
      }

      var args = (
        from p in graph.GetType().GetProperties()
        where p.PropertyType.IsArray
           || p.PropertyType.IsPrimitive
           || p.PropertyType == typeof(string)
           || p.PropertyType == typeof(DateTime)
           || p.PropertyType == typeof(TimeSpan)
        where !p.GetIndexParameters().Any()
        let value = p.GetValue(graph)
        where value != null
        select new[] { p.Name.ChangeCase(TextCase.CamelCase), value }
      );

      return SetArg(args.SelectMany());
    }

    /// <summary>
    /// Combina os pares de argumentos com os argumentos já existentes na URI.
    /// É esperado que os argumentos pares, a partir de zero, sejam nomes de argumentos, e
    /// os argumentos ímpares sejam os valores destes argumentos.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Os argumentos devem ser passados em pares de nome e valor.
    /// Os nomes devem ser obrigatoriamente <c>strings</c> e os valores devem
    /// corresponder a um dos formatos suportados.
    /// </para>
    /// <para>
    /// Formatos suportados:
    /// <list type="bullet">
    ///   <item>Tipos primitivos do .NET, como <c>int</c>, <c>float</c>, etc.</item>
    ///   <item>String</item>
    ///   <item>DateTime</item>
    ///   <item>TimeSpan</item>
    ///   <item>Arrays ou coleções destes tipos</item>
    ///   <item>
    ///     Um objeto especial no padrão Range.<br/>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Arrays</strong>
    /// <br/>
    /// Arrays são repsentados na URI na forma <c>arg[]=valor</c>.
    /// <br/>
    /// Por exemplo:
    /// <pre>
    ///     var route = new Route().Set("rows", new[] { 10, 20 });
    ///     Debug.WriteLine(route);
    ///     
    ///     // Imprime:
    ///     //    ?rows[]=10&amp;rows[]=20
    /// </pre>
    /// </para>
    /// <para>
    /// <strong>Objetos no padrão Range</strong>
    /// <br/>
    /// Um objeto no padrão Range é qualquer objeto que declare as propriedades <c>Min</c> e
    /// <c>Max</c> indicando início e fim de uma série.
    /// Os parâmetros são reproduzidos na URI na forma: <c>arg.min</c> e <c>arg.max</c>.
    /// <br/>
    /// Por exemplo:
    /// <pre>
    ///     var route = new Route().Set("rows", new object { min = 10, max = 20 });
    ///     Debug.WriteLine(route);
    ///     
    ///     // Imprime:
    ///     //    ?rows.min=10&amp;rows.max=20
    /// </pre>
    /// </para>
    /// </remarks>
    /// <param name="arg">O nome do primeiro argumento.</param>
    /// <param name="value">O valor do primeiro argumento.</param>
    /// <param name="otherArgPairs">Os pares de nome e valor</param>
    /// <returns>
    /// Uma nova instância de <see cref="Route"/> com os argumentos combinados.
    /// </returns>
    public Route SetArg(string arg, object value, params object[] otherArgPairs)
    {
      var allArgs = arg.AsSingle().Append(value).Concat(otherArgPairs);
      return SetArg(allArgs);
    }

    /// <summary>
    /// Combina os pares de argumentos com os argumentos já existentes na URI.
    /// É esperado que os argumentos pares, a partir de zero, sejam nomes de argumentos, e
    /// os argumentos ímpares sejam os valores destes argumentos.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Os argumentos devem ser passados em pares de nome e valor.
    /// Os nomes devem ser obrigatoriamente <c>strings</c> e os valores devem
    /// corresponder a um dos formatos suportados.
    /// </para>
    /// <para>
    /// Formatos suportados:
    /// <list type="bullet">
    ///   <item>Tipos primitivos do .NET, como <c>int</c>, <c>float</c>, etc.</item>
    ///   <item>String</item>
    ///   <item>DateTime</item>
    ///   <item>TimeSpan</item>
    ///   <item>Arrays ou coleções destes tipos</item>
    ///   <item>
    ///     Um objeto especial no padrão Range.<br/>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Arrays</strong>
    /// <br/>
    /// Arrays são repsentados na URI na forma <c>arg[]=valor</c>.
    /// <br/>
    /// Por exemplo:
    /// <pre>
    ///     var route = new Route().Set("rows", new[] { 10, 20 });
    ///     Debug.WriteLine(route);
    ///     
    ///     // Imprime:
    ///     //    ?rows[]=10&amp;rows[]=20
    /// </pre>
    /// </para>
    /// <para>
    /// <strong>Objetos no padrão Range</strong>
    /// <br/>
    /// Um objeto no padrão Range é qualquer objeto que declare as propriedades <c>Min</c> e
    /// <c>Max</c> indicando início e fim de uma série.
    /// Os parâmetros são reproduzidos na URI na forma: <c>arg.min</c> e <c>arg.max</c>.
    /// <br/>
    /// Por exemplo:
    /// <pre>
    ///     var route = new Route().Set("rows", new object { min = 10, max = 20 });
    ///     Debug.WriteLine(route);
    ///     
    ///     // Imprime:
    ///     //    ?rows.min=10&amp;rows.max=20
    /// </pre>
    /// </para>
    /// </remarks>
    /// <param name="argPairs">Os pares de nome e valor</param>
    /// <returns>
    /// Uma nova instância de <see cref="Route"/> com os argumentos combinados.
    /// </returns>
    public Route SetArg(IEnumerable<object> argPairs)
    {
      var pairs = argPairs.ToArray();

      var isEven = (pairs.Length % 2) == 0;
      if (!isEven)
        throw new Exception("Os parâmetros repassados para o método Set() devem ser pares de chave e valor.");

      var allArgs =
        from i in Enumerable.Range(0, pairs.Length >> 1)
        select new Arg
        {
          Name = (string)pairs[i << 1],
          Value = pairs[(i << 1) + 1]
        };

      return new Route(this, null, allArgs);
    }

    /// <summary>
    /// Remove argumentos da URI.
    /// </summary>
    /// <param name="argName">O nome do primeiro argumento a ser removido.</param>
    /// <param name="others">Os demais argumentos a serem removidos.</param>
    /// <returns>Uma nova instância de <see cref="Route"/> sem os argumentos.</returns>
    public Route UnsetArgs(string argName, params string[] others)
    {
      return UnsetArgs(argName.AsSingle().Union(others));
    }

    /// <summary>
    /// Remove argumentos da URI.
    /// </summary>
    /// <param name="argNames">Os demais argumentos a serem removidos.</param>
    /// <returns>Uma nova instância de <see cref="Route"/> sem os argumentos.</returns>
    public Route UnsetArgs(IEnumerable<string> argNames)
    {
      // Desfazer uma variavel é o mesmo que definir seu valor como nulo.
      // O método ToString retira argumentos nulos da URI
      var args = argNames.Select(name => new Arg { Name = name });
      return new Route(this, null, args);
    }

    /// <summary>
    /// Remove todos os argumentos da URI.
    /// </summary>
    /// <returns>Uma nova instância de <see cref="Route"/> sem os argumentos.</returns>
    public Route UnsetAllArgs()
    {
      // Desfazer uma variavel é o mesmo que definir seu valor como nulo.
      // O método ToString retira argumentos nulos da URI
      var args = EnumerateArg().Select(x => new Arg { Name = x.Name });
      return new Route(this, null, args);
    }

    /// <summary>
    /// Remove todos os argumentos da URI exceto aqueles indicados no parâmetro.
    /// </summary>
    /// <param name="arg">O nome do primeiro argumento a ser mantido na URI.</param>
    /// <param name="others">Os nomdes dos demais argumentos a serem mantidos na URI.</param>
    /// <returns>Uma nova instância de <see cref="Route"/> sem os argumentos.</returns>
    public Route UnsetAllArgsExcept(string arg, params string[] others)
    {
      var names = EnumerateArg().Select(x => x.Name).Except(arg.AsSingle().Union(others));
      return UnsetArgs(names);
    }

    /// <summary>
    /// Remove todos os argumentos da URI exceto aqueles indicados no parâmetro.
    /// </summary>
    /// <param name="args">Os nomdes dos argumentos a serem mantidos na URI.</param>
    /// <returns>Uma nova instância de <see cref="Route"/> sem os argumentos.</returns>
    public Route UnsetAllArgsExcept(IEnumerable<string> args)
    {
      var names = EnumerateArg().Select(x => x.Name).Except(args);
      return UnsetArgs(names);
    }

    /// <summary>
    /// Imprime a URI atual no console e no System.Diagnostics.<br/>
    /// Apenas para depuração.
    /// </summary>
    /// <returns>A prórpia instância de <see cref="Route"/>.</returns>
    public Route Echo()
    {
      Console.WriteLine(ToString());
      Trace.TraceInformation(ToString());
      return this;
    }

    /// <summary>
    /// Constrói uma URI baeada na rota corrente.
    /// </summary>
    /// <returns>A URI construída.</returns>
    public Uri ToUri()
      => new Uri(this.Text, UriKind.RelativeOrAbsolute);

    /// <summary>
    /// Obtém uma <see cref="System.String" /> que representa a instância.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> que representa a instância.
    /// </returns>
    public override string ToString()
      => this.Text;

    /// <summary>
    /// Processa os argumentos do nodo e a cadeia de nodos para produzir a URI
    /// representada por esta rota.
    /// </summary>
    /// <returns>O texto representando a URI.</returns>
    private string CreateText()
    {
      string uri = null;

      string radical = null;
      string resource = null;
      string queryString = null;

      // construindo o caminho (pathPart)
      {
        radical = "";

        IEnumerable<string> segments = Enumerable.Empty<string>();

        foreach (var token in EnumeratePath())
        {
          IEnumerable<string> parts = token.Split('/');

          if (token == MakeRelativeMacro)
          {
            // Este token é usado na remoção da informação de protocolo, host e porta.
            radical = "";
            continue;
          }

          if (token.Contains("://"))
          {
            radical = string.Join("/", parts.Take(3));
            parts = parts.Skip(3);
            segments = Enumerable.Empty<string>();
          }
          else if (token.StartsWith("/"))
          {
            segments = Enumerable.Empty<string>();
          }

          foreach (var part in parts.NonEmpty())
          {
            if (part == ".")
              continue;

            if (part == "..")
            {
              segments =
                segments.Any()
                  ? segments.Take(segments.Count() - 1)
                  : Enumerable.Empty<string>();
            }
            else
            {
              segments = segments.Append(part);
            }
          }
        }

        resource = string.Join("/", segments);
      }

      // construindo os argumentos (queryString)
      {
        var argPairs =
          from a in EnumerateArg()
          let text = CreateArgumentString(a)
          where text != null
          select text;
        queryString = string.Join("&", argPairs);
      }

      // montando a uri
      {
        uri = radical + "/" + resource;

        if (queryString.Length > 0)
          uri += "?" + queryString;
      }

      return uri;
    }

    private string CreateArgumentString(Arg arg)
    {
      if (arg.Value == null)
        return null;

      if (arg.Value is IEnumerable && !(arg.Value is string))
      {
        var values =
          from value in ((IEnumerable)arg.Value).Cast<object>()
          where value != null
          select arg.Name + "[]=" + Format(value);

        var text = string.Join("&", values);
        return text;
      }

      if (IsRange(arg.Value))
      {
        var type = arg.Value.GetType();
        var min = type.GetProperty("Min") ?? type.GetProperty("min");
        var max = type.GetProperty("Max") ?? type.GetProperty("max");

        var minValue = min.GetValue(arg.Value);
        var maxValue = max.GetValue(arg.Value);

        if (minValue != null)
          minValue = $"{arg.Name}.min={Format(minValue)}";
        if (maxValue != null)
          maxValue = $"{arg.Name}.max={Format(maxValue)}";

        var values = new[] { minValue, maxValue }.NonNull();

        var text = string.Join("&", values);
        return text;
      }

      return arg.Name + "=" + Format(arg.Value);
    }

    /// <summary>
    /// Determina se o valor indicado segue o padrão Range.
    /// Um objeto no padrão Range suporta as propriedades Min e Max.
    /// A comparação de nome de propriedade é insensível a caso.
    /// 
    /// Por exemplo, todos estes objetos são considerados Range:
    ///   
    ///   var range1 = new { Min = 10, Max = 20 };
    ///   var range2 = new { min = 10, max = 20 };
    ///   var range3 = new MyCustomRangeClass { Min = 10, Max = 20 };
    ///   
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///   <c>true</c> if the specified value is range; otherwise, <c>false</c>.
    /// </returns>
    private bool IsRange(object value)
    {
      var type = value.GetType();
      var hasMin = (type.GetProperty("Min") ?? type.GetProperty("min")) != null;
      var hasMax = (type.GetProperty("Max") ?? type.GetProperty("max")) != null;
      return hasMin && hasMax;
    }

    private string Format(object value)
    {
      if (value is bool)
        return (((bool)value) ? "1" : "0");

      if (value is DateTime)
        return ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:sszzz");

      return value?.ToString();
    }

    private IEnumerable<Route> EnumerateTrail()
    {
      var route = this;
      while (route != null)
      {
        yield return route;
        route = route.parent;
      }
    }

    private IEnumerable<string> EnumeratePath()
    {
      return EnumerateTrail().Select(x => x.paths).NonEmpty().Reverse().SelectMany();
    }

    private IEnumerable<Arg> EnumerateArg()
    {
      return
        EnumerateTrail()
          .Select(x =>
            x.args
              .NonNull()
              .GroupBy(arg => arg.Name)
              .Select(g =>
                {
                  if (g.Key.EndsWith("[]"))
                  {
                    return new Arg
                    {
                      Name = g.Key.Replace("[]", ""),
                      Value = g.Select(arg => arg.Value).ToArray()
                    };
                  }
                  else
                  {
                    return g.Last();
                  }
                }
              )
          )
          .NonEmpty()
          .Reverse()
          .SelectMany()
          .GroupBy(arg => arg.Name)
          .Select(g => g.Last());
    }

    /// <summary>
    /// Utilitário para quebrar um caminho na interrogação (?) e retornar
    /// as duas partes, o caminho do recurso e a coleção de argumentos de URI
    /// como um vetor do tipo Arg.
    /// 
    /// Os itens retornados serão:
    /// -   string, para o caminho do recurso
    /// -   Arg, para cada argumento extraído da URI.
    /// </summary>
    /// <param name="path">O caminho a ser analisado.</param>
    /// <returns>
    /// Uma coleção de objetos do tipo string, para o caminho do recurso, e
    /// Arg, para cada argumento extraído da URI.
    /// </returns>
    private IEnumerable<object> EnumerateParts(string path)
    {
      if (path == null)
        yield break;

      var tokens = path.Split('?');
      var resource = tokens.First();
      var args = tokens.Skip(1).FirstOrDefault();

      if (!string.IsNullOrEmpty(resource))
      {
        yield return resource;
      }

      if (args != null)
      {
        var items =
          from pair in args.Split('&')
          let parts = pair.Split('=')
          let name = parts.First()
          let value = parts.Skip(1).FirstOrDefault()
          select new Arg
          {
            Name = name,
            Value = value
          };
        foreach (var item in items)
        {
          yield return item;
        }
      }
    }

    #region Igualdades e conversões

    public override bool Equals(object obj)
      => this.Text.Equals(obj);

    public override int GetHashCode()
      => this.Text.GetHashCode();

    public static implicit operator string(Route route)
      => route.Text;

    public static implicit operator Route(string route)
      => new Route(route);

    public static implicit operator Uri(Route route)
      => route.ToUri();

    public static implicit operator Route(Uri route)
      => new Route(route);

    #endregion

    private struct Arg
    {
      public string Name;
      public object Value;

      public override string ToString()
      {
        return $"{Name}={Value}";
      }
    }
  }
}
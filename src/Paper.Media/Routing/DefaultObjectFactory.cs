using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Implementação básica da fábrica de objetos.
  /// A fábrica detecta os parâmetros do objeto e encontra as
  /// instâncias registradas mais apropriadas para satisfazê-los.
  /// </summary>
  public class DefaultObjectFactory : IObjectFactory
  {
    private readonly Dictionary<Type, object> cache;

    public DefaultObjectFactory()
    {
      cache = new Dictionary<Type, object>();
    }

    /// <summary>
    /// Registra uma nova instância na fábrica de objetos.
    /// </summary>
    /// <typeparam name="T">O tipo para o qual a instância será registrada.</typeparam>
    /// <param name="instance">A instância a ser registrada.</param>
    public void Add<T>(T instance)
    {
      Add(typeof(T), instance);
    }

    /// <summary>
    /// Registra uma nova instância na fábrica de objetos.
    /// </summary>
    /// <param name="type">O tipo para o qual a instância será registrada.</param>
    /// <param name="instance">A instância a ser registrada.</param>
    public DefaultObjectFactory Add(Type type, object instance)
    {
      cache[type] = instance;
      return this;
    }

    /// <summary>
    /// Cria uma instância do objeto.
    /// A fábrica de objetos resolve os parâmetros do construtor.
    /// </summary>
    /// <param name="type">O tipo a ser instanciado.</param>
    /// <param name="args">
    /// Os argumentos providos.
    /// Os demais argumentos serão inferidos pela fábrica de objetos.
    /// </param>
    /// <returns>A instância criada.</returns>
    public object CreateInstance(Type type, params object[] args)
    {
      var ctors = type.GetConstructors();

      var publicCtors = ctors.Where(x => x.IsPublic);
      if (publicCtors.Count() != 1)
      {
        throw new NotImplementedException(
          "O tipo instanciado deve ter um único construtor público declarado: " + type.FullName
        );
      }

      var ctor = publicCtors.Single();
      var ctorArgs = CreateArguments(ctor.GetParameters(), args).ToArray();

      var instance = ctor.Invoke(ctorArgs);

      return instance;
    }

    /// <summary>
    /// Constrói a lista de valores para os argumentos resolvendo tipos
    /// a partir dos objetos em cache e da lista provida pelo chamador.
    /// </summary>
    /// <param name="parameters">Lista dos argumentos.</param>
    /// <param name="providedArgs">Objetos providos pelo chamador.</param>
    /// <returns>Lista de valores criados para os argumentos.</returns>
    private IEnumerable<object> CreateArguments(ParameterInfo[] parameters, object[] providedArgs)
    {
      var availables = new List<object>(providedArgs);
      foreach (var parameter in parameters)
      {
        var type = parameter.ParameterType;

        object provided;

        //
        // Tipos exatos
        //

        provided = availables.FirstOrDefault(x => x.GetType() == type);
        if (provided != null)
        {
          availables.Remove(provided);
          yield return provided;
          continue;
        }

        if (cache.ContainsKey(type))
        {
          var resolved = cache[type];
          yield return resolved;
          continue;
        }

        //
        // Tipos por herança
        //

        provided = availables.FirstOrDefault(x => type.IsAssignableFrom(x.GetType()));
        if (provided != null)
        {
          availables.Remove(provided);
          yield return provided;
          continue;
        }

        var resolvedType = cache.Keys.FirstOrDefault(x => type.IsAssignableFrom(x));
        if (resolvedType != null)
        {
          var resolved = cache[resolvedType];
          yield return resolved;
          continue;
        }

        var defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
        yield return defaultValue;
      }

      if (availables.Any())
      {
        throw new Exception("Não existe construtor que satisfaça todos os argumentos indicados.");
      }
    }
  }
}
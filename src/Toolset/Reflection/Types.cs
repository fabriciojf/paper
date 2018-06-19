using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Reflection
{
  public static class Types
  {
    private static Assembly[] assemblies;

    /// <summary>
    /// Tenta obter uma instância do tipo usando diferentes estatégias.
    /// </summary>
    /// <remarks>
    /// Em geral é esperado que o nome do tipo seja qualificado com o nome do seu
    /// assembly para uma recuperação nativa da instância, como em:
    /// 
    ///     My.Domain.User, MyAssembly
    ///     
    /// Porém, nos casos em que o tipo não é qualificado com o assembly mas o assembly
    /// é nomeado segundo a convenção de um assembly por namespace, o algoritmo infere
    /// o nome do assembly e obtem a instância do tipo.
    /// 
    /// Por exemplo:
    /// 
    ///     My.Domain.User
    /// 
    /// Será procurado em dois assemblies, nesta ordem:
    /// 
    ///     My.Domain.dll
    ///     My.dll
    ///     
    /// Se algum dos assemblies existir e tiver a declaração do tipo a instância será
    /// lida a partir dele.
    /// </remarks>
    /// <param name="typeName">O nome do tipo procurado.</param>
    /// <returns>A instãncia do tipo se encontrada.</returns>
    public static Type FindType(string typeName)
    {
      var type = Type.GetType(typeName);
      if (type != null)
        return type;

      IEnumerable<string> tokens = typeName.Split('.');
      var count = tokens.Count();

      while (--count > 0)
      {
        var assemblyName = string.Join(".", tokens.Take(count));
        var searchName = typeName + ", " + assemblyName;

        type = Type.GetType(searchName);
        if (type != null)
          return type;
      }

      return null;
    }

    public static IEnumerable<Type> FindTypes<T>()
      where T : Attribute
    {
      if (assemblies == null)
      {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var appPath = System.IO.Path.GetDirectoryName(assembly.Location);

        assemblies =
          Directory
            .GetFiles(appPath, "*.dll", SearchOption.TopDirectoryOnly)
            .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
            .ToArray();
      }

      var types =
        from assembly in assemblies
        from type in assembly.GetTypes()
        where type.GetCustomAttribute<T>() != null
        select type;

      return types;
    }
  }
}

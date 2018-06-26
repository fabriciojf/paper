using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Toolset
{
  /// <summary>
  /// Utilitário para descoberta dinâmica de classes expostos com
  /// <see cref="ExposeAttribute"></see>
  /// </summary>
  public static class ExposedTypes
  {
    private static Assembly[] _assemblies;

    private static Assembly[] Assemblies
    {
      get
      {
        if (_assemblies == null)
        {
          var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
          var appPath = System.IO.Path.GetDirectoryName(assembly.Location);

          _assemblies =
            Directory
              .GetFiles(appPath, "*.dll", SearchOption.TopDirectoryOnly)
              .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
              .ToArray();

          Console.WriteLine("--assemblies--");
          Console.WriteLine(string.Join(Environment.NewLine, _assemblies.Select(x => x.FullName)));
          Console.WriteLine("----");

        }
        return _assemblies;
      }
    }

    /// <summary>
    /// Obtém todos os tipos que correspondem ao contrato indicado e
    /// que são expostos pelo atributo <see cref="ExposeAttribute"/>
    /// </summary>
    /// <param name="contractName">Nome do contrato.</param>
    /// <param name="contractType">Tipo do alvo procurado.</param>
    /// <returns>
    /// Todas as instâncias expostos que implementam ou estendem o tipo.
    /// </returns>
    public static IEnumerable<Type> GetTypes(string contractName, Type contractType)
    {
      return
        from assembly in Assemblies
        from type in assembly.GetTypes()
        from attribute in type.GetCustomAttributes().OfType<ExposeAttribute>()
        where (contractName == null) || attribute.ContractName == contractName
        where (contractType == null) || contractType.IsAssignableFrom(type)
        select type;
    }

    /// <summary>
    /// Obtém todos os tipos que implementam ou estendem o tipo indicado e
    /// que são expostos pelo atributo <see cref="ExposeAttribute"/>
    /// </summary>
    /// <param name="contractName">Nome do contrato.</param>
    /// <returns>
    /// Todas as instâncias expostos que implementam ou estendem o tipo.
    /// </returns>
    public static IEnumerable<Type> GetTypes(string contractName)
      => GetTypes(contractName, null);

    /// <summary>
    /// Obtém todos os tipos que implementam ou estendem o tipo indicado e
    /// que são expostos pelo atributo <see cref="ExposeAttribute"/>
    /// </summary>
    /// <param name="contractType">O tipo procurado.</param>
    /// <returns>
    /// Todas as instâncias expostos que implementam ou estendem o tipo.
    /// </returns>
    public static IEnumerable<Type> GetTypes(Type contractType)
      => GetTypes(null, contractType);

    /// <summary>
    /// Obtém todos os tipos que implementam ou estendem o tipo indicado e
    /// que são expostos pelo atributo <see cref="ExposeAttribute"/>
    /// </summary>
    /// <typeparam name="T">O tipo procurado.</typeparam>
    /// <returns>
    /// Todas as instâncias expostos que implementam ou estendem o tipo.
    /// </returns>
    public static IEnumerable<Type> GetTypes<T>()
      => GetTypes(null, typeof(T));
  }
}
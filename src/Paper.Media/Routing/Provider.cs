using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Classe de acesso ao provedor padrão do Paper.
  /// A implementação do Provedor pode variar de HOST para HOST, segundo a tecnologia
  /// adotada.
  /// Esta classe oferece um método para obter ou definir este provedor padrão.
  /// </summary>
  public static class Provider
  {
    static Provider()
    {
      try
      {
        var type = ExposedTypes.GetTypes<IProvider>().FirstOrDefault();
        if (type != null)
        {
          Current = (IProvider)Activator.CreateInstance(type);
        }
      }
      catch (Exception ex)
      {
        ex.Trace();
      }
    }

    /// <summary>
    /// Instância padrão do provedor do Paper.
    /// </summary>
    public static IProvider Current
    {
      get;
      set;
    }
  }
}
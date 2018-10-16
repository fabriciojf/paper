using System;
using System.Collections.Generic;
using System.Text;
using Toolset.Collections;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Utilitário de pesquisa de catálogos do Paper.
  /// 
  /// Cada site conectado ao Paper possui seu próprio catálogo.
  /// Este utilitário pode ser usado para resolver o catálogo mais apropriado
  /// para cada rota.
  /// </summary>
  internal class DefaultCatalogSearcher : ICatalogSearcher
  {
    private readonly HashMap<ICatalog> catalogs;

    public DefaultCatalogSearcher()
    {
      catalogs = new HashMap<ICatalog>();
      catalogs[""] = new LocalCatalog();
    }

    /// <summary>
    /// Pesquisa o catálogo do site registrado no caminho indicado.
    /// O caminho indica a raiz de um site registrado no Paper.
    /// Caso o caminho não aponte para a raiz de um site nenhum catálogo é retornado.
    /// </summary>
    /// <param name="path">
    /// O caminho de registro de um site conectado ao Paper, na forma "/Caminho/Site".
    /// </param>
    /// <returns>O catálogo mais apropriado para renderizar Papers da rota.</returns>
    public ICatalog SearchCatalog(string path)
    {
      path = SanitizePath(path);
      return catalogs[path];
    }

    /// <summary>
    /// Registra um catalogo.
    /// </summary>
    /// <param name="path">
    /// Caminho de registro do catálogo.
    /// É esperado que o caminho seja referente a um site contecato ao Paper,
    /// na forma: /Caminho/Do/Site
    /// </param>
    /// <param name="catalog">O catálogo registrado.</param>
    public void AddCatalog(string path, ICatalog catalog)
    {
      path = SanitizePath(path);
      catalogs[path] = catalog;
    }

    /// <summary>
    /// Padroniza o caminho para uso interno.
    /// </summary>
    /// <param name="path">O caminho original do catálogo.</param>
    /// <returns>O caminho interno do catálogo.</returns>
    private string SanitizePath(string path)
    {
      if (path.StartsWith("/"))
      {
        path = path.Substring(1);
      }
      if (path.EndsWith("/"))
      {
        path = path.Substring(0, path.Length - 1);
      }
      return path.ToLower();
    }
  }
}
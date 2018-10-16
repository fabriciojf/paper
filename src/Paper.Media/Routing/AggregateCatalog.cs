using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Toolset.Collections;
using Paper.Media.Design;

namespace Paper.Media.Routing
{
  /// <summary>
  /// Agregador de catálogos do Paper.
  /// O agregador permite a resolução de Papers a partir de diferentes
  /// fontes. A pesquisa considera a ordem de adição dos catálogos.
  /// </summary>
  public class AggregateCatalog : ICatalog
  {
    private IEnumerable<ICatalog> catalogs;

    public AggregateCatalog()
    {
      this.catalogs = Enumerable.Empty<ICatalog>();
    }

    public AggregateCatalog(IEnumerable<ICatalog> catalogs)
    {
      this.catalogs = catalogs;
    }

    /// <summary>
    /// Adiciona uma catálogo do Paper.
    /// </summary>
    /// <param name="catalog">Catalogo do Paper.</param>
    public void Add(ICatalog catalog)
    {
      catalogs = catalogs.Append(catalog);
    }

    /// <summary>
    /// Obtém o Paper apropriado para a rota indicada.
    /// </summary>
    /// <param name="route">A rota pesquisada na forma: "/Rota/Subrota/...</param>
    /// <returns>A classe de referência do Paper.</returns>
    public PaperBlueprint GetPaperBlueprint(string route)
    {
      var instance = (
        from catalog in catalogs
        let paper = catalog.GetPaperBlueprint(route)
        where paper != null
        select paper
      ).FirstOrDefault();
      return instance;
    }
  }
}
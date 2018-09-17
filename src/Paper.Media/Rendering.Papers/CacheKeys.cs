using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Rendering.Papers
{
  /// <summary>
  /// Coleção das chaves de cache conhecidas.
  /// </summary>
  static class CacheKeys
  {
    /// <summary>
    /// Registro de dados consultados durante a renderização da página.
    /// </summary>
    public const string Data = "Data";

    /// <summary>
    /// Coleção dos registros consultados durante a renderização da página.
    /// </summary>
    public const string Rows = "Rows";

    /// <summary>
    /// Coleção dos registros consultados durante a renderização da página.
    /// </summary>
    public const string Cards = "Cards";

    /// <summary>
    /// Determina se ainda existem mais páginas de registro no lado do servidor.
    /// </summary>
    public const string HasMoreData = "HasMoreData";

    /// <summary>
    /// Instância do paginador de registros caso exista.
    /// </summary>
    public const string Page = "RowsPage";
  }
}
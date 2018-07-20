using System;
using System.Collections.Generic;
using System.Text;

namespace Media.Design.Extensions.Papers.Rendering
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
    /// Determina se ainda existem mais páginas de registro no lado do servidor.
    /// </summary>
    public const string HasMoreRows = "HasMoreRows";

    /// <summary>
    /// Instância do paginador de registros caso exista.
    /// </summary>
    public const string RowsPage = "RowsPage";
  }
}
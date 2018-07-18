using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Papers.Rendering
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
  }
}
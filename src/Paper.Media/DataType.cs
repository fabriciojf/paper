using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paper.Media
{
  /// <summary>
  /// Tipos de dados conhencidos.
  /// </summary>
  public enum DataType
  {
    /// <summary>
    /// Tipo para campo texto.
    /// Nomes alternativos:
    /// - string
    /// </summary>
    Text,

    /// <summary>
    /// Tipo para campo booliano.
    /// Nomes alternativos:
    /// - boolean
    /// </summary>
    Bit,

    /// <summary>
    /// Tipo para campo númerico inteiro, sem dígito.
    /// Nomes alternativos:
    /// - integer
    /// - int
    /// - long
    /// </summary>
    Number,

    /// <summary>
    /// Tipo para campo numérico fracionário, com dígito.
    /// Nomes alternativos:
    /// - double
    /// - float
    /// </summary>
    Decimal,

    /// <summary>
    /// Tipo para campo data somente, sem hora.
    /// </summary>
    Date,

    /// <summary>
    /// Tipo para campo hora somente, sem data.
    /// </summary>
    Time,

    /// <summary>
    /// Tipo para campo data/hora.
    /// </summary>
    Datetime,
  }
}
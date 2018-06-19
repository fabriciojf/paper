using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Tipo para opção com múltiplos argumentos.
  /// Quando presente na linha de comando:
  /// - On é marcado como verdadeiro.
  /// - Todos os argumentos seguintes encontrados na linha de comando
  ///   são postos em Items, até que um arugmento iniciado com '-' seja
  ///   encontrado.
  /// - Value contém todos os itens separados por espaço.
  /// </summary>
  public class OptArgArray : OptArg
  {
    private string[] items;
    public string[] Items
    {
      get { return items ?? (items = new string[0]); }
      set { items = value; }
    }
  }
}

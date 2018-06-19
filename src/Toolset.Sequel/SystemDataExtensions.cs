using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Sequel
{
  /// <summary>
  /// Coleção de extensões para os objetos do namespace System.Data;
  /// </summary>
  public static class SystemDataExtensions
  {
    /// <summary>
    /// Constrói um DbDataAdapter apropriado para a conexão indicada.
    /// </summary>
    /// <param name="connection">A instância da conexão.</param>
    /// <returns>O DbDataAdapter criado.</returns>
    public static DbDataAdapter CreateDataAdapter(this DbConnection connection)
    {
      var assembly = connection.GetType().Assembly;

      var typeName = connection.GetType().FullName.Replace("Connection", "DataAdapter");
      var type = assembly.GetType(typeName);
      if (type == null)
        throw new NotImplementedException("Não existe uma implementação conhecida de DbDataAdapter para " + connection.GetType().FullName);

      var adapter = (DbDataAdapter)Activator.CreateInstance(type);
      return adapter;
    }

    /// <summary>
    /// Constrói um DbDataAdapter apropriado para o comando indicado.
    /// O comando é atribuído à propriedade SelectCommando do DbDataAdapter.
    /// </summary>
    /// <param name="connection">A instância do comando.</param>
    /// <returns>O DbDataAdapter criado.</returns>
    public static DbDataAdapter CreateDataAdapter(this DbCommand command)
    {
      var adapter = CreateDataAdapter(command.Connection);
      adapter.SelectCommand = command;
      return adapter;
    }
  }
}

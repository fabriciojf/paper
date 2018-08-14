using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolset.Reflection;

namespace Toolset.Data
{
  public static class DataExtensions
  {
    /// <summary>
    /// Diz se um valor pode ser considerado nulo.
    /// O método considera o tipo DBNull como um valor nulo.
    /// </summary>
    /// <param name="value">O valor a ser verificado.</param>
    /// <returns>Verdadeiro se o valor pode ser considerado nulo.</returns>
    public static bool IsNull(this object value)
    {
      return (value == null)
          || (value == DBNull.Value)
          || (((value as IVar)?.IsNull) == true);
    }

    public static DbProviderFactory GetFactory(string provider)
    {
      switch (provider)
      {
        case "System.Data.SqlClient":
          return SqlClientFactory.Instance;

        case "Npgsql":
        case "MySql.Data.MySqlClient":
        case "System.Data.SQLite":
        case "System.Data.SqlServerCe.3.5":
        case "System.Data.SqlServerCe.4.0":
        case "FirebirdSql.Data.FirebirdClient":
        case "OleDb":
        default:
          throw new NotSupportedException("Provedor de base de dados ainda não suportado: " + provider);
      }
    }

    public static DbDataAdapter CreateDataAdapter(this DbConnection connection)
    {
      try
      {
        var typeName = connection.GetType().FullName.Replace("Connection", "DataAdapter");

        var type = Types.FindType(typeName);
        if (type == null)
          throw new NotSupportedException("O adaptador para consulta a base de dados não existe: " + typeName);

        var adapter = (DbDataAdapter)Activator.CreateInstance(type);
        return adapter;
      }
      catch (Exception ex)
      {
        throw new NotSupportedException("Não há suporte a consulta de nfce para o driver: " + connection.GetType().FullName, ex);
      }
    }

    public static DbDataAdapter CreateDataAdapter(this DbCommand command)
    {
      try
      {
        var typeName = command.GetType().FullName.Replace("Command", "DataAdapter");

        var type = Types.FindType(typeName);
        if (type == null)
          throw new NotSupportedException("O adaptador para consulta a base de dados não existe: " + typeName);

        var adapter = (DbDataAdapter)Activator.CreateInstance(type);
        adapter.SelectCommand = command;
        return adapter;
      }
      catch (Exception ex)
      {
        throw new NotSupportedException("Não há suporte a consulta de nfce para o driver: " + command.GetType().FullName, ex);
      }
    }
  }
}

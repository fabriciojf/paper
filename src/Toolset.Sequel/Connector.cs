using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using Toolset.Data;

namespace Toolset.Sequel
{
  public static class Connector
  {
    public const string SqlServerProvider = "System.Data.SqlClient";
    public const string SqlServerCeProvider = "System.Data.SqlServerCe.3.5";

    public static DbConnection Connect(string configuration)
    {
      string connectionString = null;
      string connectionProvider = null;

      ExtractConnectionStringInfo(configuration, out connectionProvider, out connectionString);
      
      var factory = DataExtensions.GetFactory(connectionProvider);

      OptimizeConnectionString(factory, ref connectionString);

      var connection = factory.CreateConnection();
      connection.ConnectionString = connectionString;
      connection.Open();

      OptimizeConnection(connection);

      return connection;
    }

    private static void ExtractConnectionStringInfo(
        string configuration
      , out string connectionProvider
      , out string connectionString
      )
    {

      // Se temos um nome de arquivo com extensão .sdf então temos uma string de conexão
      // do SQLServer Compact Edition.
      //
      var isSqlServerCe = configuration.Contains(".sdf");
      if (isSqlServerCe)
      {
        connectionString = configuration;
        connectionProvider = SqlServerCeProvider;
        return;
      }

      // Se temos uma coleção de chave=valor então temos uma string de conexão.
      // Por padrão o provedor escolhido será o SQLServer
      //
      var isSqlServer = configuration.Contains("=");
      if (isSqlServer)
      {
        connectionString = configuration;
        connectionProvider = SqlServerProvider;
        return;
      }

      // Caso contrário vamos entender a configuração como sendo o nome de uma
      // seção <connectionString> no App.config.
      //
      // TODO: XXX: Implementar uma solução.
      throw new NotImplementedException("Não é possível determinar a string de conexão.");
      //var connectionStringInfo = App.Connections[configuration];
      //if (connectionStringInfo == null)
      //{
      //  var message = $"String de conexão não encontrada no arquivo de configuração: \"{configuration}\"";
      //  var domain = App.User?.Domain;
      //  if (domain != null)
      //  {
      //    message += $", para o domínio: \"{domain}\"";
      //  }
      //  throw new SequelException(message);
      //}
      //
      //connectionString = connectionStringInfo.ToString();
      //connectionProvider = connectionStringInfo.Provider ?? SqlServerProvider;
    }

    private static void OptimizeConnectionString(DbProviderFactory factory, ref string connectionString)
    {
      if (factory is SqlClientFactory)
      {
        var builder = new SqlConnectionStringBuilder(connectionString);
        
        builder.MultipleActiveResultSets = true;
        builder.ApplicationName = SequelSettings.DefaultApplicationName ?? builder.ApplicationName;

        connectionString = builder.ToString();
      }
    }

    private static void OptimizeConnection(DbConnection connection)
    {
      if (!(connection is SqlConnection))
      {
        // Há apenas otimizações para SqlServer até o momento.
        return;
      }

      //
      // Nota sobre 'ARITHABORT ON':
      //
      // A configuração padrão ARITHABORT de SQL Server Management Studio é ON.
      // Os aplicativos cliente que definem ARITHABORT como OFF podem receber planos de
      // consulta diferentes, dificultando a solução de problemas de consultas executadas
      // insatisfatoriamente. Ou seja, a mesma consulta pode ser executada rapidamente no
      // Management Studio, mas lentamente no aplicativo. Ao solucionar problemas de
      // consultas com Management Studio, sempre faça a correspondência com a configuração
      // ARITHABORT do cliente.
      //
      // Referência:
      // -   https://msdn.microsoft.com/pt-br/library/ms190306.aspx
      //

      using (var command = connection.CreateCommand())
      {
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText =
        @"set ansi_null_dflt_on on
          set ansi_nulls on
          set ansi_padding on
          set ansi_warnings on
          set arithabort on
          set concat_null_yields_null on
          set cursor_close_on_commit off
          set deadlock_priority normal
          set implicit_transactions off
          set lock_timeout -1
          set nocount on
          set query_governor_cost_limit 0
          set quoted_identifier on
          set rowcount 0
          set textsize 2147483647
          set transaction isolation level read committed
          ";
        command.ExecuteNonQuery();
      }
    }

  }
}

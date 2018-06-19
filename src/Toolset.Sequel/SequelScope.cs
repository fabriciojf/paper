using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Toolset;
using System.ComponentModel;

namespace Toolset.Sequel
{
  /// <summary>
  /// Escopo de utilização de uma conexão.
  /// 
  /// Visão Geral do SEQUEL
  /// ---------------------
  /// 
  /// Sequel é uma API de simplificação do uso de SQLs dentro de códigos C#,
  /// aplicando uma sintaxe mais legível para composição e parametrização das
  /// consultas.
  /// 
  /// A API é uma série de métodos de extensão obtidos pela conversão de
  /// string para o tipo Sql, pelo método AsSql():
  /// 
  ///     Sql sql = "select * from usuarios".AsSql()
  /// 
  /// Em cima da instância de Sql obtida uma série de métodos de extensão
  /// permite a manipulação da SQL, como a parametrização, a reescrita, a
  /// execução e a obtenção do resultado em diversos formatos diferentes.
  /// 
  /// Por exemplo, no exemplo abaixo o nome do usuário está sendo obtido
  /// pela parametrização da SQL e conversão do resultado:
  /// 
  ///     var nome =
  ///       @"select nome
  ///           from usuarios
  ///          where login = @login"
  ///         .AsSql()
  ///         .Set("login", "fulano")
  ///         .SelectOne()
  /// 
  /// Sobre o SequelScope
  /// -------------------
  /// 
  /// A API do Sequel, em cima da classe Sql, exige um escopo de conexão do
  /// sequel.
  /// 
  /// O escopo mantém informação sobre as conexões vigentes e garante a
  /// facilidade de uso da API pela inferência dos parâmetros repassados
  /// de uma chamada para outra.
  /// 
  /// Cada escopo deve ser obrigatoriamente englobado por um bloco "using"
  /// do C# e aberto e fechado no mesmo método.
  /// 
  /// Cada método que faz uso da API do Sequel deve ter seu próprio escopo
  /// de conexão. O Sequel se encarrega de compartilhar a conexão vigente
  /// entre os escopos.
  /// 
  /// De uma forma geral, apenas o primeiro escopo inciado provoca uma
  /// abertura de conexão com a base. O escopos abertos dentro de outro
  /// escopo compartiha a sua conexão.
  /// 
  /// Exemplo de uso:
  /// ---------------
  /// 
  ///     using (var scope = new SequelScope("conexao"))
  ///     {
  ///       var reader =
  ///         @"select *
  ///             from usuarios
  ///            where regiao = @regiao"
  ///           .AsSql()
  ///           .Set("regiao", "Minas Gerais")
  ///           .Select()
  ///           
  ///       using (reader)
  ///       {
  ///         ...
  ///       }
  ///     }
  /// 
  /// </summary>
  public partial class SequelScope : SequelConnectionScope
  {
    public SequelScope()
    {
      InitializeScope(null);
    }

    public SequelScope(string configuration)
    {
      InitializeScope(null, configuration);
    }

    public SequelScope(DbConnection connection, bool keepOpen = true)
    {
      InitializeScope(null, connection, keepOpen);
    }

    public SequelScope(Func<DbConnection> connectionFactory)
    {
      var connection = connectionFactory.Invoke();
      InitializeScope(null, connection, false);
    }

    /// <summary>
    /// Utilitário para obtenção da conexão no escopo corrente.
    /// Quando fora de um contexto SequelScope nulo é retornado.
    /// </summary>
    public static DbConnection ScopedConnection
    {
      get
      {
        var scope = CurrentScope;
        return (scope != null) ? scope.Connection : null;
      }
    }
  }
}
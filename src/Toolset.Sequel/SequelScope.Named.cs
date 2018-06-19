using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Toolset;
using System.Threading;
using System.Diagnostics;

namespace Toolset.Sequel
{
  public partial class SequelScope
  {
    /// <summary>
    /// Escopo de conexão compartilhada.
    /// 
    /// Sobre o SequelScope
    /// -------------------
    /// 
    /// O escopo padrão do Sequel, o SequelScope, permite a abertura de uma
    /// nova conexão para cada thread interessada, sem limitar a quantidade
    /// máxima de conexões abertas.
    /// 
    /// Este cenário é ideal para ambientes multi-usuário, onde cada usuário
    /// é isolado em sua própria thread e recebe o direito de se conectar com
    /// a base de dados.
    /// 
    /// Sobre os Escopos Compartilhados
    /// -------------------------------
    /// 
    /// Os escopos compartilhados, o SequelScope.Named e o SequelScope.Shared, ainda
    /// permitem a abertura ilimitada de conexões para configurações diferentes mas
    /// limitam o número de conexões abertas para uma mesma configuração.
    /// 
    /// Uma configuração identifica uma base de dados específica, por tanto, multiplas
    /// conexões anda são permitidas para bases diferentes mas são limitadas para uma
    /// mesma base de dados.
    /// 
    /// A configuração é identificada pelo nome do escopo e pela configuração de
    /// conexão.
    /// 
    /// A configuração de conexão pode ser a string de conexão ou o nome da string de
    /// conexão na base de dados.
    /// 
    /// SequelScope.Shared
    /// ------------------
    /// 
    /// Este escopo permite a criação do escopo compartilhado padrão.
    /// 
    /// Escopos compartilhados padrão compartilham o limite de conexão entre si mas não
    /// sofrem limitação pelos demais escopos.
    /// 
    /// SequelScope.Named
    /// -----------------
    /// 
    /// Este escopo permite a criação do escopo nomeado.
    /// 
    /// Escopos com o mesmo nome compartilham o limite de conexão entre si mas não
    /// sofrem limitação pelos demais escopos.
    /// </summary>
    public partial class Named : SequelConnectionScope
    {
      /// <summary>
      /// Tempo de espera para nova verificação de disponibilidade de conexões.
      /// Quando não há conexões disponíveis a thread dorme este tempo atá uma nova
      /// tentativa de obter conexão.
      /// </summary>
      private const int WaitTime = 1;

      private static Dictionary<string, int> references = new Dictionary<string, int>();
      
      /// <summary>
      /// Trava da contagem de referências de conexão.
      /// O sistema usa a contagem de referências para determinar quando há uma conexão
      /// disponível.
      /// </summary>
      private static readonly object referencesLock = new object();

      /// <summary>
      /// Trava de threads interessadas em novas conexões.
      /// O sistema usa esta trava para criar uma fila de espera, enquanto o algoritmo
      /// procura por uma conexão liberada.
      /// </summary>
      private static readonly object queueLock = new object();

      /// <summary>
      /// Nome da configuração usada pelo escopo para obtenção da conexão.
      /// </summary>
      private string configuration;

      public Named(string scopeName)
      {
        // Escopo sem configuração é considerado um escopo aninhado, isto é, deve ser
        // aberto dentro de um escopo previamente criado.
        //
        // Caso esta condição não seja satisfeita o próprio método InitializeScope
        // lançará uma exceção.
        InitializeScope(scopeName);
      }

      public Named(string scopeName, string configuration)
      {
        this.configuration = configuration;
        InitializeScope(scopeName, configuration);
      }
    }
  }
}
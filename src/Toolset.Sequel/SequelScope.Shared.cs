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
    public partial class Shared : Named
    {
      internal const string SharedScopeName = "__shared__";

      public Shared()
        : base(SharedScopeName)
      {
      }

      public Shared(string configuration)
        : base(SharedScopeName, configuration)
      {
      }
    }
  }
}
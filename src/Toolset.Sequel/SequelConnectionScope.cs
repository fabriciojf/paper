using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Toolset;

namespace Toolset.Sequel
{
  public abstract class SequelConnectionScope : IDisposable
  {
    [ThreadStatic]
    private static SequelConnectionScopeBag _bag;
    internal static SequelConnectionScopeBag ScopeBag
    {
      get { return _bag ?? (_bag = new SequelConnectionScopeBag()); }
    }
    
    [ThreadStatic]
    private static Stack<SequelConnectionScope> _stack;
    internal static Stack<SequelConnectionScope> ScopeStack
    {
      get { return _stack ?? (_stack = new Stack<SequelConnectionScope>(5)); }
    }
    internal static SequelConnectionScope CurrentScope
    {
      get { return ScopeStack.Count > 0 ? ScopeStack.Peek() : null; }
    }

    private DbConnection connection;
    private bool keepOpen;

    public string ScopeName { get; private set; }

    internal protected SequelConnectionScope()
    {
    }

    protected void InitializeScope(string scopeName)
    {
      this.ScopeName = scopeName;
      ScopeStack.Push(this);

      var primaryScope = ScopeBag[scopeName];
      if (primaryScope == null)
      {
        throw new NoConnectionException(
          "O construtor do SequelScope sem configuração ou nome de conexão deve ser executado "
        + "dentro de um escopo de SequelScope previamente aberto."
        );
      }
      InitializeConnection(primaryScope.connection, true);
    }

    protected void InitializeScope(string scopeName, string configuration)
    {
      this.ScopeName = scopeName;
      ScopeStack.Push(this);

      var connection = Connector.Connect(configuration);
      InitializeConnection(connection, false);
    }

    protected void InitializeScope(string scopeName, DbConnection connection, bool keepOpen)
    {
      this.ScopeName = scopeName;
      ScopeStack.Push(this);

      InitializeConnection(connection, keepOpen);
    }

    private void InitializeConnection(DbConnection connection, bool keepOpen)
    {
      this.connection = connection;

      if (this.connection.State == ConnectionState.Broken)
        this.connection.Close();
      if (this.connection.State != ConnectionState.Open)
        this.connection.Open();

      if (ScopeBag[ScopeName] == null)
        ScopeBag[ScopeName] = this;

      this.keepOpen = keepOpen;
    }

    public DbConnection Connection
    {
      get { return connection; }
    }

    public DatabaseScope CreateDatabaseScope(string database)
    {
      return new DatabaseScope(database);
    }

    public SequelTransactionScope CreateTransactionScope()
    {
      return new SequelTransactionScope(connection);
    }

    public virtual void Dispose()
    {
      try
      {

        if (!this.keepOpen)
        {
          try { this.connection.Close(); }
          catch { /* nada a fazer */ }

          try { this.connection.Dispose(); }
          catch { /* nada a fazer */ }
        }

        this.connection = null;

        var primaryScope = ScopeBag[ScopeName];
        if (primaryScope == this)
          ScopeBag[ScopeName] = null;

      }
      finally
      {
        ScopeStack.Pop();
      }
    }

    /// <summary>
    /// Utilitário para obtenção da conexão no escopo corrente.
    /// </summary>
    internal static DbConnection GetScopedConnection()
    {
      var scope = CurrentScope;
      if (scope == null)
      {
        throw new NoConnectionException(
          "O comando deve ser executando dentro de um escopo SequelScope()."
        );
      }
      return scope.Connection;
    }
  }
}

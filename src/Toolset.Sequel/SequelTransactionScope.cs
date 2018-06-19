using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Toolset;

namespace Toolset.Sequel
{
  public class SequelTransactionScope : IDisposable
  {
    [ThreadStatic]
    private static Dictionary<DbConnection, SequelTransactionScope> transactions;

    private readonly DbConnection connection;
    private readonly IDisposable transaction;
    private readonly bool handle;

    internal SequelTransactionScope(DbConnection connection)
    {
      this.connection = connection;

      var current = GetTransactionScopeFor(connection);
      if (current != null)
      {
        transaction = current.transaction;

        handle = false;
      }
      else
      {

        if (System.Transactions.Transaction.Current != null)
        {
          transaction = new System.Transactions.TransactionScope();
          connection.EnlistTransaction(System.Transactions.Transaction.Current);
          
          handle = true;
        }
        else
        {
          transaction = connection.BeginTransaction();

          handle = true;
        }

        if (transactions == null)
        {
          transactions = new Dictionary<DbConnection, SequelTransactionScope>();
        }
        transactions[connection] = this;
      }
    }

    public DbTransaction Transaction
    {
      get { return transaction as DbTransaction; }
    }

    internal static DbTransaction GetTransactionFor(DbConnection connection)
    {
      var scope = GetTransactionScopeFor(connection);
      return (scope != null) ? scope.Transaction : null;
    }

    internal static SequelTransactionScope GetTransactionScopeFor(DbConnection connection)
    {
      if (transactions == null)
        return null;
      if (!transactions.ContainsKey(connection))
        return null;
      return transactions[connection];
    }

    public void Complete()
    {
      if (handle)
      {
        if (transaction is System.Transactions.TransactionScope)
        {
          ((System.Transactions.TransactionScope)transaction).Complete();
        }
        else
        {
          ((DbTransaction)transaction).Commit();
        }
      }
    }

    public void Dispose()
    {
      if (handle)
      {
        try
        {
          transaction.Dispose();
        }
        catch (Exception ex)
        {
          ex.TraceWarning("Falhou a tentativa de destruir uma transação.");
        }

        if (transactions != null)
        {
          transactions.Remove(connection);
        }
      }
    }
  }
}

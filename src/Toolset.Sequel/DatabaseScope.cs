using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  public class DatabaseScope : IDisposable
  {
    internal DatabaseScope(string database)
    {
      this.Database = database;
      this.PriorDatabase = "select db_name()".AsSql().SelectOne<string>();

      if (this.Database != this.PriorDatabase)
      {
        ("use " + this.Database).AsSql().Execute();
      }
    }

    public string PriorDatabase
    {
      get;
      private set;
    }

    public string Database
    {
      get;
      private set;
    }

    public void Dispose()
    {
      if (this.Database != this.PriorDatabase)
      {
        ("use " + PriorDatabase).AsSql().Execute();
      }
    }
  }
}

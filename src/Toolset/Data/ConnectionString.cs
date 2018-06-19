using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Data
{
  public class ConnectionString
  {
    private readonly string _name;
    private readonly string _connectionString;
    private readonly string _provider;

    public ConnectionString(string name, string connectionString, string provider)
    {
      this._name = name;
      this._connectionString = connectionString;
      this._provider = provider;
    }

    public string Name
      => _name;

    public string Provider
      => _provider;

    public override string ToString()
      => _connectionString;

    public static implicit operator string(ConnectionString connectionString)
      => connectionString._connectionString;
  }
}

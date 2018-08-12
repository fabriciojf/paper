using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Toolset.Collections;

namespace Toolset.Sequel
{
  public class Sql : ICloneable
  {
    private readonly SequelScope scope;

    private HashMap _parameters;

    public Sql()
    {
    }

    public Sql(SequelScope sequelScope)
    {
      this.scope = sequelScope;
    }

    internal DbConnection Connection
      => scope?.Connection ?? SequelConnectionScope.GetScopedConnection();

    public string Text { get; set; }

    internal HashMap Parameters
      => _parameters ?? (_parameters = new HashMap ());

    public int ParameterCount
      => Parameters.Count;

    public ICollection<string> ParameterNames
      => Parameters.Keys;

    public object this[string parameterName]
    {
      get
      {
        parameterName =
          Parameters.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(parameterName))
          ?? parameterName;
        var value = Parameters[parameterName];
        return value;
      }
      set
      {
        parameterName =
          Parameters.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(parameterName))
          ?? parameterName;
        Parameters[parameterName] = value as Any ?? new Any(value);
      }
    }

    public Sql Unset(string parameterName)
    {
      Parameters.Remove(parameterName);
      return this;
    }

    public Sql UnsetAll()
    {
      Parameters.Clear();
      return this;
    }

    public override int GetHashCode()
      => (Text != null) ? Text.GetHashCode() : string.Empty.GetHashCode();

    public override bool Equals(object obj)
    {
      if (obj is string)
      {
        return obj.Equals(Text);
      }

      if (obj is Sql)
      {
        var otherText = ((Sql)obj).Text;
        return otherText == Text;
      }

      return false;
    }

    public override string ToString()
      => Text;

    public Sql Clone()
    {
      var clone = new Sql();
      clone.Text = this.Text;
      foreach (var key in this.Parameters.Keys)
      {
        var value = this.Parameters[key];
        clone.Parameters.Add(key, value);
      }
      return clone;
    }

    object ICloneable.Clone()
      => this.Clone();

    public static implicit operator string(Sql sql)
      => sql.Text;
  }
}

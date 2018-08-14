using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Toolset.Collections;
using Toolset.Reflection;

namespace Toolset.Sequel
{
  public class Sql : IParameterMap, ICloneable
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

    internal DbConnection Connection => scope?.Connection ?? SequelConnectionScope.GetScopedConnection();

    public string Text { get; set; }

    internal HashMap Parameters => _parameters ?? (_parameters = new HashMap());

    IDictionary<string, object> IParameterMap.Parameters => Parameters;

    public int ParameterCount => Parameters.Count;

    public ICollection<string> ParameterNames => Parameters.Keys;

    public object this[string name]
    {
      get
      {
        name =
          Parameters.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(name))
          ?? name;
        var value = Parameters[name];
        return value;
      }
      set
      {
        name = Parameters.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(name)) ?? name;

        if (!value.IsPrimitive())
        {
          if (value.IsEnumerable())
          {
            // Lista de objetos devem ser remapeados como lista de dicionarios
            //
            var list = ((IEnumerable)value).Cast<object>();
            var hasGraph = list.Any(x => x.IsGraph());
            if (hasGraph)
            {
              value = list.Select(item => item.IsGraph() ? item.UnwrapGraph() : item).ToArray();
            }
          }
          else if (value.IsGraph())
          {
            value = value.UnwrapGraph();
          }
        }

        Parameters[name] = value.IsPrimitive() ? value : (value as Any ?? new Any(value));
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
    {
      return (Text != null) ? Text.GetHashCode() : string.Empty.GetHashCode();
    }

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

    public override string ToString() => Text;

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

    object ICloneable.Clone() => this.Clone();

    public static implicit operator string(Sql sql) => sql.Text;
  }
}
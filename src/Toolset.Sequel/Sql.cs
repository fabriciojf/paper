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

    private KeyGen _keyGen;
    
    private Map<string, object> parameters;

    public Sql()
    {
    }

    public Sql(SequelScope sequelScope)
    {
      this.scope = sequelScope;
    }

    internal DbConnection Connection
      => scope?.Connection ?? SequelConnectionScope.GetScopedConnection();

    /// <summary>
    /// Gera uma chave única para ser adicionada à nome de parâmetros
    /// criados dinamicamente.
    /// </summary>
    /// <returns>A próxima chave para diferenciação de parâmetros dinâsmicos.</returns>
    internal KeyGen KeyGen
      => _keyGen ?? (_keyGen = new Toolset.Sequel.KeyGen());

    public string Text { get; set; }

    public int ParameterCount
      => ParameterCollection.Count;

    public ICollection<string> ParameterNames
      => ParameterCollection.Keys;

    public object this[string parameterName]
    {
      get => ParameterCollection[parameterName];
      set => ParameterCollection[parameterName] = Values.Create(value);
    }

    public Sql Unset(string parameterName)
    {
      ParameterCollection.Remove(parameterName);
      return this;
    }

    public Sql UnsetAll()
    {
      ParameterCollection.Clear();
      return this;
    }

    private Map<string, object> ParameterCollection
      => parameters ?? (parameters = new Map<string, object>());

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
      foreach (var key in this.ParameterCollection.Keys)
      {
        var value = this.ParameterCollection[key];
        clone.ParameterCollection.Add(key, value);
      }
      return clone;
    }

    object ICloneable.Clone()
      => this.Clone();

    public static implicit operator string(Sql sql)
      => sql.Text;
  }
}

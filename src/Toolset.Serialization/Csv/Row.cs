using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace Toolset.Serialization.Csv
{
  [CollectionDataContract]
  public class Row : List<Field>, IDictionary
  {
    public Row()
    {
    }

    public Row(IEnumerable<Field> fields)
    {
      this.AddRange(fields);
    }

    public Row(Field fields, params Field[] others)
    {
      this.Add(fields);
      this.AddRange(others);
    }

    public IEnumerable<string> FieldNames
    {
      get { return this.Select(x => x.Name); }
    }

    #region IDictionary<string, object>

    public bool IsFixedSize
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsReadOnly
    {
      get { throw new NotImplementedException(); }
    }

    public ICollection Keys
    {
      get { return this.Select(x => x.Name).ToArray(); }
    }

    public ICollection Values
    {
      get { return this.Select(x => x.Value).ToArray(); }
    }

    public object this[object key]
    {
      get { return this.Where(x => x.Name.Equals(key)).Select(x => x.Name).FirstOrDefault(); }
      set
      {
        var field = this.Where(x => x.Name.Equals(key)).FirstOrDefault();
        if (field != null)
        {
          field.Value = value;
        }
        else
        {
          this.Add(new Field(key.ToString(), value));
        }
      }
    }

    public void Add(object key, object value)
    {
      this.Add(new Field(key.ToString(), value));
    }

    public void Remove(object key)
    {
      this.RemoveAll(x => x.Name.Equals(key));
    }

    public bool Contains(object key)
    {
      return this.Any(x => x.Name.Equals(key));
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return new DictionaryEnumerator(this);
    }

    public class DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
    {
      private readonly IEnumerator<Field> fields;

      public DictionaryEnumerator(Row row)
      {
        this.fields = row.GetEnumerator();
      }

      public DictionaryEntry Entry
      {
        get
        {
          if (entry == null)
          {
            entry = new DictionaryEntry
            {
              Key = fields.Current.Name,
              Value = fields.Current.Value
            };
          }
          return entry.Value;
        }
      }
      private DictionaryEntry? entry;

      public object Current
      {
        get { return Entry; }
      }

      public object Key
      {
        get { return fields.Current.Name; }
      }

      public object Value
      {
        get { return fields.Current.Value; }
      }

      public bool MoveNext()
      {
        entry = null;
        return fields.MoveNext();
      }

      public void Reset()
      {
        fields.Reset();
      }

    }

    #endregion
  }
}

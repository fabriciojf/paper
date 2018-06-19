using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public class TableNavigator : IDisposable
  {
    private struct Field
    {
      public string Name;
      public object Value;
    }

    private readonly Reader tableReader;
    private readonly IEnumerator<IEnumerable<Field>> rows;
    private Field[] row;

    public TableNavigator(Reader reader)
    {
      this.tableReader = new TableTransformReader(reader);
      this.rows = EnumerateRows(this.tableReader).GetEnumerator();
    }

    public IEnumerable<string> Fields
    {
      get { return row.Select(x => x.Name); }
    }

    public IEnumerable<object> Values
    {
      get { return row.Select(x => x.Value); }
    }

    public T GetValue<T>(string fieldName)
    {
      var value = GetValue(fieldName);
      return (value is T) ? (T)value : default(T);
    }

    public object GetValue(string fieldName)
    {
      return (
        from field in row
        where field.Name.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase)
        select field.Value
        ).FirstOrDefault();
    }

    public T GetValue<T>(int fieldIndex)
    {
      var value = GetValue(fieldIndex);
      return (value is T) ? (T)value : default(T);
    }

    public object GetValue(int fieldIndex)
    {
      return row[fieldIndex].Value;
    }

    public bool MoveNext()
    {
      var ready = rows.MoveNext();
      row = ready ? rows.Current.ToArray() : null;
      return ready;
    }

    private IEnumerable<IEnumerable<Field>> EnumerateRows(Reader reader)
    {

      var fields = new Queue<Field>();
      string fieldName = null;

      while (reader.Read())
      {
        switch (reader.NodeType)
        {
          case NodeType.ObjectStart:
            {
              fields.Clear();
              break;
            }

          case NodeType.ObjectEnd:
            {
              yield return fields;
              break;
            }

          case NodeType.PropertyStart:
            {
              fieldName = (string)reader.NodeValue;
              break;
            }

          case NodeType.Value:
            {
              var fieldValue = reader.NodeValue;
              fields.Enqueue(new Field { Name = fieldName, Value = fieldValue });
              break;
            }
        }
      }
    }

    public void Close()
    {
      tableReader.Close();
    }

    public void Dispose()
    {
      Close();
    }
  }
}

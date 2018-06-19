using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public class MatrixNavigator : IDisposable
  {
    private readonly Reader tableReader;
    private readonly IEnumerator<IEnumerable<object>> rows;
    private object[] row;

    private int collectionDepth;

    public MatrixNavigator(Reader reader)
    {
      this.tableReader = new MatrixTransformReader(reader);
      this.rows = EnumerateRows(this.tableReader).GetEnumerator();
    }

    public int Count
    {
      get { return (row != null) ? row.Length : 0; }
    }

    public IEnumerable<object> Values
    {
      get { return row; }
    }

    public T GetValue<T>(int index)
    {
      var value = GetValue(index);
      return (value is T) ? (T)value : default(T);
    }

    public object GetValue(int index)
    {
      return row[index];
    }

    public bool MoveNext()
    {
      var ready = rows.MoveNext();
      row = ready ? rows.Current.ToArray() : null;
      return ready;
    }

    private IEnumerable<IEnumerable<object>> EnumerateRows(Reader reader)
    {
      var values = new Queue<object>();
      while (reader.Read())
      {
        switch (reader.NodeType)
        {
          case NodeType.CollectionStart:
            {
              collectionDepth++;
              if (collectionDepth == 2)
              {
                values.Clear();
              }
              break;
            }

          case NodeType.CollectionEnd:
            {
              if (collectionDepth == 2)
              {
                yield return values;
              }
              collectionDepth--;
              break;
            }

          case NodeType.Value:
            {
              values.Enqueue(reader.NodeValue);
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

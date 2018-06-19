using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public class MatrixCreator : IDisposable
  {
    private readonly Writer tableWriter;
    private readonly List<object> row;

    private bool initialized;
    private int lastIndex = -1;

    public MatrixCreator(Writer writer)
    {
      this.tableWriter = new MatrixTransformWriter(writer);
      this.row = new List<object>();
    }

    public MatrixCreator(Writer writer, string[] fields)
    {
      this.tableWriter = new TableTransformWriter(writer);
      this.row = new List<object>();
    }

    public object[] Values
    {
      get { return row.ToArray(); }
      set
      {
        row.Clear();
        row.AddRange(value);
      }
    }

    public void SetValue(int fieldIndex, object value)
    {
      while (fieldIndex >= row.Count)
      {
        row.Add(null);
      }
      row[fieldIndex] = value;
      lastIndex = fieldIndex;
    }

    public void SetValues(object value, params object[] others)
    {
      row.Clear();
      row.Add(value);
      row.AddRange(others);

      lastIndex = row.Count - 1;
    }

    public void SetValues(IEnumerable<object> values)
    {
      row.Clear();
      row.AddRange(values);

      lastIndex = row.Count - 1;
    }

    public void AddValue(object value)
    {
      SetValue(++lastIndex, value);
    }

    public void CreateRow()
    {
      if (!initialized)
      {
        initialized = true;
        tableWriter.WriteDocumentStart();
        tableWriter.WriteCollectionStart();
      }

      tableWriter.WriteCollectionStart();
      foreach (var value in row)
      {
        tableWriter.WriteValue(value);
      }
      tableWriter.WriteCollectionEnd();

      row.Clear();
      lastIndex = -1;
    }

    private void Complete()
    {
      if (initialized)
      {
        tableWriter.WriteCollectionEnd();
        tableWriter.WriteDocumentEnd();
      }
    }

    public void Close()
    {
      Complete();
      tableWriter.Close();
    }

    public void Dispose()
    {
      Close();
    }
  }
}

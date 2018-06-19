using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public class TableCreator : IDisposable
  {
    private class Field
    {
      public string Name;
      public object Value;
    }

    private readonly Writer tableWriter;
    private readonly List<Field> row;

    private bool initialized;
    private int lastIndex = -1;
    
    public TableCreator(Writer writer)
    {
      this.tableWriter = new TableTransformWriter(writer);
      this.row = new List<Field>();
    }

    public TableCreator(Writer writer, string[] fields)
    {
      this.tableWriter = new TableTransformWriter(writer);
      this.row = new List<Field>();
      this.Fields = fields;
    }

    public string[] Fields
    {
      get { return row.Select(x => x.Name).ToArray(); }
      set
      {
        row.Clear();

        var array = value;
        if (array != null)
        {
          for (var i = 0; i < array.Length; i++)
          {
            var field = GetOrCreateField(i);
            field.Name = array[i];
          }
        }
      }
    }

    public object[] Values
    {
      get { return row.Select(x => x.Value).ToArray(); }
      set
      {
        foreach (var field in row)
          field.Value = null;

        var array = value;
        if (array != null)
        {
          for (var i = 0; i < array.Length; i++)
          {
            var field = GetOrCreateField(i);
            field.Value = array[i];
          }
        }
      }
    }

    public void SetField(string fieldIndex, string fieldName)
    {
      var field = GetOrCreateField(fieldIndex);
      field.Name = fieldName;

      lastIndex = row.IndexOf(field);
    }

    public void SetValue(string fieldName, object value)
    {
      var field = GetOrCreateField(fieldName);
      field.Value = value;

      lastIndex = row.IndexOf(field);
    }

    public void SetValue(int fieldIndex, object value)
    {
      var field = GetOrCreateField(fieldIndex);
      field.Value = value;

      lastIndex = row.IndexOf(field);
    }

    public void SetValues(object value, params object[] others)
    {
      SetValues(new[] { value }.Union(others));
    }

    public void SetValues(IEnumerable<object> values)
    {
      var fieldIndex = -1;
      foreach (var value in values)
      {
        var field = GetOrCreateField(++fieldIndex);
        field.Value = value;
      }

      lastIndex = fieldIndex;
    }

    public void AddValue(object value)
    {
      SetValue(++lastIndex, value);
    }

    private Field GetOrCreateField(string fieldName)
    {
      var field = row.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase));
      if (field == null)
      {
        field = new Field { Name = fieldName };
        row.Add(field);
      }
      return field;
    }

    private Field GetOrCreateField(int fieldIndex)
    {
      while (fieldIndex >= row.Count)
      {
        var fieldName = "Field" + row.Count;
        row.Add(new Field { Name = fieldName });
      }
      return row[fieldIndex];
    }

    public void CreateRow()
    {
      if (!initialized)
      {
        initialized = true;
        tableWriter.WriteDocumentStart();
        tableWriter.WriteCollectionStart();
      }

      tableWriter.WriteObjectStart();
      foreach (var field in row)
      {
        tableWriter.WriteProperty(field.Name, field.Value);
        field.Value = null;
      }
      tableWriter.WriteObjectEnd();

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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  public class RecordResult : Result<Record>
  {
    public RecordResult(Func<DbCommand> factory)
      : base(factory, reader => new Record(reader))
    {
    }

    public int FieldCount
    {
      get { return (Reader != null) ? Reader.FieldCount : 0; }
    }

    public IEnumerable<int> Fields
    {
      get { return Enumerable.Range(0, FieldCount); }
    }

    public string GetFieldName(int fieldIndex)
    {
      if (Reader == null)
        return null;

      var name = Reader.GetName(fieldIndex);
      return name;
    }

    public int GetFieldIndex(string fieldName)
    {
      if (Reader == null)
        return -1;

      var index = Reader.GetOrdinal(fieldName);
      return index;
    }
  }
}

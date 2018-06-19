using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Toolset.Sequel
{
  public class Record : IDataRecord
  {
    private readonly IDataRecord record;

    public Record(IDataRecord record)
    {
      this.record = record;
    }

    public int FieldCount { get { return record.FieldCount; } }

    public object this[int i] { get { return record[i]; } }
    public object this[string name] { get { return record[name]; } }

    public bool GetBoolean(int i) { return record.GetBoolean(i); }
    public byte GetByte(int i) { return record.GetByte(i); }
    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) { return record.GetBytes(i, fieldOffset, buffer, bufferoffset, length); }
    public char GetChar(int i) { return record.GetChar(i); }
    public long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length) { return record.GetChars(i, fieldOffset, buffer, bufferoffset, length); }
    public IDataReader GetData(int i) { return record.GetData(i); }
    public string GetDataTypeName(int i) { return record.GetDataTypeName(i); }
    public DateTime GetDateTime(int i) { return record.GetDateTime(i); }
    public decimal GetDecimal(int i) { return record.GetDecimal(i); }
    public double GetDouble(int i) { return record.GetDouble(i); }
    public Type GetFieldType(int i) { return record.GetFieldType(i); }
    public float GetFloat(int i) { return record.GetFloat(i); }
    public Guid GetGuid(int i) { return record.GetGuid(i); }
    public short GetInt16(int i) { return record.GetInt16(i); }
    public int GetInt32(int i) { return record.GetInt32(i); }
    public long GetInt64(int i) { return record.GetInt64(i); }
    public string GetName(int i) { return record.GetName(i); }
    public int GetOrdinal(string name) { return record.GetOrdinal(name); }
    public string GetString(int i) { return record.GetString(i); }
    public object GetValue(int i) { return record.GetValue(i); }
    public int GetValues(object[] values) { return record.GetValues(values); }
    public bool IsDBNull(int i) { return record.IsDBNull(i); }
  }
}
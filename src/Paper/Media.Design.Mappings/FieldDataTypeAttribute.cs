using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Collections;

namespace Paper.Media.Design.Mappings
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FieldDataTypeAttribute : Attribute
  {
    public string[] DataTypes { get; }

    public FieldDataTypeAttribute(DataType dataType, params DataType[] otherDataTypes)
    {
      DataTypes = dataType.AsSingle().Union(otherDataTypes).Select(x => x.GetName()).ToArray();
    }

    public FieldDataTypeAttribute(string dataType, params string[] otherDataTypes)
    {
      DataTypes = dataType.AsSingle().Union(otherDataTypes).ToArray();
    }
  }
}
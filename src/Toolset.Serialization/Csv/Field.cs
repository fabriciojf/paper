using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Toolset.Serialization.Csv
{
  [DataContract]
  public class Field
  {
    public Field()
    {
    }

    public Field(string name, object value)
    {
      this.Name = name;
      this.Value = value;
    }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public object Value { get; set; }
  }
}

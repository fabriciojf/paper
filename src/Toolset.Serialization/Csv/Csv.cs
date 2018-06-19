using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Serialization.Json;
using System.Runtime.Serialization;

namespace Toolset.Serialization.Csv
{
  [CollectionDataContract]
  public class Csv : List<Row>
  {
    public Csv()
    {
    }

    public Csv(IEnumerable<Row> rows)
    {
      this.AddRange(rows);
    }

    public Csv(Row row, params Row[] others)
    {
      this.Add(row);
      this.AddRange(others);
    }
  }
}

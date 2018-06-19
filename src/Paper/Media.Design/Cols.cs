using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Design
{
  public class Cols : Collection<Col>
  {
    public Cols()
    {
    }

    public Cols(IEnumerable<Col> items)
      : base(items)
    {
    }

    public Col this[string name]
      => this.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));

    public Cols AddHidden(string name, string type = null)
    {
      Add(new Col { Name = name, Type = type });
      return this;
    }

    public Cols Add(string name, string title = null, string type = null, bool? hidden = null)
    {
      Add(new Col { Name = name, Title = title, Type = type, Hidden = hidden });
      return this;
    }
  }
}
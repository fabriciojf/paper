using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Papers
{
  public class Blueprint
  {
    private Details _info;

    public bool HasNavBox { get; set; }

    public string Theme { get; set; }

    public Details Info
    {
      get => _info ?? (_info = new Details());
      set => _info = value;
    }

    public class Details
    {
      public Guid Guid { get; set; }
      public string Name { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }
      public Version Version { get; set; }
    }
  }
}
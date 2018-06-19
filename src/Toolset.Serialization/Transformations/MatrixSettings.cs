using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Transformations
{
  public class MatrixSettings : SerializationSettings
  {
    public MatrixSettings()
      : base(new SerializationSettings())
    {
    }

    public MatrixSettings(SerializationSettings coreSettings)
      : base(coreSettings)
    {
    }

    public bool HasHeaders
    {
      get { return Get<bool>("HasHeaders"); }
      set { Set("HasHeaders", value); }
    }
  }
}

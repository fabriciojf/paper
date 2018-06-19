using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Serialization;
using System.Reflection;
using System.Globalization;

namespace Toolset.Serialization.Graph
{
  public class GraphSerializationSettings : SerializationSettings
  {
    public GraphSerializationSettings()
      : base(new SerializationSettings())
    {
      this.IgnoreNullProperties = true;
      this.IgnoreFalseProperties = true;
    }

    public GraphSerializationSettings(SerializationSettings coreSettings)
      : base(coreSettings)
    {
      this.IgnoreNullProperties = true;
      this.IgnoreFalseProperties = true;
    }

    public bool IgnoreFalseProperties
    {
      get { return Get<bool>("IgnoreFalseProperties"); }
      set { Set("IgnoreFalseProperties", value); }
    }

    public bool IgnoreDefaultProperties
    {
      get { return Get<bool>("IgnoreDefaultProperties"); }
      set { Set("IgnoreDefaultProperties", value); }
    }

    public bool IgnoreEmptyProperties
    {
      get { return Get<bool>("IgnoreEmptyProperties"); }
      set { Set("IgnoreEmptyProperties", value); }
    }

    public bool IgnoreNullProperties
    {
      get { return Get<bool>("IgnoreNullProperties"); }
      set { Set("IgnoreNullProperties", value); }
    }
  }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Serialization;

namespace Toolset.Serialization.Json
{
  public class JsonSerializationSettings : SerializationSettings
  {
    public JsonSerializationSettings()
      : base(new SerializationSettings())
    {
    }

    public JsonSerializationSettings(SerializationSettings coreSettings)
      : base(coreSettings)
    {
    }

    public bool Indent
    {
      get { return Get<bool>("Indent"); }
      set { Set("Indent", value); }
    }

    public string IndentChars
    {
      get { return Get<string>("IndentChars", "  "); }
      set { Set("IndentChars", value); }
    }

    public bool KeepOpen
    {
      get { return Get<bool>("KeepOpen"); }
      set { Set("KeepOpen", value); }
    }
  }
}
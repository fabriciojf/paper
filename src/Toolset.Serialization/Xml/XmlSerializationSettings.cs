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

namespace Toolset.Serialization.Xml
{
  public class XmlSerializationSettings : SerializationSettings
  {
    public XmlSerializationSettings()
      : base(new SerializationSettings())
    {

    }

    public XmlSerializationSettings(SerializationSettings coreSettings)
      : base(coreSettings)
    {
    }

    public bool AutoFlush
    {
      get { return Get<bool>("AutoFlush", true); }
      set { Set("AutoFlush", value); }
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
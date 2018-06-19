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

namespace Toolset.Serialization.Excel
{
  public class ExcelSerializationSettings : SerializationSettings
  {
    public ExcelSerializationSettings()
      : base(new SerializationSettings())
    {
      this.HasHeaders = true;
    }

    public ExcelSerializationSettings(SerializationSettings coreSettings)
      : base(coreSettings)
    {
      this.HasHeaders = true;
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

    public override string DateTimeFormat
    {
      get { return Get<string>("DateTimeFormat", "dd/MM/yyyy hh:mm:ss"); }
      set { Set("DateTimeFormat", value); }
    }

    public bool HasHeaders
    {
      get { return Get<bool>("HasHeaders"); }
      set { Set("HasHeaders", value); }
    }

    public bool KeepOpen
    {
      get { return Get<bool>("KeepOpen"); }
      set { Set("KeepOpen", value); }
    }
  }
}
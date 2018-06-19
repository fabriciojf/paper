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

namespace Toolset.Serialization.Csv
{
  public class CsvSerializationSettings : SerializationSettings
  {
    public CsvSerializationSettings()
      : base(new SerializationSettings())
    {
    }

    public CsvSerializationSettings(SerializationSettings coreSettings)
      : base(coreSettings)
    {
    }

    public bool HasHeaders
    {
      get { return Get<bool>("HasHeaders"); }
      set { Set("HasHeaders", value); }
    }

    public char FieldDelimiter
    {
      get { return Get<char>("FieldDelimiter", ','); }
      set { Set("FieldDelimiter", value); }
    }

    public bool KeepOpen
    {
      get { return Get<bool>("KeepOpen"); }
      set { Set("KeepOpen", value); }
    }

  }
}
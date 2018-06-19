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

namespace Toolset.Serialization
{
  public enum NodeType
  {
    Unknown = 0x00,

    Start = 0x01,
    End = 0x02,

    Value = 0x04,

    Document = 0x08,
    DocumentStart = Document | Start,
    DocumentEnd = Document | End,

    Object = 0x10,
    ObjectStart = Object | Start,
    ObjectEnd = Object | End,

    Collection = 0x20,
    CollectionStart = Collection | Start,
    CollectionEnd = Collection | End,

    Property = 0x40,
    PropertyStart = Property | Start,
    PropertyEnd = Property | End,
  }
}
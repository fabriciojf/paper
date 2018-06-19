using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Excel
{
  public struct Sheet
  {
    public int Id;
    public string Name;
    public string Label;
    public string ZipEntryName;
    public string TempFilename;
    public Action<SheetBuilder> Builder;
  }
}
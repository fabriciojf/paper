using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Paper.Media.Design.Extensions;

namespace Paper.Media.Papers
{
  public interface IPaperData : IPaper
  {
    string GetTitle();

    DataTable GetData();

    IEnumerable<HeaderInfo> GetDataHeaders(DataTable data);

    IEnumerable<ILink> GetDataLinks(DataTable data);
  }
}
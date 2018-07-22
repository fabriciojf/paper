using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Paper.Media.Design;

namespace Paper.Media.Design.Papers
{
  public interface IPaperData : IPaper
  {
    string GetTitle();

    DataTable GetData();

    IEnumerable<HeaderInfo> GetDataHeaders(DataTable data);

    IEnumerable<ILink> GetDataLinks(DataTable data);
  }
}
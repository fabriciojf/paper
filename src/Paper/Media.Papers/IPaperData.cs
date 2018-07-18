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

    DataRow GetData();

    IEnumerable<ILink> GetDataLinks(DataRow data);

    IEnumerable<HeaderInfo> GetDataHeaders(DataRow data);
  }
}
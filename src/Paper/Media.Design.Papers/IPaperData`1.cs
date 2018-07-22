using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Paper.Media.Design;

namespace Paper.Media.Design.Papers
{
  public interface IPaperData<TData> : IPaper
  {
    string GetTitle();

    TData GetData();

    IEnumerable<HeaderInfo> GetDataHeaders(TData data);

    IEnumerable<ILink> GetDataLinks(TData data);
  }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Paper.Media.Design.Extensions;

namespace Paper.Media.Papers
{
  public interface IPaperData<TData> : IPaper
  {
    string GetTitle();

    TData GetData();

    IEnumerable<ILink> GetDataLinks(TData data);

    IEnumerable<HeaderInfo> GetDataHeaders(TData data);
  }
}
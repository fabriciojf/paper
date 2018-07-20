using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Paper.Media.Design.Extensions;

namespace Media.Design.Extensions.Papers
{
  public interface IPaperData<TData> : IPaper
  {
    string GetTitle();

    TData GetData();

    IEnumerable<HeaderInfo> GetDataHeaders(TData data);

    IEnumerable<ILink> GetDataLinks(TData data);
  }
}
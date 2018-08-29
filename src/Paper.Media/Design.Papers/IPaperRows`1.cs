using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Paper.Media.Design;

namespace Paper.Media.Design.Papers
{
  public interface IPaperRows<TFilter, TRow> : IPaper
    where TFilter : IFilter
  {
    Page Page { get; }

    Sort Sort { get; }

    TFilter Filter { get; }

    string GetTitle();

    IEnumerable<TRow> GetRows();

    IEnumerable<HeaderInfo> GetRowHeaders(IEnumerable<TRow> rows);

    IEnumerable<ILink> GetRowLinks(TRow row);
  }
}
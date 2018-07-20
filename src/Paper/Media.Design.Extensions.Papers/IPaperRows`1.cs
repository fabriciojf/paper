using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Paper.Media.Design.Extensions;

namespace Media.Design.Extensions.Papers
{
  public interface IPaperRows<TRow> : IPaper
  {
    Page RowsPage { get; }

    Sort RowsSort { get; }

    IFilter RowFilter { get; }

    string GetTitle();

    IEnumerable<TRow> GetRows();

    IEnumerable<HeaderInfo> GetRowHeaders(IEnumerable<TRow> rows);

    IEnumerable<ILink> GetRowLinks(TRow row);
  }
}
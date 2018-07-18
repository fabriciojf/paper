using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Paper.Media.Design.Extensions;

namespace Paper.Media.Papers
{
  public interface IPaperRows<TRow> : IPaper
  {
    Pagination RowPagination { get; }

    Sort RowSort { get; }

    IFilter RowFilter { get; }

    string GetTitle();

    IEnumerable<TRow> GetRows();

    IEnumerable<Link> GetRowLinks(TRow row);

    IEnumerable<HeaderInfo> GetRowHeaders(TRow row);
  }
}
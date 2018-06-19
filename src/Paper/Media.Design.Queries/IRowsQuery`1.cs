using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Paper.Media.Design.Queries
{
  public interface IRowsQuery<TRow, TFilter> : IQuery
  {
    Pagination Pagination { get; }

    Sort Sort { get; }

    TFilter Filter { get; }

    string GetTitle();

    Links GetLinks();

    Cols GetRowsHeaders();

    IEnumerable<TRow> GetRows();

    Links GetRowLinks(TRow row);
  }
}

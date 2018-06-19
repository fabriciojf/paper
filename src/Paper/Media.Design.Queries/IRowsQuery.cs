using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Paper.Media.Design.Queries
{
  public interface IRowsQuery<TFilter> : IQuery
  {
    Pagination Pagination { get; }

    Sort Sort { get; }

    TFilter Filter { get; }

    string GetTitle();

    Links GetLinks();

    Cols GetRowsHeaders();

    DataTable GetRows();

    Links GetRowLinks(DataRow row);
  }
}

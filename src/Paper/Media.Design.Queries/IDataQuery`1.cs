using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Queries
{
  public interface IDataQuery<TData> : IQuery
  {
    string GetTitle();

    Links GetLinks();

    Cols GetDataHeaders();

    TData GetData();

    Links GetDataLinks(TData data);
  }
}
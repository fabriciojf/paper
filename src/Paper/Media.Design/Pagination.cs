using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Design
{
  public class Pagination
  {
    public Pagination()
    {
    }

    public Pagination(int limit, int offset)
    {
      this.Limit = limit;
      this.Offset = offset;
    }

    public int Limit { get; set; } = 50;

    public int Offset { get; set; }

    public Pagination SetPage(int page, int pageSize)
    {
      this.Limit = pageSize;
      this.Offset = (page - 1) * pageSize;
      return this;
    }

    public override string ToString()
    {
      return $"offset={Offset}&limit={Limit}";
    }
  }
}
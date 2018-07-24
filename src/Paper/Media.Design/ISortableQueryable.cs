using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Paper.Media.Design
{
  public interface ISortableQueryable<T> : IQueryable<T>
  {
    IQueryable<T> Source { get; }
  }
}

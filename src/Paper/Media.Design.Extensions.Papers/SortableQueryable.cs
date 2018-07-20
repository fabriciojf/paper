using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Media.Design.Extensions.Papers
{
  internal class SortableQueryable<T> : ISortableQueryable<T>
  {
    public SortableQueryable(IQueryable<T> source)
    {
      this.Source = source;
    }

    public IQueryable<T> Source { get; }

    public Type ElementType => Source.ElementType;

    public Expression Expression => Source.Expression;

    public IQueryProvider Provider => Source.Provider;

    public IEnumerator<T> GetEnumerator()
    {
      return this.Source.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.Source.GetEnumerator();
    }
  }
}

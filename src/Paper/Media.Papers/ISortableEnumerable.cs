using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Papers
{
  public interface ISortableEnumerable<T> : IEnumerable<T>
  {
    IEnumerable<T> Source { get; }
  }
}

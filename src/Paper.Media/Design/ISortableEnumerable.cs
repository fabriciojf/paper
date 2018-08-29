using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design
{
  public interface ISortableEnumerable<T> : IEnumerable<T>
  {
    IEnumerable<T> Source { get; }
  }
}

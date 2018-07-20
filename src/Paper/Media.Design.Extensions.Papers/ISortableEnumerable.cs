using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Media.Design.Extensions.Papers
{
  public interface ISortableEnumerable<T> : IEnumerable<T>
  {
    IEnumerable<T> Source { get; }
  }
}

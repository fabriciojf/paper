using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Papers
{
  public class Filter<T> : Filter
  {
    public Filter()
    {
      this.AddFieldsFrom<T>();
    }
  }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Design.Papers
{
  public class Sort<T> : Sort
  {
    public Sort()
    {
      this.AddFieldsFrom<T>();
    }
  }
}
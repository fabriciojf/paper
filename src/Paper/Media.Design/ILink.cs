using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design
{
  public interface ILink
  {
    string Title { get; }

    NameCollection Rel { get; }
  }
}

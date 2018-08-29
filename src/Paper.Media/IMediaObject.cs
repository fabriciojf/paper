using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media
{
  public interface IMediaObject
  {
    string Title { get; set; }

    NameCollection Class { get; set; }

    NameCollection Rel { get; set; }
  }
}

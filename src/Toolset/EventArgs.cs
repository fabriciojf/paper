using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset
{
  public class EventArgs<T> : EventArgs
  {
    public EventArgs(T data)
    {
      this.Data = data;
    }

    public T Data { get; }
  }
}

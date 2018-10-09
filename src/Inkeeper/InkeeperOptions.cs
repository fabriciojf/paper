using System;
using System.Collections.Generic;
using System.Text;

namespace Inkeeper
{
  public class InkeeperOptions
  {
    internal int Port { get; set; } = 90;

    public InkeeperOptions AddPort(int port)
    {
      this.Port = port;
      return this;
    }
  }
}

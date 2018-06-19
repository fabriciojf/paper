using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Toolset
{
  public class RetStatus : IRet
  {
    public int Status { get; set; }

    object IRet.Data => null;

    object IRet.Fault => null;
  }
}
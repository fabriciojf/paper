using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset
{
  public interface IRet
  {
    int Status { get; }
    object Data { get; }
    object Fault { get; }
  }
}

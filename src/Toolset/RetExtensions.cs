using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Toolset
{
  public static class RetExtensions
  {
    public static bool IsOk(this IRet ret)
      => ret.Status < 400;

    public static bool IsFault(this IRet ret)
      => ret.Status >= 400;

    public static bool IsNotFound(this IRet ret)
      => ret.Status == (int)HttpStatusCode.NotFound;

    public static bool IsNotFoundOrImplemented(this IRet ret)
      => ret.Status == (int)HttpStatusCode.NotFound || ret.Status == (int)HttpStatusCode.NotImplemented;

    public static Exception GetException(this IRet ret)
      => ret.Fault as Exception;

    public static string GetMessage(this IRet ret)
    {
      var ex = ret.Fault as Exception;
      if (ex != null)
      {
        var message = ex.Message;
        while (ex.InnerException != null)
        {
          ex = ex.InnerException;
          message += "\n" + ex.Message;
        }
        return message;
      }
      else
      {
        return ret.Fault?.ToString();
      }
    }
  }
}
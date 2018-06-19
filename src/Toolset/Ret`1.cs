using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Toolset
{
  public class Ret<T> : Ret, IRet
  {
    internal Ret()
    {
    }

    public new T Data
    {
      get => (base.Data is T) ? (T)base.Data : default(T);
      set => base.Data = value;
    }

    public static implicit operator T(Ret<T> ret)
    {
      return ret.Data;
    }

    public static implicit operator Ret<T>(T instance)
    {
      return Ret.Ok(instance);
    }

    public static implicit operator Exception(Ret<T> ret)
    {
      return ret.Fault as Exception;
    }

    public static implicit operator Ret<T>(Exception exception)
    {
      return Ret.Fail(exception);
    }

    public static implicit operator Ret<T>(RetStatus ok)
    {
      return new Ret<T>
      {
        Status = ok.Status
      };
    }

    public static implicit operator Ret<T>(RetFault fault)
    {
      return new Ret<T>
      {
        Status = fault.Status,
        Fault = fault.Fault
      };
    }
  }
}
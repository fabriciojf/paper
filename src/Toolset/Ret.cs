using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Toolset
{
  public class Ret : IRet
  {
    internal Ret()
    {
    }

    public int Status { get; set; }

    public object Data { get; set; }

    public object Fault { get; set; }

    public static implicit operator Exception(Ret ret)
    {
      return ret.Fault as Exception;
    }

    public static implicit operator Ret(Exception exception)
    {
      return Ret.Fail(exception);
    }

    public static implicit operator Ret(RetStatus ok)
    {
      return new Ret
      {
        Status = ok.Status
      };
    }

    public static implicit operator Ret(RetFault fault)
    {
      return new Ret
      {
        Status = fault.Status,
        Fault = fault.Fault
      };
    }

    public static implicit operator Ret(bool status)
    {
      return status ? Ret.Ok() : Ret.As(HttpStatusCode.BadRequest);
    }

    #region Métodos Ok()

    public static RetStatus Ok()
    {
      return new RetStatus
      {
        Status = (int)HttpStatusCode.OK
      };
    }

    public static Ret<T> Ok<T>(T data)
    {
      return new Ret<T>
      {
        Status = (int)HttpStatusCode.OK,
        Data = data
      };
    }

    #endregion

    #region Métodos Create()

    public static RetStatus As(int status)
    {
      return new RetStatus
      {
        Status = status
      };
    }

    public static Ret<T> As<T>(int status, T data)
    {
      return new Ret<T>
      {
        Status = status,
        Data = data
      };
    }

    public static RetStatus As(HttpStatusCode status)
    {
      return new RetStatus
      {
        Status = (int)status
      };
    }

    public static Ret<T> As<T>(HttpStatusCode status, T data)
    {
      return new Ret<T>
      {
        Status = (int)status,
        Data = data
      };
    }

    #endregion

    #region Métodos Fail()

    public static RetFault Throw(IRet ret)
    {
      return new RetFault
      {
        Status = ret.Status,
        Fault = ret.Fault
      };
    }

    public static RetFault Fail()
    {
      return new RetFault
      {
        Status = (int)HttpStatusCode.InternalServerError
      };
    }

    public static RetFault Fail(object fault)
    {
      return new RetFault
      {
        Status = (int)HttpStatusCode.InternalServerError,
        Fault = fault
      };
    }

    public static RetFault Fail(string message)
    {
      return new RetFault
      {
        Status = (int)HttpStatusCode.InternalServerError,
        Fault = message
      };
    }

    public static RetFault Fail(Exception exception)
    {
      var status =
        exception is NotImplementedException
          ? HttpStatusCode.NotImplemented
          : HttpStatusCode.InternalServerError;

      exception.Debug();
      return new RetFault
      {
        Status = (int)status,
        Fault = exception
      };
    }

    public static RetFault Fail(int status)
    {
      return new RetFault
      {
        Status = status
      };
    }

    public static RetFault Fail(int status, object fault)
    {
      return new RetFault
      {
        Status = status,
        Fault = fault
      };
    }

    public static RetFault Fail(int status, string message)
    {
      return new RetFault
      {
        Status = status,
        Fault = message
      };
    }

    public static RetFault Fail(int status, Exception exception)
    {
      exception.Debug();
      return new RetFault
      {
        Status = status,
        Fault = exception
      };
    }

    public static RetFault Fail(HttpStatusCode status)
    {
      return new RetFault
      {
        Status = (int)status
      };
    }

    public static RetFault Fail(HttpStatusCode status, object fault)
    {
      return new RetFault
      {
        Status = (int)status,
        Fault = fault
      };
    }

    public static RetFault Fail(HttpStatusCode status, string message)
    {
      return new RetFault
      {
        Status = (int)status,
        Fault = message
      };
    }

    public static RetFault Fail(HttpStatusCode status, Exception exception)
    {
      exception.Debug();
      return new RetFault
      {
        Status = (int)status,
        Fault = exception
      };
    }

    #endregion

  }
}
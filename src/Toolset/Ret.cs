using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Toolset
{
  /// <summary>
  /// Representação de execução de uma função sem retorno (void).
  /// 
  /// Permite uma sintaxe padronizada para retorno de funções que não
  /// propagam exceções.
  /// 
  /// É esperado que uma função que retorna Ret não lance exceções.
  /// O método chamador espera que falhas sejam retornadas pelo próprio
  /// objeto Ret.
  /// 
  /// Esquelo geral de uma função implementada com Ret:
  /// 
  ///     public Ret Funcao()
  ///     {
  ///       try
  ///       {
  ///         if (... bem sucedido ...)
  ///         {
  ///           return true;      //- conversao implitica de booliano para Ret
  ///         }
  ///         else
  ///         {
  ///           return false;     //- conversao implitica de booliano para Ret
  ///         }
  ///       }
  ///       catch (Exception ex)
  ///       {
  ///         return ex;          //- conversao implitica de Exception para Ret
  ///       }
  ///     }
  ///     
  /// A checagem do retorno pode ser feita pelas propriedades de Ret ou
  /// pela conversão implícita de Ret para o tipo booliano.
  /// 
  /// Exemplo 1: Mais controle sobre o fluxo
  /// 
  ///     Ret ret = Funcao()
  ///     if (ret.Ok)
  ///     {
  ///       //...
  ///     }
  ///     else
  ///     {
  ///       Debug.WriteLine(string.Join(Environment.NewLine, ret.Falhas));
  ///       Debug.WriteLine(ret.StackTrace);
  ///     }
  ///     
  /// Exemplo 2: Recomendado somente se o detalhe da falha puder ser ignorado
  /// 
  ///     Ret ok = Funcao()
  ///     if (ok)
  ///     {
  ///       // ...
  ///     }
  ///     
  ///     ou
  ///     
  ///     Ret ok = Funcao()
  ///     if (!ok)
  ///     {
  ///       // ...
  ///     }
  ///     
  /// </summary>
  public struct Ret
  {
    private string[] _faultReasons;

    /// <summary>
    /// Verdadeiro quando a função é executada sem erros; Falso caso contrário.
    /// </summary>
    public bool Ok => (int)Status < 400;

    /// <summary>
    /// Valor retornado em caso de sucesso.
    /// </summary>
    public object Value
    {
      get;
      internal set;
    }

    /// <summary>
    /// Código de status do retorno.
    /// O código segue o padrão de códigos HTTP.
    /// </summary>
    public HttpStatusCode Status
    {
      get;
      internal set;
    }

    /// <summary>
    /// Coleção das falhas emitidas pela função.
    /// Quando Ok é falso é garantido que Falhas é diferente de nulo.
    /// </summary>
    public string[] FaultReasons
    {
      get => Ok ? _faultReasons : (_faultReasons ?? (_faultReasons = new string[0]));
      internal set => _faultReasons = value;
    }

    /// <summary>
    /// Exceção ocorrida.
    /// </summary>
    public Exception FaultException
    {
      get;
      internal set;
    }

    /// <summary>
    /// Obtém uma mensagem de status representando o sucesso ou a falha ocorrida.
    /// </summary>
    /// <returns>A mensagem de status representando o sucesso ou a falha ocorrida.</returns>
    public string GetMessage()
    {
      return FaultReasons.Length > 0
        ? string.Join(Environment.NewLine, FaultReasons)
        : Status.ToString().ChangeCase(TextCase.ProperCase);
    }

    /// <summary>
    /// Conversão implitica de booliano para Ret.
    /// Permite uma sintaxe mais simples com o retorno direto de `true' ou `false'.
    /// 
    /// Exemplo:
    ///     public Ret Funcao()
    ///     {
    ///       if (feito) 
    ///       {
    ///         return true;
    ///       }
    ///       else
    ///       {
    ///         return false;
    ///       }
    ///     }
    /// </summary>
    /// <param name="ok">
    /// Verdadeiro em caso de retorno de sucesso; Falso caso contrário.
    /// </param>
    public static implicit operator Ret(bool ok)
    {
      return new Ret { Status = ok ? HttpStatusCode.OK : HttpStatusCode.BadRequest };
    }

    /// <summary>
    /// Conversão implitica de HttpStatusCode para Ret.
    /// Permite uma sintaxe mais simples com o retorno direto de HttpStatusCode.
    /// 
    /// Exemplo:
    ///     public Ret Funcao()
    ///     {
    ///       if (feito) 
    ///       {
    ///         return HttpStatusCode.OK;
    ///       }
    ///       else
    ///       {
    ///         return HttpStatusCode.NotFound;
    ///       }
    ///     }
    /// </summary>
    /// <param name="status">
    /// O status HTTP para retorno.
    /// </param>
    public static implicit operator Ret(HttpStatusCode status)
    {
      return new Ret { Status = status };
    }

    /// <summary>
    /// Conversão implitica de Ret para booliano.
    /// Permite uma sintaxe mais simples na comparação de resultado da função.
    /// 
    /// Exemplo:
    ///     public Ret Funcao()
    ///     {
    ///       //...
    ///     }
    ///     
    ///     var ok = Funcao();
    ///     if (ok)
    ///     {
    ///       //...
    ///     }
    /// </summary>
    /// <param name="ok">
    /// Verdadeiro em caso de retorno de sucesso; Falso caso contrário.
    /// </param>
    public static implicit operator bool(Ret ret)
    {
      return ret.Ok;
    }

    /// <summary>
    /// Conversão implitica de Ret para HttpStatusCode.
    /// Permite uma sintaxe mais simples na comparação de resultado da função.
    /// 
    /// Exemplo:
    ///     public Ret Funcao()
    ///     {
    ///       //...
    ///     }
    ///     
    ///     var status = Funcao();
    ///     if (status == HttpStatusCode.Forbidden)
    ///     {
    ///       //...
    ///     }
    /// </summary>
    /// <param name="ok">
    /// Verdadeiro em caso de retorno de sucesso; Falso caso contrário.
    /// </param>
    public static implicit operator HttpStatusCode(Ret ret)
    {
      return ret.Status;
    }

    /// <summary>
    /// Conversão implitica de exceção para Ret.
    /// Permite uma sintaxe mais simples com o retorno direto da exceção.
    /// 
    /// Exemplo:
    ///     public Ret Funcao()
    ///     {
    ///       try
    ///       {
    ///         return true;
    ///       }
    ///       catch (Exception ex)
    ///       {
    ///         return ex;
    ///       }
    ///     }
    /// </summary>
    /// <param name="exception">A exceção capturada.</param>
    public static implicit operator Ret(Exception exception)
    {
      return new Ret
      {
        Status = HttpStatusCode.InternalServerError,
        FaultReasons = exception.GetCauseMessages(),
        FaultException = exception
      };
    }

    /// <summary>
    /// Emite uma falha com as causas indicadas.
    /// </summary>
    /// <param name="faultReasons">As causas da falha.</param>
    /// <returns>O retorno de função representando a falha.</returns>
    public static Ret Fail(params string[] faultReasons)
    {
      return new Ret
      {
        Status = HttpStatusCode.InternalServerError,
        FaultReasons = faultReasons
      };
    }

    /// <summary>
    /// Emite uma falha com as causas indicadas.
    /// </summary>
    /// <param name="causas">As causas da falha.</param>
    /// <param name="stackTrace">A pilha de erro em caso de captura de exceção.</param>
    /// <param name="exception">Causa da falha.</param>
    /// <returns>O retorno de função representando a falha.</returns>
    public static Ret Fail(IEnumerable<string> faultReasons, Exception exception = null)
    {
      return new Ret
      {
        Status = HttpStatusCode.InternalServerError,
        FaultReasons = faultReasons.ToArray(),
        FaultException = exception
      };
    }

    /// <summary>
    /// Emite um status personalizado.
    /// </summary>
    /// <param name="status">Status de retorno.</param>
    /// <param name="faultReasons">As causas da falha.</param>
    /// <returns>O retorno de função representando o status.</returns>
    public static Ret As(HttpStatusCode status, params string[] faultReasons)
    {
      return new Ret
      {
        Status = status,
        FaultReasons = faultReasons
      };
    }

    /// <summary>
    /// Emite um status personalizado.
    /// </summary>
    /// <param name="status">Status de retorno.</param>
    /// <param name="faultReasons">As causas da falha.</param>
    /// <param name="exception">Causa da falha.</param>
    /// <returns>O retorno de função representando o status.</returns>
    public static Ret As(HttpStatusCode status, IEnumerable<string> faultReasons, Exception exception = null)
    {
      return new Ret
      {
        Status = HttpStatusCode.InternalServerError,
        FaultReasons = faultReasons.ToArray(),
        FaultException = exception
      };
    }

    /// <summary>
    /// Emite um status personalizado.
    /// </summary>
    /// <param name="value">O valor retornado.</param>
    /// <param name="status">Status de retorno.</param>
    /// <returns>O retorno de função representando o status.</returns>
    public static Ret<T> Succeed<T>(T value, HttpStatusCode status)
    {
      return new Ret<T>
      {
        Value = value,
        Status = status
      };
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Toolset
{
  /// <summary>
  /// Representação de um retorno de função.
  /// 
  /// Permite uma sintaxe padronizada para retorno de funções que não
  /// propagam exceções.
  /// 
  /// O valor produzido pela função ou a exceção capturada podem ser
  /// lançadas diretamente com o uso das conversões implícitas ativadas
  /// para Ret.
  /// 
  /// É esperado que uma função que retorna Ret não lance exceções.
  /// O método chamador espera que falhas sejam retornadas pelo próprio
  /// objeto Ret.
  /// 
  /// Esquelo geral de uma função implementada com Ret:
  /// 
  ///     public Ret<T> Funcao()
  ///     {
  ///       try
  ///       {
  ///         T resultado = ....
  ///        
  ///         //...
  ///        
  ///         return resultado;   //- conversao implitica de T para Ret<T>
  ///       }
  ///       catch (Exception ex)
  ///       {
  ///         return ex;          //- conversao implitica de Exception para Ret<T>
  ///       }
  ///     }
  ///     
  /// A checagem do retorno pode ser feita pelas propriedades de Ret ou
  /// pela conversão implícita de Ret para o tipo retornado.
  /// 
  /// Exemplo 1: Mais controle sobre o fluxo
  /// 
  ///     Ret<T> ret = Funcao()
  ///     if (ret.Ok)
  ///     {
  ///       T valor = ret.Valor;
  ///     }
  ///     else
  ///     {
  ///       Debug.WriteLine(string.Join(Environment.NewLine, ret.Falhas));
  ///       Debug.WriteLine(ret.StackTrace);
  ///     }
  ///     
  /// Exemplo 2: Se T for um objeto e uma falha puder ser ignorada
  /// 
  ///     T valor = Funcao()
  ///     if (valor != null)
  ///     {
  ///       // ...
  ///     }
  ///     
  /// Exemplo 3: Se T for um struct e uma falha puder ser ignorada
  /// 
  ///     T valor = Funcao()
  ///     if (valor != default(T))
  ///     {
  ///       // ...
  ///     }
  ///     
  /// </summary>
  /// <typeparam name="T">O tipo do dado retornado.</typeparam>
  public struct Ret<T>
  {
    private string[] _faultReasons;

    /// <summary>
    /// Verdadeiro quando a função é executada sem erros; Falso caso contrário.
    /// </summary>
    public bool Ok => (int)Status < 400;

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
    /// O valor detornado pela função em caso de sucesso.
    /// O valor existe apenas quando Ok é verdadeiro.
    /// </summary>
    public T Value
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
    /// Pilha de erro em caso de exceção capturada.
    /// </summary>
    public Exception FaultException
    {
      get;
      internal set;
    }

    /// <summary>
    /// Conversão implitica de um dado para Ret<T>.
    /// Permite uma sintaxe mais simples com o retorno direto
    /// do dado produzido pela função.
    /// 
    /// Exemplo:
    ///     public Ret<T> Funcao()
    ///     {
    ///       try
    ///       {
    ///         T resultado = ...
    ///         
    ///         // ...
    /// 
    ///         return resultado; //- conversao implicita de T para Ret<T>
    ///       }
    ///       catch (Exception ex)
    ///       {
    ///         return ex;
    ///       }
    ///     }
    /// </summary>
    /// <param name="value">
    /// O resultado produzido pela função.
    /// </param>
    public static implicit operator Ret<T>(T value)
    {
      return new Ret<T>
      {
        Status = HttpStatusCode.OK,
        Value = value
      };
    }

    /// <summary>
    /// Conversão implitica de Ret<T> para o dado encapsulado.
    /// Permite uma sintaxe mais simples na obtenção do retorno da função.
    /// 
    /// Exemplo:
    ///     public Ret<T> Funcao()
    ///     {
    ///       T resultado = ....
    /// 
    ///       //...
    /// 
    ///       return resultado;
    ///     }
    ///     
    ///     T resultado = Funcao(); //- Conversao implitica de Ret<T> para T
    /// </summary>
    /// <param name="valor">
    /// O resultado produzido pela função.
    /// </param>
    public static implicit operator T(Ret<T> ret)
    {
      return ret.Value;
    }

    /// <summary>
    /// Conversão implitica de Ret<T> para Ret.
    /// Permite uma sintaxe mais genérica na construção do código.
    /// 
    /// Exemplo:
    ///     public Ret<T> FuncaoA()
    ///     {
    ///       //...
    ///     }
    ///     public Ret<X> FuncaoB()
    ///     {
    ///       //...
    ///     }
    ///     
    ///     Ret ok;
    ///     
    ///     ok = FuncaoA();
    ///     if (!ok)
    ///     {
    ///       ok = FuncaoB();
    ///       if (!ok)
    ///       {
    ///         Debug.WriteLine("...");
    ///       }
    ///     }
    /// </summary>
    /// <param name="ret">
    /// O tipo Ret<T> retornado por uma função.
    /// </param>
    public static implicit operator Ret(Ret<T> ret)
    {
      return new Ret
      {
        Status = ret.Status,
        Value = ret.Value,
        FaultReasons = ret.FaultReasons,
        FaultException = ret.FaultException
      };
    }

    /// <summary>
    /// Conversão implícita de Ret<T> para Ret.
    /// </summary>
    public static implicit operator Ret<T>(Ret ret)
    {
      return new Ret
      {
        Status = ret.Status,
        Value = ret.Value,
        FaultReasons = ret.FaultReasons,
        FaultException = ret.FaultException
      };
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
    public static implicit operator Ret<T>(bool ok)
    {
      return new Ret<T> { Status = ok ? HttpStatusCode.OK : HttpStatusCode.BadRequest };
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
    public static implicit operator bool(Ret<T> ret)
    {
      return ret.Ok;
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
    public static implicit operator Ret<T>(HttpStatusCode status)
    {
      return new Ret<T> { Status = status };
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
    public static implicit operator HttpStatusCode(Ret<T> ret)
    {
      return ret.Status;
    }

    /// <summary>
    /// Conversão implitica de exceção para Ret<T>.
    /// Permite uma sintaxe mais simples com o retorno direto da exceção.
    /// 
    /// Exemplo:
    ///     public Ret<T> Funcao()
    ///     {
    ///       try
    ///       {
    ///         T resultado = ....
    ///        
    ///         //...
    ///        
    ///         return resultado;
    ///       }
    ///       catch (Exception ex)
    ///       {
    ///         return ex;
    ///       }
    ///     }
    /// </summary>
    /// <param name="exception">A exceção capturada.</param>
    public static implicit operator Ret<T>(Exception exception)
    {
      return new Ret<T>
      {
        Status = HttpStatusCode.InternalServerError,
        FaultReasons = exception.GetCauseMessages(),
        FaultException = exception
      };
    }
  }
}

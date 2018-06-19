using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Toolset
{
  public static class ExceptionExtensions
  {
    public static string GetStackTrace(this Exception excecao)
    {
      using (var saida = new StringWriter())
      {
        Trace(excecao, saida);
        return saida.ToString();
      }
    }

    public static void Debug(this Exception excecao)
    {
      try
      {
        var pilha = GetStackTrace(excecao);
        System.Diagnostics.Debug.WriteLine(pilha);
      }
      catch (Exception ex)
      {
        Dump(excecao, ex);
      }
    }

    public static void Trace(this Exception excecao)
    {
      try
      {
        var pilha = GetStackTrace(excecao);
        System.Diagnostics.Trace.TraceError(pilha);
      }
      catch (Exception ex)
      {
        Dump(excecao, ex);
      }
    }

    public static void TraceWarning(this Exception excecao)
    {
      var pilha = GetStackTrace(excecao);
      try
      {
        System.Diagnostics.Trace.TraceWarning(pilha);
      }
      catch (Exception ex)
      {
        Dump(excecao, ex);
      }
    }

    public static void Trace(this Exception excecao, string mensagem)
    {
      var pilha = GetStackTrace(excecao);
      try
      {
        System.Diagnostics.Trace.TraceError("{0}\nCausa:\n{1}", mensagem, pilha);
      }
      catch (Exception ex)
      {
        Dump(excecao, ex);
      }
    }

    public static void Trace(this Exception excecao, string formato, params object[] args)
    {
      Trace(excecao, string.Format(formato, args));
    }

    public static void TraceWarning(this Exception excecao, string mensagem)
    {
      try
      {
        var pilha = GetStackTrace(excecao);
        System.Diagnostics.Trace.TraceWarning("{0}\nCausa:\n{1}", mensagem, pilha);
      }
      catch (Exception ex)
      {
        Dump(excecao, ex);
      }
    }

    public static void TraceWarning(this Exception excecao, string formato, params object[] args)
    {
      TraceWarning(excecao, string.Format(formato, args));
    }

    public static void Trace(this Exception excecao, Stream saida)
    {
      using (var gravador = new StreamWriter(saida))
      {
        Trace(excecao, gravador);
      }
    }

    public static void Trace(this Exception excecao, TextWriter saida)
    {
      saida.Write("fault ");
      Exception ex = excecao;
      do
      {
        saida.WriteLine(ex.Message);
        saida.Write(" type ");
        saida.WriteLine(ex.GetType().FullName);

        var trace = ex.StackTrace;
        if (trace != null)
        {
          saida.Write(trace);
          if (!trace.EndsWith(Environment.NewLine))
          {
            saida.WriteLine();
          }
        }

        ex = ex.InnerException;
        if (ex != null)
        {
          saida.Write("cause ");
        }
      } while (ex != null);
    }

    public static Exception[] GetCauses(this Exception excecao)
    {
      var causes = EnumerateExceptionCauses(excecao).ToArray();
      return causes;
    }

    public static string[] GetCauseMessages(this Exception excecao)
    {
      var causes =
        EnumerateExceptionCauses(excecao)
          .Select(x => x.Message ?? "Falha não identificada.")
          .Distinct()
          .ToArray();
      return causes;
    }

    private static IEnumerable<Exception> EnumerateExceptionCauses(Exception exception)
    {
      while (exception != null)
      {
        yield return exception;
        exception = exception.InnerException;
      }
    }

    /// <summary>
    /// Este método tenta registrar as exceções da forma possível.
    /// Deve ser usado em caso de falha do método geral de gravação de
    /// exceções.
    /// </summary>
    /// <param name="excecoes">As exceções a serem gravadas.</param>
    private static void Dump(params Exception[] excecoes)
    {
      foreach (var excecao in excecoes)
      {
        try
        {
          var pilha = GetStackTrace(excecao);

          Console.Write("[FALHA]");
          Console.WriteLine(excecao.Message);
          Console.WriteLine(pilha);

          System.Diagnostics.Debug.Write("[FALHA]");
          System.Diagnostics.Debug.WriteLine(excecao.Message);
          System.Diagnostics.Debug.WriteLine(pilha);
        }
        catch
        {
          // nada a fazer
        }
      }
    }

  }
}

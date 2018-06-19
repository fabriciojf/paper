using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Exceção para parâmetro inválido.
  /// É recomendado capturar esta exceção e imprimir a mensagem com 
  /// Prompt.PrintInvalidUsage().
  /// </summary>
  public class InvalidUsageException : PromptException
  {
    public InvalidUsageException(string msg) : base(msg) { }
    public InvalidUsageException(string msg, Exception ex) : base(msg, ex) { }
  }
}

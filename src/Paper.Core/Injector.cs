using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Paper.Media.Rendering;

namespace Paper.Core
{
  /// <summary>
  /// Implementação do injetor de dependências.
  /// </summary>
  class Injector : IInjector
  {
    private readonly IServiceProvider serviceProvider;

    public Injector(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Instancia o tipo indicado resolvendo os parâmetros indicados no construtor.
    /// </summary>
    /// <param name="intanceType">Os argumentos repassados para o tipo.</param>
    /// <param name="args">Argumentos adicionais do construtor não providos pelo injetor.</param>
    /// <returns>O tipo instanciado.</returns>
    public object CreateInstance(Type intanceType, params object[] args)
    {
      return ActivatorUtilities.CreateInstance(serviceProvider, intanceType, args);
    }
  }
}

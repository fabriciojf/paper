using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Toolset.Collections;

namespace Paper
{
  public static class ServiceProviderExtensions
  {
    public static T CreateInstance<T>(this IServiceProvider provider, params object[] args)
    {
      return ActivatorUtilities.CreateInstance<T>(provider, args);
    }

    public static object CreateInstance(this IServiceProvider provider, Type intanceType, params object[] args)
    {
      return ActivatorUtilities.CreateInstance(provider, intanceType, args);
    }
  }
}
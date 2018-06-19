using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset
{
  /// <summary>
  /// Expõe uma objeto para descoberta dinâmica de referências.
  /// 
  /// Classes expostas com este atributo podem ser descobertas dinamicamente
  /// pelo utilitário <see cref="ExposedTypes"/>
  /// </summary>
  /// <seealso cref="ExposedTypes" />
  /// <seealso cref="System.Attribute" />
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
  public class ExposeAttribute : Attribute
  {
    public ExposeAttribute()
    {
    }

    public ExposeAttribute(string contractName)
    {
      this.ContractName = contractName;
    }

    public string ContractName { get; }
  }
}

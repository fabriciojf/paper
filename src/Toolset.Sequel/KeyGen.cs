using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Sequel
{
  /// <summary>
  /// Utilitário para geração de nomes de parâmetros únicos.
  /// O utilitário modifica o nome do parâmetro original com uma variação única.
  /// </summary>
  internal class KeyGen
  {
    /// <summary>
    /// Gerador de escopos aninhados.
    /// </summary>
    private class Deeper
    {
      /// <summary>
      /// O nome do escopo corrente.
      /// </summary>
      private char current = 'a';

      /// <summary>
      /// Aprofunda o escopo produzindo um novo caracter único de identificação de escopo.
      /// </summary>
      public char Deepen() => current++;
    }

    private readonly Deeper deeper;
    private char scope;
    private int index;

    public KeyGen()
      : this(new Deeper())
    {
    }

    private KeyGen(Deeper deeper)
    {
      this.deeper = deeper;
      this.scope = deeper.Deepen();
    }

    /// <summary>
    /// Deriva um novo KeyGen a partir do KeyGen corrente.
    /// O novo KeyGen pode ser usado para gerar nomes únicos em outro escopo
    /// sem produzir nomes conflitantes.
    /// </summary>
    /// <returns>O KeyGen derivado.</returns>
    public KeyGen Derive()
    {
      return new KeyGen(deeper);
    }

    /// <summary>
    /// Deriva um novo nome único de parâmetro a partir do nome de parâmetro indicado.
    /// </summary>
    /// <param name="name">O nome do parâmetro para derivação.</param>
    public string DeriveName(string name)
    {
      name = name.Replace("@", "");

      if (name.Contains("___"))
      {
        name = name.Substring(0, name.IndexOf("___"));
      }

      name = $"{name}___{scope}{++index}";
      return name;
    }
  }
}

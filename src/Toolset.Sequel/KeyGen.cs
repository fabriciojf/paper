using System;
using System.Collections.Generic;
using System.Linq;
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
      private readonly List<char> sequence = new List<char> { 'a' };

      public string Current => new string(sequence.ToArray());

      /// <summary>
      /// Aprofunda o escopo produzindo um novo caracter único de identificação de escopo.
      /// </summary>
      public Deeper Deepen()
      {
        var iterator = Enumerable.Range(0, sequence.Count).GetEnumerator();
        for (int currIndex = sequence.Count - 1; currIndex >= 0; currIndex--)
        {
          var prevIndex = currIndex - 1;

          var curr = sequence[currIndex];
          var prev = (prevIndex >= 0) ? sequence[prevIndex] : (char)0;

          if (curr < 'z')
          {
            sequence[currIndex]++;
            break;
          }

          if (prev > 0 && prev < 'z')
          {
            sequence[prevIndex]++;
            sequence[currIndex] = 'a';
            break;
          }

          if ((currIndex + 1) == sequence.Count)
          {
            sequence.Clear();
            sequence.AddRange(Enumerable.Range(0, currIndex + 2).Select(x => 'a'));
            break;
          }
        }
        return this;
      }
    }

    private readonly Deeper deeper;
    private string scope;
    private int index;

    public KeyGen()
      : this(new Deeper())
    {
    }

    private KeyGen(Deeper deeper)
    {
      this.deeper = deeper;
      this.scope = deeper.Current;
    }

    /// <summary>
    /// Deriva um novo KeyGen a partir do KeyGen corrente.
    /// O novo KeyGen pode ser usado para gerar nomes únicos em outro escopo
    /// sem produzir nomes conflitantes.
    /// </summary>
    /// <returns>O KeyGen derivado.</returns>
    public KeyGen Derive()
    {
      return new KeyGen(deeper.Deepen());
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

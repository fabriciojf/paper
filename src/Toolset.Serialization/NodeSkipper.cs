using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Serialization
{
  /// <summary>
  /// Utilitário para descartar completamente um nodo do fluxo de entrada.
  /// Em geral, quando Settings.IsLenient é marcado em um Writer o
  /// nodo lido sem correspondência com o modelo de dados pode ser
  /// ignorado.
  /// 
  /// Por exemplo, quando leniente é marcado, um GraphWriter ignora complemente uma propriedade
  /// lida do Reader quando não existe uma propriedade correspondente no objeto alvo.
  /// 
  /// Como funciona?
  /// 
  /// O NodeSkipper deve receber os nodos do fluxo de entrada.
  /// Se o skipper considerar que o nodo deve ser descartado verdadeiro será retornado e o
  /// algoritmo cliente pode avançar para o próximo nodo.
  /// Se o skipper retornar falso, então, o algoritmo cliente pode procesar o nodo normalmente.
  /// 
  /// O skipper, quando ativado, ignora a próxima hierarquia de nodos.
  /// Por exemplo, se depois de ativado o skipper receber uma propriedade, esta propriedade será
  /// ignorada e todos os nodos na sua hierarquia.
  /// Depois da propriedade ser terminada o skipper se desativa e não mais ignora nodos até ser
  /// ativado novamente.
  /// 
  /// Exemplo de uso:
  /// 
  ///     // em algum lugar do codigo
  ///     nodeSkipper.Activate();
  /// 
  ///     // em um metodo de leitura
  ///     var skip = nodeSkipper.ShouldSkip(node);
  ///     if (!skip)
  ///     {
  ///       processa o nodo...
  ///     }
  /// </summary>
  internal class NodeSkipper
  {
    private int depth;

    public bool Active { get; private set; }

    public void Activate()
    {
      Active = true;
    }

    public bool ShouldSkip(Node node)
    {
      if (!Active)
        return false;

      if (node.Type.HasFlag(NodeType.Start))
        depth++;
      if (node.Type.HasFlag(NodeType.End))
        depth--;

      if (depth == 0)
        Active = false;

      return true;
    }
  }
}

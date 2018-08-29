using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paper.Media.Serialization
{
  public interface ISerializer
  {
    /// <summary>
    /// Serializa a entidade para string.
    /// </summary>
    /// <param name="entity">Entidade a ser serializada.</param>
    string Serialize(Entity entity);

    /// <summary>
    /// Serializa a entidade para a saída indicada.
    /// </summary>
    /// <param name="entity">Entidade a ser serializada.</param>
    /// <param name="output">Stream de saída.</param>
    void Serialize(Entity entity, Stream output);

    /// <summary>
    /// Serializa a entidade para a saída indicada.
    /// </summary>
    /// <param name="entity">Entidade a ser serializada.</param>
    /// <param name="output">Stream de saída.</param>
    void Serialize(Entity entity, TextWriter output);

    /// <summary>
    /// Deserializa o texto para uma instância de Entity.
    /// </summary>
    /// <param name="text">O texto a ser deserializado.</param>
    /// <returns>A entidade obtida da serialização.</returns>
    Entity Deserialize(string text);

    /// <summary>
    /// Deserializa o texto para uma instância de Entity.
    /// </summary>
    /// <param name="input">A entrada para leitura do texto a ser deserializado.</param>
    /// <returns>A entidade obtida da serialização.</returns>
    Entity Deserialize(Stream input);

    /// <summary>
    /// Deserializa o texto para uma instância de Entity.
    /// </summary>
    /// <param name="input">A entrada para leitura do texto a ser deserializado.</param>
    /// <returns>A entidade obtida da serialização.</returns>
    Entity Deserialize(TextReader input);
  }
}

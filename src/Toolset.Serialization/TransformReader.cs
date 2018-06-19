using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public class TransformReader : Reader
  {
    private readonly Reader reader;
    private readonly ITransform transform;
    private IEnumerator<Node> nodes;

    public TransformReader(Reader reader, ITransform transform)
      : base(reader.Settings)
    {
      this.reader = reader;
      this.nodes = EnumerateNodes().GetEnumerator();
      this.transform = transform;
      this.transform.Settings = this.Settings;
    }

    public TransformReader(Reader reader, ITransform transform, SerializationSettings settings)
      : base(settings ?? new SerializationSettings())
    {
      this.reader = reader;
      this.nodes = EnumerateNodes().GetEnumerator();
      this.transform = transform;
      this.transform.Settings = this.Settings;
    }

    public override Node Current
    {
      get { return nodes.Current; }
    }

    protected override bool DoRead()
    {
      while (nodes.MoveNext())
      {
        if (nodes.Current.RawType == NodeType.Document && Settings.IsFragment)
          continue;

        return true;
      }
      return false;
    }

    private IEnumerable<Node> EnumerateNodes()
    {
      while (reader.Read())
      {
        var nodes = transform.TransformNode(reader.Current);
        foreach (var node in nodes)
        {
          yield return node;
        }
      }

      var lastNodes = transform.Complete();
      foreach (var node in lastNodes)
      {
        yield return node;
      }
    }

    public override void Close()
    {
      reader.Close();
    }
  }
}

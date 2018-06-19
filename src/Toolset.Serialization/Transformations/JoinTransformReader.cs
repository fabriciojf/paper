using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset.Serialization.Excel;

namespace Toolset.Serialization.Transformations
{
  public sealed class JoinTransformReader : Reader
  {
    private readonly SerializationSettings settings;
    private readonly Reader[] readers;
    private readonly IEnumerator<Node> enumerator;

    private int readerCount;
    private Node currentNode;

    #region Construtores extras...

    public JoinTransformReader(IEnumerable<Reader> readers)
      : this(null, readers)
    {
      // nada a fazer aqui. use o outro construtor...
    }

    public JoinTransformReader(Reader reader, params Reader[] others)
      : this(null, reader, others)
    {
      // nada a fazer aqui. use o outro construtor...
    }
    #endregion

    public JoinTransformReader(SerializationSettings settings, IEnumerable<Reader> readers)
      : base(settings ?? new SerializationSettings())
    {
      this.settings = settings;
      this.readers = readers.ToArray();
      this.enumerator = EnumerateNodes().GetEnumerator();
    }

    public JoinTransformReader(SerializationSettings settings, Reader reader, params Reader[] others)
      : base(settings ?? new SerializationSettings())
    {
      this.readers = (new[] { reader }.Union(others)).ToArray();
      this.enumerator = EnumerateNodes().GetEnumerator();
    }

    public override Node Current
    {
      get { return currentNode; }
    }

    protected override bool DoRead()
    {
      var ready = enumerator.MoveNext();
      currentNode = ready ? enumerator.Current : null;
      return ready;
    }

    private IEnumerable<Node> EnumerateNodes()
    {
      if (!settings.IsFragment)
      {
        yield return new Node { Type = NodeType.DocumentStart };
        yield return new Node { Type = NodeType.ObjectStart };
      }

      var enumerator = this.readers.Cast<Reader>().GetEnumerator();
      while (enumerator.MoveNext())
      {
        readerCount++;

        if (!settings.IsFragment)
        {
          var name = "Document" + readerCount;
          var conventionName = ValueConventions.CreateName(name, settings, TextCase.KeepOriginal);
          yield return new Node { Type = NodeType.PropertyStart, Value = conventionName };
        }

        var reader = enumerator.Current;
        while (reader.Read())
        {
          var isDocument = (reader.Current.Type & NodeType.Document) != 0;
          if (!isDocument)
          {
            yield return reader.Current;
          }
        }

        if (!settings.IsFragment)
        {
          yield return new Node { Type = NodeType.PropertyEnd };
        }
      }

      if (!settings.IsFragment)
      {
        yield return new Node { Type = NodeType.ObjectEnd };
        yield return new Node { Type = NodeType.DocumentEnd };
      }
    }

    public override void Close()
    {
      foreach (var reader in readers)
      {
        reader.Close();
      }
    }
  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolset.Serialization.Json
{
  public sealed class JsonReader : Reader
  {
    private readonly TextReader reader;
    
    private readonly char[] tokens = new[] { '{', '}', '[', ']', ':', ',' };

    private IEnumerator<Node> nodeEnumerator;
    private Node currentNode;

    #region Construtores extras ...

    public JsonReader(TextReader textReader)
      : this(textReader, (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }
    
    public JsonReader(Stream textStream)
      : this(new StreamReader(textStream), (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }
    
    public JsonReader(Stream textStream, SerializationSettings settings)
      : this(new StreamReader(textStream), settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }
    
    public JsonReader(string filename)
      : this(File.OpenRead(filename), (SerializationSettings)null)
    {
      // nada a fazer aqui. use o outro construtor.
    }
    
    public JsonReader(string filename, SerializationSettings settings)
      : this(File.OpenRead(filename), settings)
    {
      // nada a fazer aqui. use o outro construtor.
    }
    
    #endregion

    public JsonReader(TextReader textReader, SerializationSettings settings)
      : base(settings)
    {
      this.reader = textReader;
      this.nodeEnumerator = EnumerateNodes().GetEnumerator();
      base.IsValid = true;
    }

    public new JsonSerializationSettings Settings
    {
      get { return base.Settings.As<JsonSerializationSettings>(); }
    }

    public override Node Current
    {
      get { return currentNode; }
    }

    public override void Close()
    {
      if (!Settings.KeepOpen)
      {
        reader.Close();
      }
    }

    protected override bool DoRead()
    {
      var ready = nodeEnumerator.MoveNext();
      currentNode = ready ? nodeEnumerator.Current : null;
      return ready;
    }

    private IEnumerable<Node> EnumerateNodes()
    {
      var tokenEnumerator = EnumerateTokens().GetEnumerator();
      var stack = new Stack<NodeType>();
      string suggestedName = null;

      var ready = tokenEnumerator.MoveNext();
      while (ready)
      {
        var token = tokenEnumerator.Current;
        
        switch (token)
        {
          case ",":
            {
              var type = stack.FirstOrDefault();
              if (type == NodeType.Property)
              {
                stack.Pop();
                yield return new Node { Type = NodeType.PropertyEnd };
              }
              ready = tokenEnumerator.MoveNext();
              break;
            }

          case "{":
            {
              var depth = stack.Count;
              var isDocument = (!Settings.IsFragment && depth == 0);

              if (isDocument)
              {
                stack.Push(NodeType.Document);
                var name = ValueConventions.CreateName("document", Settings, TextCase.CamelCase);
                yield return new Node { Type = NodeType.DocumentStart, Value = name };

                //
                // a proxima propriedade deve ser tratada como o nome do objeto ou coleção
                // exemplo:
                //   {
                //     "tipo": { ... }
                //   }
                //

                // nome da propriedade
                ready = tokenEnumerator.MoveNext();
                if (!ready) throw new SerializationException("Fim inesperado de arquivo.");
                var propertyName = tokenEnumerator.Current;
                
                // dois pontos
                ready = tokenEnumerator.MoveNext();
                if (!ready) throw new SerializationException("Fim inesperado de arquivo.");
                var delimiter = tokenEnumerator.Current;
                if (delimiter != ":") throw new SerializationException("Token não esperado: " + delimiter);

                suggestedName = propertyName;
              }
              else
              {
                stack.Push(NodeType.Object);
                var name = ValueConventions.CreateName(suggestedName ?? "object", Settings, TextCase.CamelCase);
                yield return new Node { Type = NodeType.ObjectStart, Value = name };
                suggestedName = null;
              }

              ready = tokenEnumerator.MoveNext();
              break;
            }

          case "[":
            {
              stack.Push(NodeType.Collection);
              var name = ValueConventions.CreateName(suggestedName ?? "collection", Settings, TextCase.CamelCase);
              yield return new Node { Type = NodeType.CollectionStart, Value = name };
              ready = tokenEnumerator.MoveNext();
              suggestedName = null;
              break;
            }

          case "]":
          case "}":
            {
              NodeType type;

              type = stack.FirstOrDefault();
              if (type == NodeType.Property)
              {
                stack.Pop();
                yield return new Node { Type = NodeType.PropertyEnd }; 
              }

              type = stack.Pop();
              yield return new Node { Type = type | NodeType.End };

              type = stack.FirstOrDefault();
              if (type == NodeType.Property)
              {
                stack.Pop();
                yield return new Node { Type = NodeType.PropertyEnd };
              }

              ready = tokenEnumerator.MoveNext();
              break;
            }

          default:
            {
              ready = tokenEnumerator.MoveNext();
              if (!ready) throw new SerializationException("Fim inesperado de arquivo.");

              var nextToken = tokenEnumerator.Current;
              if (nextToken == ":")
              {
                stack.Push(NodeType.Property);

                var name = ValueConventions.CreateName(token, Settings, TextCase.CamelCase);
                yield return new Node { Type = NodeType.PropertyStart, Value = name };

                ready = tokenEnumerator.MoveNext();
              }
              else
              {
                if (token.StartsWith("'"))
                  token = token.Substring(1, token.Length - 2);

                var value = ValueConventions.CreateValue(token, Settings, Toolset.Json.Unescape);
                yield return new Node { Type = NodeType.Value, Value = value };
              }
              break;
            }
        }

      }
    }

    private IEnumerable<string> EnumerateTokens()
    {
      while (true)
      {
        var characters = ReadCharacters();
        var text = new string(characters.ToArray());
        if (!string.IsNullOrWhiteSpace(text))
        {
          yield return text.Trim();
        }

        var token = ReadToken();
        if (token == '\x0')
        {
          break;
        }

        yield return token.ToString();
      }
    }

    private IEnumerable<char> ReadCharacters()
    {
      var symbols = this.tokens;
      
      var prev = '\x0';
      
      var quoted = false;
      var quoteChar = '\x0';

      int read;
      while ((read = reader.Peek()) != -1)
      {
        var ch = (char)read;

        if (quoted)
        {
          if (ch == quoteChar)
          {
            if (prev != '\\')
            {
              quoted = false;
            }
          }
        }
        else
        {
          if ((ch == '\'') || (ch == '"'))
          {
            quoted = true;
            quoteChar = ch;
          }
          else if (prev != '\\' && symbols.Contains(ch))
          {
            break;
          }
        }

        prev = ch;
        yield return ch;

        reader.Read();
      }
    }

    private char ReadToken()
    {
      int read = reader.Read();
      return (read > -1) ? (char)read : '\x0';
    }

  }
}

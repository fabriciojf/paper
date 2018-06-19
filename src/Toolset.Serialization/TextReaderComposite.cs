using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Serialization
{
  public class TextReaderComposite : TextReader
  {
    private readonly IEnumerable<TextReader> readers;
    private readonly IEnumerator<TextReader> enumerator;
    private TextReader reader;

    public TextReaderComposite(IEnumerable<TextReader> readers)
    {
      this.readers = readers;
      this.enumerator = readers.GetEnumerator();
      NextReader();
    }

    public TextReaderComposite(params TextReader[] readers)
    {
      this.readers = readers;
      this.enumerator = readers.Cast<TextReader>().GetEnumerator();
      NextReader();
    }

    public override int Peek()
    {
      if (reader == null)
        return -1;

      var result = reader.Peek();
      if (result == -1)
      {
        var ready = NextReader();
        if (!ready)
          return -1;
      }

      return reader.Peek();
    }

    public override int Read()
    {
      var result = Peek();
      if (result > -1)
        result = reader.Read();
      return result;
    }

    private bool NextReader()
    {
      var ready = enumerator.MoveNext();
      this.reader = ready ? enumerator.Current : null;
      return ready;
    }

    public override void Close()
    {
      foreach (var reader in readers)
      {
        try
        {
          reader.Close();
        }
        catch
        {
          // nada a fazer...
        }
      }
    }

  }
}

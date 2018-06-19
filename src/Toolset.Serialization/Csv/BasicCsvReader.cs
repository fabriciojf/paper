using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Toolset.Serialization.Csv
{
  internal sealed class BasicCsvReader : Reader
  {
    private readonly TextReader textReader;
    private readonly Queue<Node> queue;

    private IEnumerable<string> colNamesAlgorithm;

    private Node currentNode;
    private IEnumerator<string> rows;
    private IEnumerator<string> cols;
    private IEnumerator<string> colNames;

    private bool done;

    public BasicCsvReader(TextReader textReader)
      : base(null)
    {
      this.textReader = textReader;
      this.queue = new Queue<Node>();
      this.colNamesAlgorithm = CreateColNames();
    }

    public BasicCsvReader(TextReader textReader, SerializationSettings settings)
      : base(settings)
    {
      this.textReader = textReader;
      this.queue = new Queue<Node>();
      this.colNamesAlgorithm = CreateColNames();
    }

    public new CsvSerializationSettings Settings
    {
      get { return base.Settings.As<CsvSerializationSettings>(); }
    }

    public override Node Current
    {
      get { return currentNode; }
    }

    private IEnumerable<string> CreateColNames()
    {
      var colIndex = 0;
      while (++colIndex <= int.MaxValue)
      {
        var name = "Field" + colIndex;
        var headerName = ValueConventions.CreateName(name, Settings, TextCase.KeepOriginal);
        yield return headerName;
      }
    }

    public override void Close()
    {
      if (!Settings.KeepOpen)
      {
        textReader.Close();
      }
    }

    protected override bool DoRead()
    {
      if (queue.Count == 0)
      {
        var ok = FetchNodes();
        if (!ok)
          return false;
      }

      currentNode = queue.Dequeue();
      return true;
    }

    private bool FetchNodes()
    {
      if (done)
        return false;

      if (rows == null)
      {
        rows = EnumerateRows(textReader).GetEnumerator();
        if (Settings.HasHeaders)
        {
          var hasMoreRows = rows.MoveNext();
          if (!hasMoreRows)
          {
            done = true;
            EmitRowCollectionEnd();
            return true;
          }

          var colNames = EnumerateCols(rows.Current).ToArray();
          this.colNamesAlgorithm = colNames;
        }

        EmitRowCollectionStart();
        return true;
      }

      if (cols == null)
      {

        var hasMoreRows = rows.MoveNext();
        if (!hasMoreRows)
        {
          done = true;
          EmitRowCollectionEnd();
          return true;
        }

        cols = EnumerateCols(rows.Current).GetEnumerator();
        colNames = this.colNamesAlgorithm.GetEnumerator();

        EmitRowStart();
        return true;
      }

      var hasMoreCols = cols.MoveNext();
      if (!hasMoreCols)
      {
        cols = null;
        EmitRowEnd();
        return true;
      }

      var colValue = ValueConventions.CreateValue(cols.Current, Settings);
      EmitCol(colValue);
      return true;
    }
    
    #region Emissores de nodos...

    private void EmitRowCollectionStart()
    {
      var collectionName = ValueConventions.CreateName("Csv", Settings, TextCase.KeepOriginal);

      if (!Settings.IsFragment)
        queue.Enqueue(new Node { Type = NodeType.DocumentStart });

      queue.Enqueue(new Node { Type = NodeType.CollectionStart, Value = collectionName });
    }

    private void EmitRowCollectionEnd()
    {
      queue.Enqueue(new Node { Type = NodeType.CollectionEnd });

      if (!Settings.IsFragment)
        queue.Enqueue(new Node { Type = NodeType.DocumentEnd });
    }

    private void EmitRowStart()
    {
      var objectName = ValueConventions.CreateName("Row", Settings, TextCase.KeepOriginal);
      queue.Enqueue(new Node { Type = NodeType.ObjectStart, Value = objectName });
    }

    private void EmitRowEnd()
    {
      queue.Enqueue(new Node { Type = NodeType.ObjectEnd });
    }

    private void EmitCol(object value)
    {
      colNames.MoveNext();
      var colName = colNames.Current;

      queue.Enqueue(new Node { Type = NodeType.PropertyStart, Value = colName });
      queue.Enqueue(new Node { Type = NodeType.Value, Value = value });
      queue.Enqueue(new Node { Type = NodeType.PropertyEnd });
    }

    #endregion

    #region Algoritmos de extração de linhas e colunas do CSV...

    private IEnumerable<string> EnumerateRows(TextReader reader)
    {
      string line = null;
      do
      {
        line = reader.ReadLine();
        if (!string.IsNullOrWhiteSpace(line))
        {
          yield return line.Trim();
        }

      } while (line != null);
    }

    private IEnumerable<string> EnumerateCols(string line)
    {
      var terminators = EnumerateColTerminators(line);
      var inipos = 0;
      foreach (var endpos in terminators)
      {
        var length = endpos - inipos;
        var text = line.Substring(inipos, length);

        if (text.StartsWith("\'"))
        {
          text = '"' + text.Substring(1, text.Length - 2) + '"';
        }

        text = Regex.Unescape(text);
        text = text.Trim();
        yield return string.IsNullOrEmpty(text) ? null : text;

        inipos = endpos + 1;
      }
    }

    private IEnumerable<int> EnumerateColTerminators(string line)
    {
      var index = -1;
      while (true)
      {
        index = line.IndexOfAny(new[] { Settings.FieldDelimiter, '"', '\'' }, index + 1);
        if (index == -1)
        {
          yield return line.Length;
          break;
        }

        var ch = line[index];
        if ((ch == '"') || (ch == '\''))
        {

          while (true)
          {
            index = line.IndexOfAny(new[] { ch }, index + 1);
            if (index == -1)
              throw new Exception("Premature end of text.");

            var prevChar = (index > 0) ? line[index - 1] : default(char);
            var isEscaped = (prevChar == '\\');
            if (!isEscaped)
              break;
          }

          continue;
        }

        yield return index;
      }

    }

    #endregion

  }
}

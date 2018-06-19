using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Toolset.Serialization.Excel
{
  /// <summary>
  /// Exportador de arquivos XLSX.
  /// Referências:
  /// - https://msdn.microsoft.com/en-us/library/aa338205.aspx
  /// - http://www.ecma-international.org/publications/standards/Ecma-376.htm
  /// </summary>
  internal sealed class BasicExcelWriter : Writer
  {
    private readonly Stream output;
    private readonly WorkbookBuilder workbook;

    private SheetBuilder sheet;
    private int collectionDepth;

    public BasicExcelWriter(Stream stream, SerializationSettings settings)
      : base(settings)
    {
      this.output = stream;
      this.workbook = new WorkbookBuilder(this.Settings);
    }

    public new ExcelSerializationSettings Settings
    {
      get { return base.Settings.As<ExcelSerializationSettings>(); }
    }

    protected override void DoWrite(Node node)
    {
      switch (node.Type)
      {
        case NodeType.CollectionStart:
          {
            collectionDepth++;

            if (collectionDepth == 1)
            {
              string name =
                (node.Value != null)
                  ? node.Value.ToString()
                  : "Planilha" + (workbook.Sheets.Count + 1);
              var conventionName = ValueConventions.CreateName(name, Settings, ExcelWriter.DefaultTextCase);

              sheet = workbook.CreateSheet(conventionName);
              sheet.StartSheet();
            }
            else if (collectionDepth == 2)
            {
              sheet.StartRow();
            }

            break;
          }

        case NodeType.CollectionEnd:
          {
            if (collectionDepth == 2)
            {
              sheet.EndRow();
            }
            else if (collectionDepth == 1)
            {
              sheet.EndSheet();
              sheet = null;
            }
            collectionDepth--;
            break;
          }

        case NodeType.Value:
          {
            sheet.Cell(node.Value);
            break;
          }
      }
    }

    protected override void DoWriteComplete()
    {
      workbook.Write(output);
      output.Flush();
    }

    protected override void DoFlush()
    {
      output.Flush();
    }

    protected override void DoClose()
    {
      output.Flush();
      if (!Settings.KeepOpen)
      {
        output.Close();
      }
    }
  }
}

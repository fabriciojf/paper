using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

namespace Toolset.Serialization.Excel
{
  public sealed class SheetBuilder
  {
    private const string ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    private readonly Sheet sheet;
    private readonly XmlWriter writer;

    private int currentRowNumber = 0;
    private int currentCellNumber = 0;

    internal SheetBuilder(Sheet sheet, XmlWriterSettings settings)
    {
      this.sheet = sheet;
      this.writer = XmlWriter.Create(sheet.TempFilename, settings);
    }

    public void StartSheet()
    {
      writer.WriteStartDocument();
      writer.WriteStartElement("worksheet", ns);
      writer.WriteStartElement("sheetData", ns);
    }

    public void StartRow()
    {
      StartRow(++currentRowNumber);
    }

    public void StartRow(int rowNumber)
    {
      currentRowNumber = rowNumber;
      currentCellNumber = 0;

      writer.WriteStartElement("row", ns);
      writer.WriteAttributeString("r", rowNumber.ToString());
    }

    public void Cell(object cellValue)
    {
      Cell(++currentCellNumber, cellValue);
    }

    public void Cell(int cellNumber, object cellValue)
    {
      var cellInfo = CreateCellInfo(cellValue);
      var cellName = CreateCellName(currentRowNumber, cellNumber);

      writer.WriteStartElement("c", ns);
      writer.WriteAttributeString("r", cellName);
      writer.WriteAttributeString("s", cellInfo.Style);
      writer.WriteAttributeString("t", cellInfo.Type);

      if (cellInfo.Type == CellInfo.TextType)
      {
        writer.WriteStartElement("is", ns);
        writer.WriteStartElement("t", ns);
        writer.WriteValue(cellInfo.Value);
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
      else
      {
        writer.WriteStartElement("v", ns);
        writer.WriteValue(cellInfo.Value);
        writer.WriteEndElement();
      }

      writer.WriteEndElement();

      this.currentCellNumber = cellNumber;
    }

    public void EndRow()
    {
      writer.WriteEndElement();
    }

    public void EndSheet()
    {
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndDocument();

      writer.Flush();
      writer.Close();
    }

    #region Algoritmos auxiliares...

    private string CreateCellName(int rowNumber, int cellNumber)
    {
      var name = "";
      
      var index = cellNumber - 1;
      var letters = 26;
      while (index >= 0)
      {
        var remaining = index % letters;
        index = (index - letters - remaining) / letters;
        name += (char)(remaining + 'A');
      }

      return name + rowNumber;
    }

    private CellInfo CreateCellInfo(object value)
    {
      if (value == null)
      {
        return new CellInfo
        {
          Type = CellInfo.TextType,
          Style = CellInfo.GeneralStyle,
          Value = ""
        };
      }
      if (value is bool)
      {
        return new CellInfo
        {
          Type = CellInfo.BooleanType,
          Style = CellInfo.GeneralStyle,
          Value = ((bool)value) ? "1" : "0"
        };
      }
      if (value is DateTime)
      {
        var culture = CultureInfo.InvariantCulture;
        return new CellInfo
        {
          Type = CellInfo.NumberType,
          Style = CellInfo.DateStyle,
          Value = ((DateTime)value).ToOADate().ToString(culture)
        };
      }
      else if (IsNumber(value))
      {
        var type = value.GetType();
        var culture = CultureInfo.InvariantCulture;
        var formatter = type.GetMethod("ToString", new[] { typeof(IFormatProvider) });
        return new CellInfo
        {
          Type = CellInfo.NumberType,
          Style = CellInfo.GeneralStyle,
          Value = (string)formatter.Invoke(value, new[] { culture })
        };
      }
      else
      {
        var type = value.GetType();
        var culture = CultureInfo.InvariantCulture;
        var formatter = type.GetMethod("ToString", new[] { typeof(IFormatProvider) });
        return new CellInfo
        {
          Type = CellInfo.TextType,
          Style = CellInfo.GeneralStyle,
          Value = (value ?? "").ToString()
        };
      }
    }

    private bool IsNumber(object value)
    {
      return value is sbyte
          || value is byte
          || value is short
          || value is ushort
          || value is int
          || value is uint
          || value is long
          || value is ulong
          || value is float
          || value is double
          || value is decimal;
    }

    private struct CellInfo
    {
      public const string DateType = "d";
      public const string NumberType = "n";
      public const string BooleanType = "b";
      public const string TextType = "inlineStr";

      public const string GeneralStyle = "0";
      public const string DateStyle = "1";

      public string Type;
      public string Style;
      public string Value;
    }

    #endregion
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Globalization;
using System.Collections.ObjectModel;
using System.IO.Compression;

namespace Toolset.Serialization.Excel
{
  public sealed class WorkbookBuilder
  {
    private const string ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    private readonly XmlWriterSettings xmlSettings;
    private readonly ExcelSerializationSettings settings;
    private readonly List<Sheet> sheets;

    public WorkbookBuilder()
      : this(null)
    {
      // nada a fazer aqui. use o outro construtor.
    }

    public WorkbookBuilder(ExcelSerializationSettings settings)
    {
      this.sheets = new List<Sheet>();
      this.settings = settings ?? new ExcelSerializationSettings();
      this.xmlSettings = new XmlWriterSettings
      {
        Indent = this.settings.Indent,
        IndentChars = this.settings.IndentChars,
        Encoding = Encoding.UTF8
      };
    }

    public ReadOnlyCollection<Sheet> Sheets
    {
      get { return readonlySheets ?? (readonlySheets = new ReadOnlyCollection<Sheet>(sheets)); }
    }
    private ReadOnlyCollection<Sheet> readonlySheets;

    public SheetBuilder CreateSheet()
    {
      return CreateSheet(null);
    }

    public SheetBuilder CreateSheet(string sheetLabel)
    {
      var sheetIndex = (sheets.Count + 1);

      var id = sheetIndex;
      var name = "sheet" + id;
      var zipEntryName = "/xl/worksheets/" + name + ".xml";
      var tempFilename = Path.GetTempFileName();
      
      var sheet = new Sheet
      {
        Id = id,
        Name = name,
        Label = sheetLabel ?? ("Planilha" + sheetIndex),
        ZipEntryName = zipEntryName,
        TempFilename = tempFilename
      };

      sheets.Add(sheet);

      return new SheetBuilder(sheet, xmlSettings);
    }

    public void Write(Stream stream)
    {
      using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
      {
        WriteStyles(zip);
        WriteSheets(zip);
        WriteWorkbook(zip);
        WriteWorkbookRelationships(zip);
        WritePackageRelationships(zip);
        WritePackageIndex(zip);
      }
    }

    #region Algoritmos de escrita da planilha do Excel...

    private void WritePackageIndex(ZipArchive zip)
    {
      var entry = zip.CreateEntry("[Content_Types].xml");
      using (var stream = entry.Open())
      {
        var ns = (XNamespace)"http://schemas.openxmlformats.org/package/2006/content-types";

        var xml =
          new XDocument(
            new XElement(ns + "Types",
              new[] {
                    new XElement(ns + "Override",
                      new XAttribute("PartName", "/_rels/.rels"),
                      new XAttribute("ContentType", "application/vnd.openxmlformats-package.relationships+xml")
                    ),
                    new XElement(ns + "Override",
                      new XAttribute("PartName", "/xl/_rels/workbook.xml.rels"),
                      new XAttribute("ContentType", "application/vnd.openxmlformats-package.relationships+xml")
                    ),
                    new XElement(ns + "Override",
                      new XAttribute("PartName", "/xl/styles.xml"),
                      new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml")
                    ),
                    new XElement(ns + "Override",
                      new XAttribute("PartName", "/xl/workbook.xml"),
                      new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml")
                    )
                }.Union(
                sheets.Select(sheet =>
                  new XElement(ns + "Override",
                    new XAttribute("PartName", sheet.ZipEntryName),
                    new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml")
                  )
                )
              )
            )
          );

        var writer = XmlWriter.Create(stream, xmlSettings);
        xml.WriteTo(writer);
        writer.Flush();
      }
    }

    private void WritePackageRelationships(ZipArchive zip)
    {
      var entry = zip.CreateEntry("_rels/.rels");
      using (var stream = entry.Open())
      {
        var xml = XDocument.Parse(
          "<Relationships xmlns='http://schemas.openxmlformats.org/package/2006/relationships'>"
        + "  <Relationship Id='workbook' Type='http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument' Target='xl/workbook.xml' />"
        + "</Relationships>"
        );
        var writer = XmlWriter.Create(stream, xmlSettings);
        xml.WriteTo(writer);
        writer.Flush();
      }
    }

    private void WriteStyles(ZipArchive zip)
    {
      var entry = zip.CreateEntry("xl/styles.xml");
      using (var stream = entry.Open())
      {
        var dateFormat = settings.DateTimeFormat;
        var xml = XDocument.Parse(
          "<styleSheet xmlns='http://schemas.openxmlformats.org/spreadsheetml/2006/main'>"
        + "  <numFmts count='2'>"
        + "    <numFmt numFmtId='164' formatCode='GENERAL' />"
        + "    <numFmt numFmtId='165' formatCode='" + dateFormat + "' />"
        + "  </numFmts>"
        + "  <cellStyleXfs count='2'>"
        + "    <xf numFmtId='164'/>"
        + "  </cellStyleXfs>"
        + "  <cellXfs count='2'>"
        + "    <xf numFmtId='164' xfId='0'/>"
        + "    <xf numFmtId='165' xfId='0'/>"
        + "  </cellXfs>"
        + "</styleSheet>"
        );
        var writer = XmlWriter.Create(stream, xmlSettings);
        xml.WriteTo(writer);
        writer.Flush();
      }
    }

    private void WriteWorkbook(ZipArchive zip)
    {
      var entry = zip.CreateEntry("xl/workbook.xml");
      using (var stream = entry.Open())
      {
        var ns = (XNamespace)"http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        var nsr = (XNamespace)"http://schemas.openxmlformats.org/officeDocument/2006/relationships";

        var xml =
          new XDocument(
            new XElement(ns + "workbook",
              new XAttribute(XNamespace.Xmlns + "r", nsr),
              new XElement(ns + "sheets",
                sheets.Select(sheet =>
                  new XElement(ns + "sheet",
                    new XAttribute("name", sheet.Label),
                    new XAttribute("sheetId", sheet.Id),
                    new XAttribute("state", "visible"),
                    new XAttribute(nsr + "id", sheet.Name)
                  )
                )
              )
            )
          );

        var writer = XmlWriter.Create(stream, xmlSettings);
        xml.WriteTo(writer);
        writer.Flush();
      }
    }

    private void WriteWorkbookRelationships(ZipArchive zip)
    {
      var entry = zip.CreateEntry("xl/_rels/workbook.xml.rels");
      using (var stream = entry.Open())
      {
        var ns = (XNamespace)"http://schemas.openxmlformats.org/package/2006/relationships";

        var xml =
          new XDocument(
            new XElement(ns + "Relationships",
              new XElement(ns + "Relationship",
                new XAttribute("Id", "styles"),
                new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"),
                new XAttribute("Target", "/xl/styles.xml")
              ),
              sheets.Select(sheet =>
                new XElement(ns + "Relationship",
                  new XAttribute("Id", sheet.Name),
                  new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"),
                  new XAttribute("Target", sheet.ZipEntryName)
                )
              )
            )
          );

        var writer = XmlWriter.Create(stream, xmlSettings);
        xml.WriteTo(writer);
        writer.Flush();
      }
    }

    private void WriteSheets(ZipArchive zip)
    {
      foreach (var sheet in sheets)
      {
        var zipEntry = sheet.ZipEntryName;
        var filename = sheet.TempFilename;

        var entry = zip.CreateEntry(zipEntry);
        using (var stream = entry.Open())
        {
          using (var file = File.OpenRead(filename))
          {
            file.CopyTo(stream);
          }
        }
      }
    }

    #endregion
  }
}
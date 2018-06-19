using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Toolset.Xml
{
  /// <summary>
  /// Utilitário para exportação de esquemas XSD baseado em objetos mapeados
  /// com System.Runtime.Serialization.DataContractAttribute.
  /// </summary>
  public class DataContractXsdExporter
  {
    /// <summary>
    /// Extrai todos os esquemas XSD do tipo indicado e exporta para a pasta destino.
    /// </summary>
    /// <typeparam name="T">O tipo mapeado com System.Runtime.Serialization.DataContractAttribute</typeparam>
    /// <param name="targetFolder">A pasta destino.</param>
    public void Export<T>(string targetFolder)
      where T : class
    {
      Export(typeof(T), targetFolder);
    }

    /// <summary>
    /// Extrai todos os esquemas XSD do tipo indicado e exporta para a pasta destino.
    /// </summary>
    /// <param name="type">O tipo mapeado com System.Runtime.Serialization.DataContractAttribute</param>
    /// <param name="targetFolder">A pasta destino.</param>
    public void Export(Type type, string targetFolder)
    {
      var exporter = new XsdDataContractExporter();
      exporter.Export(type);

      targetFolder = targetFolder ?? "";
      if (targetFolder.Length > 0 && !Directory.Exists(targetFolder))
      {
        Directory.CreateDirectory(targetFolder);
      }

      var index = 0;
      foreach (var schema in exporter.Schemas.Schemas().Cast<XmlSchema>())
      {
        var xmlns = schema.TargetNamespace ?? ("schema-" + ++index);
        var name =
          Regex.Replace(
            Regex.Replace(xmlns, "[^0-9a-zA-Z-._]", "-")
            , "-+", "-"
          );

        if (name.StartsWith("-")) name = name.Substring(1);
        if (name.EndsWith("-")) name = name.Substring(0, name.Length - 1);

        var filename = name + ".xsd";
        var filepath = Path.Combine(targetFolder ?? "", filename);

        using (var stream = new FileStream(filepath, FileMode.Create))
        {
          schema.Write(stream);
          stream.Flush();
        }
      }
    }
  }
}

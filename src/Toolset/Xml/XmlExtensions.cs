using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Toolset.Xml
{
  /// <summary>
  /// Coleção de métodos de extensão para conversão entre diferentes representações de XML.
  /// </summary>
  public static class XmlExtensions
  {
    #region ToXmlObject<T>

    public static T ToXmlObject<T>(this object valor)
    {
      return (T)ToXmlObject(valor, typeof(T));
    }

    public static T ToXmlObject<T>(this Stream valor)
    {
      return (T)ToXmlObject(valor, typeof(T));
    }

    public static T ToXmlObject<T>(this String valor)
    {
      return (T)ToXmlObject(valor, typeof(T));
    }

    public static T ToXmlObject<T>(this XmlNode valor)
    {
      return (T)ToXmlObject(valor, typeof(T));
    }

    public static T ToXmlObject<T>(this XContainer valor)
    {
      return (T)ToXmlObject(valor, typeof(T));
    }

    #endregion

    #region ToXmlObject

    public static Object ToXmlObject(this object valor, Type tipo)
    {
      if (valor is XContainer)
        return ToXmlObject((XContainer)valor, tipo);
      if (valor is XmlNode)
        return ToXmlObject((XmlNode)valor, tipo);
      if (valor is Stream)
        return ToXmlObject((Stream)valor, tipo);
      if (valor is string)
        return ToXmlObject((string)valor, tipo);

      return valor;
    }

    public static Object ToXmlObject(this Stream valor, Type tipo)
    {
      return Deserializar(tipo, valor);
    }

    public static Object ToXmlObject(this String valor, Type tipo)
    {
      using (var leitor = new StringReader(valor))
      {
        return Deserializar(tipo, leitor);
      }
    }

    public static Object ToXmlObject(this XmlNode valor, Type tipo)
    {
      using (var leitor = valor.CreateNavigator().ReadSubtree())
      {
        return Deserializar(tipo, leitor);
      }
    }

    public static Object ToXmlObject(this XContainer valor, Type tipo)
    {
      return Deserializar(tipo, valor.CreateReader());
    }

    #endregion

    #region ToXmlStream

    public static Stream ToXmlStream(this Object valor)
    {
      if (valor is XContainer)
        return ToXmlStream((XContainer)valor);
      if (valor is XmlNode)
        return ToXmlStream((XmlNode)valor);
      if (valor is Stream)
        return (Stream)valor;
      if (valor is string)
        return ToXmlStream((string)valor);

      var memoria = new MemoryStream();
      try
      {
        using (var writer = XmlWriter.Create(memoria,
          new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8, OmitXmlDeclaration = true }))
        {
          Serializar(valor, writer);

          memoria.Position = 0;

          return memoria;
        }
      }
      catch (Exception ex)
      {
        memoria.Dispose();
        throw ex;
      }
    }

    public static Stream ToXmlStream(this String valor)
    {
      var memoria = new MemoryStream();
      try
      {
        var bytes = Encoding.UTF8.GetBytes(valor);
        memoria.Write(bytes, 0, bytes.Length);
        memoria.Flush();
        memoria.Position = 0;
        return memoria;
      }
      catch (Exception ex)
      {
        memoria.Dispose();
        throw ex;
      }
    }

    public static Stream ToXmlStream(this XmlNode valor)
    {
      var memoria = new MemoryStream();
      try
      {
        using (var saida = XmlWriter.Create(memoria,
          new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8, OmitXmlDeclaration = true }))
        {
          valor.WriteTo(saida);
        }
        memoria.Flush();
        memoria.Position = 0;
        return memoria;
      }
      catch (Exception ex)
      {
        memoria.Dispose();
        throw ex;
      }
    }

    public static Stream ToXmlStream(this XContainer valor)
    {
      var memoria = new MemoryStream();
      try
      {
        if (valor is XDocument)
        {
          ((XDocument)valor).Save(memoria, SaveOptions.DisableFormatting);
        }
        else
        {
          ((XElement)valor).Save(memoria, SaveOptions.DisableFormatting);
        }
        memoria.Flush();
        memoria.Position = 0;
        return memoria;
      }
      catch (Exception ex)
      {
        memoria.Dispose();
        throw ex;
      }
    }

    #endregion

    #region ToXmlStream para um Stream determinado

    public static void ToXmlStream(this Object valor, Stream stream)
    {
      if (valor is XContainer)
      {
        ToXmlStream((XContainer)valor, stream);
        return;
      }
      if (valor is XmlNode)
      {
        ToXmlStream((XmlNode)valor, stream);
        return;
      }
      if (valor is Stream)
      {
        ((Stream)valor).CopyTo(stream);
        return;
      }
      if (valor is string)
      {
        ToXmlStream((string)valor, stream);
        return;
      }

      var stringWriter = new StringWriter();
      using (var writer = XmlWriter.Create(stringWriter,
        new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8, OmitXmlDeclaration = true }))
      {
        Serializar(valor, writer);
        var xml = stringWriter.ToString();

        var bytes = Encoding.UTF8.GetBytes(xml);
        stream.Write(bytes, 0, bytes.Length);
        stream.Flush();

        if (stream is MemoryStream)
        {
          ((MemoryStream)stream).Position = 0;
        }
      }
    }

    public static void ToXmlStream(this String valor, Stream stream)
    {
      var bytes = Encoding.UTF8.GetBytes(valor);
      stream.Write(bytes, 0, bytes.Length);
      stream.Flush();

      if (stream is MemoryStream)
      {
        ((MemoryStream)stream).Position = 0;
      }
    }

    public static void ToXmlStream(this XmlNode valor, Stream stream)
    {
      using (var saida = XmlWriter.Create(stream,
        new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8, OmitXmlDeclaration = true }))
      {
        valor.WriteTo(saida);
      }
      stream.Flush();
      stream.Position = 0;

      if (stream is MemoryStream)
      {
        ((MemoryStream)stream).Position = 0;
      }
    }

    public static void ToXmlStream(this XContainer valor, Stream stream)
    {
      if (valor is XDocument)
      {
        ((XDocument)valor).Save(stream, SaveOptions.DisableFormatting);
      }
      else
      {
        ((XElement)valor).Save(stream, SaveOptions.DisableFormatting);
      }
      stream.Flush();
      stream.Position = 0;

      if (stream is MemoryStream)
      {
        ((MemoryStream)stream).Position = 0;
      }
    }

    #endregion

    #region ToXmlWriter

    public static void ToXmlWriter(this Object valor, XmlWriter writer)
    {
      if (valor is XContainer)
      {
        ToXmlWriter((XContainer)valor, writer);
        return;
      }
      if (valor is XmlNode)
      {
        ToXmlWriter((XmlNode)valor, writer);
        return;
      }
      if (valor is Stream)
      {
        using (var reader = XmlReader.Create((Stream)valor))
        {
          writer.WriteNode(reader, true);
        }
      }
      if (valor is string)
      {
        ToXmlWriter((string)valor, writer);
        return;
      }

      Serializar(valor, writer);
    }

    public static void ToXmlWriter(this String valor, XmlWriter writer)
    {
      var xml = XElement.Parse(valor);
      xml.Save(writer);
    }

    public static void ToXmlWriter(this XmlNode valor, XmlWriter writer)
    {
      valor.WriteTo(writer);
    }

    public static void ToXmlWriter(this XContainer valor, XmlWriter writer)
    {
      if (valor is XDocument)
      {
        ((XDocument)valor).Save(writer);
      }
      else
      {
        ((XElement)valor).Save(writer);
      }
    }

    #endregion

    #region ToXmlElement

    public static XmlElement ToXmlElement(this Object valor)
    {
      if (valor is XContainer)
        return ToXmlElement((XContainer)valor);
      if (valor is XmlNode)
        return ToXmlElement((XmlNode)valor);
      if (valor is Stream)
        return ToXmlElement((Stream)valor);
      if (valor is string)
        return ToXmlElement((string)valor);

      using (var memoria = new MemoryStream())
      {
        var formato = new XmlWriterSettings
        {
          Encoding = Encoding.UTF8,
          Indent = false,
        };
        var writer = XmlTextWriter.Create(memoria, formato);

        Serializar(valor, writer);

        writer.Flush();
        memoria.Position = 0;

        var xml = new XmlDocument();
        xml.PreserveWhitespace = false;
        xml.Load(memoria);
        return xml.DocumentElement;
      }
    }

    public static XmlElement ToXmlElement(this Stream valor)
    {
      var xml = new XmlDocument();
      xml.PreserveWhitespace = false;
      xml.Load(valor);
      return xml.DocumentElement;
    }

    public static XmlElement ToXmlElement(this String valor)
    {
      var xml = new XmlDocument();
      xml.PreserveWhitespace = false;
      xml.LoadXml(valor);
      return xml.DocumentElement;
    }

    public static XmlElement ToXmlElement(this XmlNode valor)
    {
      if (valor is XmlDocument)
      {
        return ((XmlDocument)valor).DocumentElement;
      }
      else
      {
        return (XmlElement)valor;
      }
    }

    public static XmlElement ToXmlElement(this XContainer valor)
    {
      using (var leitor = valor.CreateReader())
      {
        var xml = new XmlDocument();
        xml.PreserveWhitespace = false;
        xml.Load(leitor);
        return xml.DocumentElement;
      }
    }

    #endregion

    #region ToXmlDocument

    public static XmlDocument ToXmlDocument(this Object valor)
    {
      if (valor is XContainer)
        return ToXmlDocument((XContainer)valor);
      if (valor is XmlNode)
        return ToXmlDocument((XmlNode)valor);
      if (valor is Stream)
        return ToXmlDocument((Stream)valor);
      if (valor is string)
        return ToXmlDocument((string)valor);

      using (var memoria = new MemoryStream())
      {
        var formato = new XmlWriterSettings
        {
          Encoding = Encoding.UTF8,
          Indent = false,
        };
        var writer = XmlTextWriter.Create(memoria, formato);

        Serializar(valor, writer);

        writer.Flush();
        memoria.Position = 0;

        var xml = new XmlDocument();
        xml.PreserveWhitespace = false;
        xml.Load(memoria);
        return xml;
      }
    }

    public static XmlDocument ToXmlDocument(this Stream valor)
    {
      var xml = new XmlDocument();
      xml.PreserveWhitespace = false;
      xml.Load(valor);
      return xml;
    }

    public static XmlDocument ToXmlDocument(this String valor)
    {
      var xml = new XmlDocument();
      xml.PreserveWhitespace = false;
      xml.LoadXml(valor);
      return xml;
    }

    public static XmlDocument ToXmlDocument(this XmlNode valor)
    {
      if (valor is XmlDocument)
      {
        return (XmlDocument)valor;
      }
      else
      {
        var xml = new XmlDocument();
        using (XmlReader leitor = valor.CreateNavigator().ReadSubtree())
        {
          xml.Load(leitor);
        }
        return xml;
      }
    }

    public static XmlDocument ToXmlDocument(this XContainer valor)
    {
      using (var leitor = valor.CreateReader())
      {
        var xml = new XmlDocument();
        xml.PreserveWhitespace = false;
        xml.Load(leitor);
        return xml;
      }
    }

    #endregion

    #region ToXElement

    public static XElement ToXElement(this Object valor)
    {
      if (valor is XContainer)
        return ToXElement((XContainer)valor);
      if (valor is XmlNode)
        return ToXElement((XmlNode)valor);
      if (valor is Stream)
        return ToXElement((Stream)valor);
      if (valor is string)
        return ToXElement((string)valor);

      var xml = new XDocument();
      using (var saida = xml.CreateWriter())
      {
        Serializar(valor, saida);
      }
      return xml.Root;
    }

    public static XElement ToXElement(this Stream valor)
    {
#if NET35
      return Xmls.Load(valor);
#else
      var xml = XDocument.Load(valor);
      return xml.Root;
#endif
    }

    public static XElement ToXElement(this String valor)
    {
      var xml = XDocument.Parse(valor);
      return xml.Root;
    }

    public static XElement ToXElement(this XmlNode valor)
    {
      using (XmlReader leitor = valor.CreateNavigator().ReadSubtree())
      {
        var xml = XDocument.Load(leitor);
        return xml.Root;
      }
    }

    public static XElement ToXElement(this XContainer valor)
    {
      return (valor is XDocument) ? ((XDocument)valor).Root : (XElement)valor;
    }

    #endregion

    #region ToXDocument

    public static XDocument ToXDocument(this Object valor)
    {
      if (valor is XContainer)
        return ToXDocument((XContainer)valor);
      if (valor is XmlNode)
        return ToXDocument((XmlNode)valor);
      if (valor is Stream)
        return ToXDocument((Stream)valor);
      if (valor is string)
        return ToXDocument((string)valor);

      var xml = new XDocument();
      using (var saida = xml.CreateWriter())
      {
        Serializar(valor, saida);
      }
      return xml;
    }

    public static XDocument ToXDocument(this Stream valor)
    {
#if NET35
      return Xmls.Load(valor);
#else
      return XDocument.Load(valor);
#endif
    }

    public static XDocument ToXDocument(this String valor)
    {
      return XDocument.Parse(valor);
    }

    public static XDocument ToXDocument(this XmlNode valor)
    {
      using (XmlReader leitor = valor.CreateNavigator().ReadSubtree())
      {
        return XDocument.Load(leitor);
      }
    }

    public static XDocument ToXDocument(this XContainer valor)
    {
      return (valor is XDocument) ? (XDocument)valor : new XDocument(valor);
    }

    #endregion

    #region ToXmlString

    public static String ToXmlString(this Object valor)
    {
      if (valor is XContainer)
        return ToXmlString((XContainer)valor);
      if (valor is XmlNode)
        return ToXmlString((XmlNode)valor);
      if (valor is Stream)
        return ToXmlString((Stream)valor);
      if (valor is string)
        return (string)valor;

      using (var memoria = new MemoryStream())
      {
        var formato = new XmlWriterSettings
        {
          Encoding = Encoding.UTF8,
          Indent = false,
          OmitXmlDeclaration = true
        };
        var xmlWriter = XmlWriter.Create(memoria, formato);

        Serializar(valor, xmlWriter);
        xmlWriter.Flush();

        memoria.Position = 0;
        var xml = new StreamReader(memoria).ReadToEnd();
        return xml;
      }
    }

    public static String ToXmlString(this Stream valor)
    {
      using (var stream = new StreamReader(valor))
      {
        return stream.ReadToEnd();
      }
    }

    public static String ToXmlString(this XmlNode valor)
    {
      return valor.OuterXml;
    }

    public static String ToXmlString(this XContainer valor)
    {
      return valor.ToString(SaveOptions.DisableFormatting);
    }

    #endregion

    #region ToXmlTrace

    public static void ToXmlTrace(this XContainer valor)
    {
      Trace.WriteLine(valor.ToString());
    }

    public static void ToXmlTrace(this String valor)
    {
      var stringReader = new StringReader(valor);
      var xmlTextReader = XmlTextReader.Create(stringReader);

      var saida = new StringWriter();
      using (var xmlTextWriter = XmlTextWriter.Create(saida, new XmlWriterSettings { Indent = true }))
      {
        xmlTextWriter.WriteNode(xmlTextReader, true);
      }

      Trace.WriteLine(saida.ToString());
    }

    public static void ToXmlTrace(this XmlNode valor)
    {
      using (XmlReader leitor = valor.CreateNavigator().ReadSubtree())
      {
        var xmlFormatado = XElement.Load(leitor);
        Trace.WriteLine(xmlFormatado);
      }
    }

    public static void ToXmlTrace(this Stream valor)
    {
#if NET35
      Trace.WriteLine(Xmls.Load(valor).ToString());
#else
      Trace.WriteLine(XElement.Load(valor).ToString());
#endif
    }

    public static void ToXmlTrace(this Object valor)
    {
      var xml = new XDocument();
      using (var saida = xml.CreateWriter())
      {
        Serializar(valor, saida);
      }
      Trace.WriteLine(xml.Root.ToString());
    }

    #endregion

    #region Serializadores

    private static void Serializar(object objeto, XmlWriter saida)
    {
      var tipo = objeto.GetType();
      var isXmlType = tipo.GetCustomAttributes(typeof(XmlTypeAttribute), true).Any();
      var isDataContract =
           tipo.GetCustomAttributes(typeof(CollectionDataContractAttribute), true).Any()
        || tipo.GetCustomAttributes(typeof(DataContractAttribute), true).Any();

      if (isXmlType || !isDataContract)
      {
        var serializador = new XmlSerializer(tipo);
        serializador.Serialize(saida, objeto);
      }
      else
      {
        var serializador = new DataContractSerializer(tipo);
        serializador.WriteObject(saida, objeto);
      }

      saida.Flush();
    }

    private static object Deserializar(Type tipo, Stream entrada)
    {
      return Deserializar(tipo, XmlReader.Create(entrada));
    }

    private static object Deserializar(Type tipo, TextReader entrada)
    {
      return Deserializar(tipo, XmlReader.Create(entrada));
    }

    private static object Deserializar(Type tipo, XmlReader entrada)
    {
      bool isXmlType = tipo.GetCustomAttributes(typeof(XmlTypeAttribute), true).Any();
      bool isDataContract =
           tipo.GetCustomAttributes(typeof(DataContractAttribute), true).Any()
        || tipo.GetCustomAttributes(typeof(CollectionDataContractAttribute), true).Any();

      if (IsDataContract(tipo))
      {
        var serializador = new DataContractSerializer(tipo);
        return serializador.ReadObject(entrada);
      }
      else
      {
        var serializador = new XmlSerializer(tipo);
        return serializador.Deserialize(entrada);
      }
    }

    internal static bool IsDataContract(Type tipo)
    {
      bool mapeadoComXmlType = tipo.GetCustomAttributes(typeof(XmlTypeAttribute), true).Any();
      bool mapeadoComDataContract =
           tipo.GetCustomAttributes(typeof(DataContractAttribute), true).Any()
        || tipo.GetCustomAttributes(typeof(CollectionDataContractAttribute), true).Any();

      bool prevalescerXmlType = mapeadoComXmlType || !mapeadoComDataContract;
      return !prevalescerXmlType;
    }

    #endregion
  }
}

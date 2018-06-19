using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Toolset;

namespace Paper.Media
{
  [Serializable]
  public class CaseVariantString : IXmlSerializable
  {
    public string Value { get; set; }

    public CaseVariantString()
    {
    }

    public CaseVariantString(string value)
    {
      this.Value = value;
    }

    public override string ToString()
    {
      return Value.ToString();
    }

    public override bool Equals(object obj)
    {
      return Value.Equals(obj);
    }

    public override int GetHashCode()
    {
      return Value.GetHashCode();
    }

    public string ChangeCase(TextCase textCase)
    {
      return Value.ChangeCase(textCase);
    }

    XmlSchema IXmlSerializable.GetSchema()
    {
      return null;
    }

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      this.Value = reader.ReadString();
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
      writer.WriteString(this.Value);
    }

    public static implicit operator string(CaseVariantString text)
    {
      return text?.Value;
    }

    public static implicit operator CaseVariantString(string text)
    {
      return (text == null) ? null : new CaseVariantString(text);
    }
  }
}

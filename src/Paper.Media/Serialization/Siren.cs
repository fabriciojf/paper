using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paper.Media.Serialization
{
  public static class Siren
  {
    /// <summary>
    /// Mime type "application/vnd.siren+json" para serialização de entidade em forma de JSON.
    /// </summary>
    public const string JsonMimetype = "application/vnd.siren+json";

    /// <summary>
    /// Mime type "application/xml" para serialização de entidade em forma de XML.
    /// <note>
    /// O formato "application/vnd.siren+xml" ainda não é suportado.
    /// </note>
    /// </summary>
    public const string XmlMimetype = "application/xml";
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paper.Media.Service
{
  public class PaperSettingsBuilder
  {
    private string pathBase;
    private readonly List<Uri> remotePaperServerUris = new List<Uri>();

    public PaperSettingsBuilder UsePathBase(string pathBase)
    {
      this.pathBase = pathBase;
      return this;
    }

    public void UseRemotePaperServer(string remotePaperServerUri)
    {
      var uri = new Uri(remotePaperServerUri, UriKind.RelativeOrAbsolute);
      this.remotePaperServerUris.Add(uri);
    }

    public IPaperSettings Build()
    {
      return new PaperSettings
      {
        PathBase = pathBase,
        RemotePaperServerUris = this.remotePaperServerUris.Distinct().ToArray()
      };
    }
  }
}

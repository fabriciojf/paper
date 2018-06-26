using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Paper.Media.Service
{
  internal class PaperSettings : IPaperSettings
  {
    public Uri BaseUri { get; internal set; }
    public PathString PathBase { get; internal set; }
    public Uri[] RemotePaperServerUris { get; internal set; }
  }
}

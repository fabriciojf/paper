using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Paper.Core.Service
{
  public interface IPaperSettings
  {
    Uri BaseUri { get; }
    PathString PathBase { get; }
    Uri[] RemotePaperServerUris { get; }
  }
}

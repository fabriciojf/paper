using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Paper.Services
{
  public interface IPaperSettings
  {
    Uri BaseUri { get; }
    PathString PathBase { get; }
    Uri[] RemotePaperServerUris { get; }
  }
}

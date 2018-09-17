using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Toolset;

namespace Paper.Core.Proxies
{
  class TimedProxyNotificationService : IHostedService, IDisposable
  {
    private static readonly TimeSpan interval = TimeSpan.FromSeconds(5);
    private static readonly ManualResetEventSlim stopEvent = new ManualResetEventSlim(false);

    private Thread thread;

    private readonly HttpClient httpClient;
    private readonly IPaperSettings settings;

    public TimedProxyNotificationService(IPaperSettings settings)
    {
      this.httpClient = new HttpClient();
      this.settings = settings;
    }

    private async void DoWork()
    {
      do
      {
        try
        {
          await NotifyPresence();
        }
        catch (Exception ex)
        {
          ex.Trace();
        }
        stopEvent.Wait(interval);
      } while (!stopEvent.IsSet);
    }

    private async Task NotifyPresence()
    {
      var baseUri = this.settings?.BaseUri;
      var pathBase = this.settings?.PathBase.ToString() ?? "/";
      var remotePaperServerUris = this.settings?.RemotePaperServerUris;

      if (baseUri == null || remotePaperServerUris == null)
      {
        return;
      }

      foreach (var uri in remotePaperServerUris)
      {
        try
        {
          var localUri = new Route(baseUri).Combine(pathBase).ToString();

          // exemplo: http://localhost:80/My/Path
          var url = new Route(uri);

          // exemplo: /My/Path
          var remotePath = url.MakeRelative("/");

          // exemplo: http://localhost:80/Api/1/Proxies?path=/My/Path&reverseUri=http://my.host/
          var remoteUri =
            url
              .Combine("/Api/1/Proxies")
              .SetArg("path", remotePath)
              .SetArg("reverseUri", localUri)
              .ToString();

          await httpClient.GetAsync(remoteUri);
        }
        catch (Exception ex)
        {
          ex.TraceWarning();
        }
      }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      thread = new Thread(DoWork);
      thread.Name = "Timed Proxy Notification Service";
      thread.Start();
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      stopEvent.Set();
      thread = null;
      return Task.CompletedTask;
    }

    public void Dispose()
    {
      stopEvent.Set();
      thread = null;

      httpClient?.Dispose();
    }
  }
}

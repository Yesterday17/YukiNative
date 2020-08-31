using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YukiNative.server {
  public delegate void RequestDelegate(HttpServer server, Request request, HttpListenerResponse response);

  public class HttpServer {
    private readonly HttpListener _listener;
    private readonly Dictionary<string, RequestDelegate> _routes = new Dictionary<string, RequestDelegate>();
    private bool _running = true;

    public HttpServer() {
      if (!HttpListener.IsSupported) {
        throw new Exception("Http listener is not supported!");
      }

      _listener = new HttpListener();
    }

    public void Stop() {
      _running = false;
    }

    public void Close() {
      _listener.Close();
    }

    public HttpServer AddRoute(string route, RequestDelegate @delegate) {
      _routes.Add(route, @delegate);
      return this;
    }

    public async Task Listen(string prefix) {
      _listener.Prefixes.Add(prefix);
      _listener.Start();

      while (_running) {
        var context = await _listener.GetContextAsync();

        var request = new Request(context.Request);
        var response = context.Response;
        if (!_routes.ContainsKey(request.Path)) {
          response.OutputStream.Close();
          continue;
        }

        try {
          _routes[request.Path].Invoke(this, request, response);
        }
        catch (Exception e) {
          var data = Encoding.UTF8.GetBytes(e.StackTrace);
          response.StatusCode = 400;
          await response.OutputStream.WriteAsync(data, 0, data.Length);
        }

        response.Close();
      }
    }
  }
}
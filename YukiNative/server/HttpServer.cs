using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using YukiNative.services;

namespace YukiNative.server {
  public delegate Task RequestDelegate(HttpServer server, Request request, Response response);

  public delegate void RequestDelegateSync(HttpServer server, Request request, Response response);

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

    public HttpServer AddRoute(string route, RequestDelegateSync @delegate) {
      return AddRoute(route,
        async (server, request, response) => { await Task.Run(() => { @delegate(server, request, response); }); });
    }

    public async Task Listen(string prefix) {
      _listener.Prefixes.Add(prefix);
      _listener.Start();

      while (_running) {
        var context = await _listener.GetContextAsync();

        var request = new Request(context.Request, context);
        var response = new Response(context.Response);
        if (!_routes.ContainsKey(request.Path)) {
          continue;
        }

#if DEBUG
        Console.WriteLine("{0} {1}", request.Method, request.Path);
        Console.WriteLine(request.Body);
#endif

        try {
          await _routes[request.Path].Invoke(this, request, response);
        }
        catch (Exception e) {
          response.StatusCode(400);
          await response.WriteText(e.StackTrace);
        }

        if (!request.IsWebsocket) {
          response.Close();
        }
      }
    }
  }
}
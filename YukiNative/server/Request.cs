using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace YukiNative.server {
  public class Request {
    private readonly HttpListenerRequest _request;
    private readonly HttpListenerContext _context;

    public Request(HttpListenerRequest request, HttpListenerContext context) {
      _request = request;
      _context = context;
      Body = new StreamReader(request.InputStream).ReadToEnd();
    }

    public string Method => _request.HttpMethod;

    public string Path => _request.Url.LocalPath;

    public string Body { get; }

    public bool IsWebsocket => _request.IsWebSocketRequest;

    public Task<HttpListenerWebSocketContext> AcceptWebSocketAsync(string sub = null) {
      return _context.AcceptWebSocketAsync(sub);
    }
  }
}
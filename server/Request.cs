using System.IO;
using System.Net;

namespace YukiNative.server {
  public class Request {
    private readonly HttpListenerRequest _request;

    public Request(HttpListenerRequest request) {
      _request = request;
      Body = new StreamReader(request.InputStream).ReadToEnd();
    }

    public string Method => _request.HttpMethod;

    public string Path => _request.Url.LocalPath;

    public string Body { get; }
  }
}
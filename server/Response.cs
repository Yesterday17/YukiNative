using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YukiNative.server {
  public class Response {
    private readonly HttpListenerResponse _response;

    public Response(HttpListenerResponse response) {
      _response = response;
    }

    public async Task WriteText(string text) {
      var data = Encoding.UTF8.GetBytes(text);
      await _response.OutputStream.WriteAsync(data, 0, data.Length);
    }

    public Response StatusCode(int code) {
      _response.StatusCode = code;
      return this;
    }

    public void Close() {
      _response.Close();
    }
  }
}
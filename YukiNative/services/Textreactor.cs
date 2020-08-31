using Textreactor;
using YukiNative.server;

namespace YukiNative.services {
  public class Textreactor {
    public static global::Textreactor.Textreactor Instance;

    public static void InitializeTextreactor(string cli) {
      Instance = new global::Textreactor.Textreactor(cli);
      Instance.OnTextreactorOutput += OnTextreactorOutput;
      Instance.Start();
    }

    public static void TextreactorService(HttpServer server, Request request, Response response) {
      var index = request.Body.IndexOf('|');

      var pid = request.Body;
      if (index > 0 && index != pid.Length - 1) {
        var code = pid.Substring(index + 1);
        pid = pid.Substring(0, index - 1);
        Instance.Hook(int.Parse(pid), code);
      }
      else {
        Instance.Attach(int.Parse(pid));
      }
    }

    private static void OnTextreactorOutput(TextOutputObject output) {
      WebsocketService.SendMessage(WebsocketService.PushTextreactorResult, output);
    }
  }
}
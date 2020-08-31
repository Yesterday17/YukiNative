using Textractor;
using YukiNative.server;

namespace YukiNative.services {
  public class Textractor {
    public static global::Textractor.Textractor Instance;

    public static void InitializeTextractor(string cli) {
      Instance = new global::Textractor.Textractor(cli);
      Instance.OnTextractorOutput += OnTextractorOutput;
      Instance.Start();
    }

    public static void TextractorService(HttpServer server, Request request, Response response) {
      var index = request.Body.IndexOf('|');

      var pid = request.Body;
      if (index > 0 && index != pid.Length - 1) {
        var code = pid.Substring(index + 1);
        pid = pid.Substring(0, index - 1);
        Instance.Hook(int.Parse(pid), code);
      }
      else if (index == pid.Length - 1) {
        Instance.Attach(int.Parse(pid.Substring(0, index - 1)));
      }
      else {
        Instance.Attach(int.Parse(pid));
      }
    }

    private static void OnTextractorOutput(TextOutputObject output) {
      WebsocketService.SendMessage(WebsocketService.PushTextractorResult, output);
    }
  }
}
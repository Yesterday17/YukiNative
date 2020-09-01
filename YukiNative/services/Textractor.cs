using Textractor;
using YukiNative.server;

namespace YukiNative.services {
  public static class Textractor {
    private static global::Textractor.Textractor _instance;

    public static void InitializeTextractor(string cli) {
      _instance = new global::Textractor.Textractor(cli);
      _instance.OnTextractorOutput += OnTextractorOutput;
      _instance.Start();
    }

    public static void TextractorService(HttpServer server, Request request, Response response) {
      var index = request.Body.IndexOf('|');

      var pid = request.Body;
      if (index > 0 && index != pid.Length - 1) {
        var code = pid.Substring(index + 1);
        pid = pid.Substring(0, index - 1);
        _instance.Hook(int.Parse(pid), code);
      }
      else if (index == pid.Length - 1) {
        _instance.Attach(int.Parse(pid.Substring(0, index - 1)));
      }
      else {
        _instance.Attach(int.Parse(pid));
      }
    }

    private static void OnTextractorOutput(TextOutputObject output) {
      WebsocketService.SendMessage(WebsocketService.PushTextractorResult, output);
    }
  }
}
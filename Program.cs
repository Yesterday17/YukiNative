using YukiNative.server;
using YukiNative.services;
using YukiNative.utils;

namespace YukiNative {
  enum WorkMode {
    Http,
    Websocket
  }

  internal static class Program {
    private static void Main(string[] args) {
      // Work mode
      var mode = WorkMode.Http;
      if (args.Length >= 1 && args[0].ToLower().Equals("websocket")) {
        mode = WorkMode.Websocket;
      }

      // Load directories
      if (args.Length >= 1) {
        var i = args[0].ToLower().Equals("websocket")
                || args[0].ToLower().Equals("http")
          ? 1
          : 0;
        for (; i < args.Length; i++) {
          Library.SetDllDirectory(args[i]);
        }
      }

      // HTTP Mode
      if (mode == WorkMode.Http) {
        var server = new HttpServer();
        server
          .AddRoute("/translate", JBeijing7.JBeijing7Service)
          .Listen("http://localhost:8080/")
          .GetAwaiter()
          .GetResult();
        server.Close();
      }
      else {
        // TODO: Websocket Mode
      }
    }
  }
}
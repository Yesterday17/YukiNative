using System;
using CommandLine;
using YukiNative.server;
using YukiNative.services;
using YukiNative.utils;

namespace YukiNative {
  internal static class Program {
    private static void Main(string[] args) {
      Parser.Default.ParseArguments<Config>(args)
        .WithParsed(options => {
          // Launch Textractor
          services.Textractor.InitializeTextractor(options.TextractorLocation);

          // Load directories
          foreach (var dir in options.DllDirectories) {
            Library.SetDllDirectory(dir);
          }

          var server = new HttpServer();
          var listen = server
            .AddRoute("/ws", WebsocketService.WebSocketService)
            .AddRoute("/library", Library.AddLibraryService)
            .AddRoute("/jbeijing7", JBeijing7.JBeijing7TranslateService)
            .AddRoute("/jbeijing7/dict", JBeijing7.JBeijing7OpenUserDictService)
            .AddRoute("/mecab", Mecab.MecabService)
            .AddRoute("/textractor", services.Textractor.TextractorService)
            .AddRoute("/ping", (_, __, response) => response.WriteText("pong"))
            .AddRoute("/shutdown", (httpServer, request, response) => {
              // Close response manually before server stops
              response.Close();
              // Stop server
              httpServer.Stop();
            })
            .Listen(options.ListenAddr);

          Console.WriteLine("YukiNative listening on {0}...", options.ListenAddr);
          listen.GetAwaiter().GetResult();
          server.Close();
        });
    }
  }
}
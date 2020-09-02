using System;
using System.Diagnostics;
using Textractor;
using YukiNative.server;

namespace YukiNative.services {
  public static class Textractor {
    private static global::Textractor.Textractor _instance;
    private static global::Textractor.Textractor _instance64;
    private static string _cli;

    public static void InitializeTextractor(string path) {
      _cli = path;

      Init86();
      if (Environment.Is64BitOperatingSystem) {
        Init64();
      }
    }

    private static void Init86() {
      _instance = new global::Textractor.Textractor(_cli) {
        OnTextractorOutput = OnTextractorOutput,
        OnExit = OnExit,
      };
      _instance.Start();
    }

    private static void Init64() {
      _instance64 = new global::Textractor.Textractor(_cli + @"x64\") {
        OnTextractorOutput = OnTextractorOutput,
        OnExit = OnExit64,
      };
      _instance64.Start();
    }

    public static void TextractorService(HttpServer server, Request request, Response response) {
      var index = request.Body.IndexOf('|');

      var body = request.Body;
      var pid = 0;
      var code = "";
      if (index > 0 && index != body.Length - 1) {
        code = body.Substring(index + 1);
        body = body.Substring(0, index - 1);
        pid = int.Parse(body);
      }
      else if (index == body.Length - 1) {
        pid = int.Parse(body.Substring(0, index));
      }
      else {
        pid = int.Parse(body);
      }

      var handle = Process.GetProcessById(pid).Handle;
      if (Win32.IsWin64Emulator(handle)) {
        // x86
        _instance.Inject(pid, code);
      }
      else {
        _instance64.Inject(pid, code);
      }
    }

    private static void OnTextractorOutput(TextOutputObject output) {
      WebsocketService.SendMessage(WebsocketService.PushTextractorResult, output);
    }

    private static void OnExit(object e, EventArgs args) {
      var p = (Process) e;
      Console.WriteLine("Textractor(x86) exited at {0} with code {1}.", p.ExitTime, p.ExitCode);
      Init86();
    }

    private static void OnExit64(object e, EventArgs args) {
      var p = (Process) e;
      Console.WriteLine("Textractor(x64) exited at {0} with code {1}.", p.ExitTime, p.ExitCode);
      Init64();
    }
  }
}
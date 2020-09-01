using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using YukiNative.server;

namespace YukiNative.services {
  class PushMessage<T> {
    public readonly string type;
    public readonly T message;

    public PushMessage(string type, T message) {
      this.type = type;
      this.message = message;
    }
  }

  public static class WebsocketService {
    private const int MaxMessageSize = 1024;
    private static readonly BufferBlock<string> ToPush = new BufferBlock<string>();
    private static WebSocketContext _context;

    private static async Task HandleWebSocket(WebSocketContext context) {
      _context = context;

      var ws = context.WebSocket;
      while (ws.State == WebSocketState.Open) {
        var data = await ToPush.ReceiveAsync();
        ws.SendAsync(
          new ArraySegment<byte>(Encoding.UTF8.GetBytes(data)),
          WebSocketMessageType.Text,
          true,
          CancellationToken.None
        ).Wait();
      }

      _context = null;
    }

    private static void Interrupt() {
      _context?.WebSocket.Abort();
      _context = null;
    }


    public const string PushTextractorResult = "textractor";
    public const string PushWindowEvent = "window";

    private static void PushMessage(string msg) {
      if (_context != null && _context.WebSocket.State == WebSocketState.Open) {
        ToPush.Post(msg);
      }
    }

    public static void SendMessage<T>(string type, T message) {
      PushMessage(JsonConvert.SerializeObject(new PushMessage<T>(type, message)));
    }

    public static async Task WebSocketService(HttpServer server, Request request, Response response) {
      if (request.IsWebsocket) {
        var wsContext = await request.AcceptWebSocketAsync();
        Interrupt();
        HandleWebSocket(wsContext);
      }
      else {
        response.StatusCode(400);
      }
    }
  }
}
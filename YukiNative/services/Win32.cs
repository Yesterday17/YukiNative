using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using YukiNative.server;

namespace YukiNative.services {
  public class Win32 {
    private class Win32Event<T> {
      public string Event { get; set; }
      public T Value { get; set; }
    }

    private static void WatchProcessExit(int pid) {
      var p = Process.GetProcessById(pid);
      p.EnableRaisingEvents = true;
      p.Exited += (sender, args) => {
        WebsocketService.SendMessage(WebsocketService.PushWin32Event, new Win32Event<int> {
          Event = "exit",
          Value = pid,
        }, false);
      };
    }

    public static void WatchProcessExitService(HttpServer server, Request request, Response response) {
      var pid = int.Parse(request.Body);
      WatchProcessExit(pid);
    }

    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

    public static bool IsWin64Emulator(IntPtr handle) {
      return IsWow64Process(handle, out var ret) && ret;
    }

    /////////////////////////////////////////////////////////////////////////

    public enum Events : uint {
      SystemForeground = 0x0003,
      SystemMinimizeEnd = 0x0017,
      SystemMinimizeStart = 0x0016,
      ObjectLocationChange = 0x800B,
    }

    private delegate void WinEventProc(IntPtr hWinEventHook, Events eventType, IntPtr hwnd, int idObject, int idChild,
      uint dwEventThread, uint dwmsEventTime);

    private static void HandleWinEvent(
      IntPtr eventHook, Events type,
      IntPtr hwnd, int idObject,
      int child, uint thread, uint time) {
      switch (type) {
        case Events.SystemMinimizeStart:
          WebsocketService.SendMessage(WebsocketService.PushWin32Event, new Win32Event<int> {
            Event = "minimize",
            Value = 0,
          });
          break;
        case Events.SystemMinimizeEnd:
          WebsocketService.SendMessage(WebsocketService.PushWin32Event, new Win32Event<int> {
            Event = "restore",
            Value = 0,
          });
          break;
        case Events.SystemForeground:
          break;
        case Events.ObjectLocationChange:
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
    }

    private static readonly WinEventProc WinEventCallback = HandleWinEvent;

    private static RequestDelegate NewEventService(Events e) {
      return async (server, request, response) => {
        var pid = uint.Parse(request.Body);
        var hook = await MessageLoop.AddHook(e, pid).Task;
        await response.WriteText(hook.ToString());
      };
    }

    public static readonly RequestDelegate EventMinimizeService = NewEventService(Events.SystemMinimizeStart);

    public static readonly RequestDelegate EventRestoreService = NewEventService(Events.SystemMinimizeEnd);

    /////////////////////////////////////////////////////////////////////////

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    public static void EventFocusService(HttpServer server, Request request, Response response) {
      var pid = int.Parse(request.Body);
      var p = Process.GetProcessById(pid);
      var hwnd = p.MainWindowHandle;
      SetForegroundWindow(hwnd);
    }

    /////////////////////////////////////////////////////////////////////////

    public static class MessageLoop {
      private static readonly Queue<Tuple<Events, uint, Action<int>>> Tasks =
        new Queue<Tuple<Events, uint, Action<int>>>();

      public static TaskCompletionSource<int> AddHook(Events @event, uint pid) {
        var promise = new TaskCompletionSource<int>();
        Tasks.Enqueue(new Tuple<Events, uint, Action<int>>(@event, pid, i => promise.TrySetResult(i)));
        return promise;
      }

      public static void Run() {
        while (true) {
          if (PeekMessage(out var msg, 0, 0, 0, 1)) {
            Console.WriteLine(msg);

            if (msg.Message == WmQuit)
              break;
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);
          }
          else if (Tasks.Count > 0) {
            while (Tasks.Count > 0) {
              var task = Tasks.Dequeue();
              var hook = SetWinEventHook(
                task.Item1,
                task.Item1,
                IntPtr.Zero,
                WinEventCallback,
                task.Item2,
                0,
                0);
              task.Item3(hook);
            }
          }

          Thread.Sleep(1);
        }
      }

      const uint WmQuit = 0x0012;

      [StructLayout(LayoutKind.Sequential)]
      private struct MSG {
        IntPtr Hwnd;
        public uint Message;
        IntPtr WParam;
        IntPtr LParam;
        uint Time;
        POINT Point;
      }

      [StructLayout(LayoutKind.Sequential)]
      private struct POINT {
        long x;
        long y;
      }

      [DllImport("user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
      private static extern bool GetMessage(ref MSG msg, int hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

      [DllImport("user32.dll")]
      private static extern bool PeekMessage(out MSG msg, int hWnd, uint wMsgFilterMin, uint wMsgFilterMax,
        uint wRemoveMsg);

      [DllImport("user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
      private static extern bool TranslateMessage(ref MSG msg);

      [DllImport("user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
      private static extern IntPtr DispatchMessage(ref MSG msg);

      [DllImport("user32.dll", SetLastError = true)]
      private static extern int SetWinEventHook(Events eventMin, Events eventMax, IntPtr hmodWinEventProc,
        WinEventProc lpfnWinEventProc, uint idProcess, uint idThread, uint dwflags);

      [DllImport("user32.dll", SetLastError = true)]
      private static extern int UnhookWinEvent(int hWinEventHook);
    }
  }
}
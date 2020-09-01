using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using YukiNative.server;

namespace YukiNative.services {
  public class Win32 {
    [Flags]
    private enum ProcessAccessFlags : uint {
      All = 0x001F0FFF,
      Terminate = 0x00000001,
      CreateThread = 0x00000002,
      VirtualMemoryOperation = 0x00000008,
      VirtualMemoryRead = 0x00000010,
      VirtualMemoryWrite = 0x00000020,
      DuplicateHandle = 0x00000040,
      CreateProcess = 0x000000080,
      SetQuota = 0x00000100,
      SetInformation = 0x00000200,
      QueryInformation = 0x00000400,
      QueryLimitedInformation = 0x00001000,
      Synchronize = 0x00100000
    }

    [DllImport("kernel32.dll")]
    private static extern int OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

    public static int OpenProcess(int pid) {
      return OpenProcess(ProcessAccessFlags.Synchronize, false, pid);
    }

    public class Win32Event<T> {
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

    /////////////////////////////////////////////////////////////////////////

    public enum Events {
      SystemForeground = 0x0003,
      SystemMinimizeEnd = 0x0017,
      SystemMinimizeStart = 0x0016,
      ObjectLocationChange = 0x800B,
    }

    private delegate void WinEventProc(int hWinEventHook, uint eventType, int hwnd, int idObject, int idChild,
      uint dwEventThread, uint dwmsEventTime);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWinEventHook(Events eventMin, Events eventMax, int hmodWinEventProc,
      WinEventProc lpfnWinEventProc, int idProcess, int idThread, int dwflags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int UnhookWinEvent(int hWinEventHook);

    public static int SubscribeWinEvent(Events @event, int pid, Action<int> callback) {
      var hook = SetWinEventHook(
        @event,
        @event,
        0,
        (eventHook, type, hwnd, idObject, child, thread, time) => { callback(hwnd); },
        pid,
        0,
        0);

      return hook;
    }

    public static int UnsubscribeWinEvent(int hook) {
      return UnhookWinEvent(hook);
    }
  }
}
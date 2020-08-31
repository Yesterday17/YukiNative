using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using YukiNative.server;

namespace YukiNative.services {
  [StructLayout(LayoutKind.Sequential)]
  internal static class JBeijing7 {
    [DllImport("JBJCT.dll", CharSet = CharSet.Unicode,
      CallingConvention = CallingConvention.Cdecl)]
    private static extern int JC_Transfer_Unicode(
      int hwnd, uint fromCordPage, uint toCodePage,
      int unknown, int unknown2,
      string from, StringBuilder to, ref int toCapacity,
      StringBuilder buffer, ref int bufferCapacity);

    private static string Translate(String text, uint toCodePage = 936) {
      var result = new StringBuilder(text.Length * 8);
      var buf = new StringBuilder(text.Length * 8);
      var toCapacity = result.Capacity;
      var bufferCapacity = buf.Capacity;
      JC_Transfer_Unicode(0, 932, toCodePage, 1, 1, text, result, ref toCapacity, buf, ref bufferCapacity);
      return result.ToString();
    }

    public static async void JBeijing7Service(HttpServer server, Request request, HttpListenerResponse response) {
      var data = Encoding.UTF8.GetBytes(Translate(request.Body));
      await response.OutputStream.WriteAsync(data, 0, data.Length);
    }
  }
}
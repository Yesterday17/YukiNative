using System.Runtime.InteropServices;
using YukiNative.server;

namespace YukiNative.utils {
  public static class Library {
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetDllDirectory(string lpPathName);

    public static void AddLibraryService(HttpServer server, Request request, Response response) {
      SetDllDirectory(request.Body);
    }
  }
}
using System.Runtime.InteropServices;

namespace YukiNative.utils {
  public class Library {
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetDllDirectory(string lpPathName);
  }
}
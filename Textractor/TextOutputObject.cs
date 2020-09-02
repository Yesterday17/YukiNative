// ReSharper disable MemberCanBePrivate.Global

namespace Textractor {
  public class TextOutputObject {
    public readonly ulong Handle;
    public readonly ulong Pid;
    public readonly ulong Addr;
    public readonly ulong Ctx;
    public readonly ulong Ctx2;
    public readonly string Name;
    public readonly string Code;
    public string Text { get; private set; }

    private TextOutputObject(
      ulong handle, ulong pid, ulong addr,
      ulong ctx, ulong ctx2,
      string name, string code, string text = ""
    ) {
      this.Handle = handle;
      this.Pid = pid;
      this.Addr = addr;
      this.Ctx = ctx;
      this.Ctx2 = ctx2;
      this.Name = name;
      this.Code = code;
      this.Text = text;
    }

    public static TextOutputObject Parse(string line) {
      var rb = line.IndexOf(']');
      var args = line.Substring(1, rb - 1).Split(':');
      var text = line.Substring(rb + 2);
      return new TextOutputObject(
        Hex2Num(args[0]), Hex2Num(args[1]), Hex2Num(args[2]),
        Hex2Num(args[3]), Hex2Num(args[4]),
        args[5], args[6], text
      );
    }

    public void AppendText(string append) {
      Text += "\n" + append;
    }

    private static ulong Hex2Num(string hex) {
      return ulong.Parse(hex, System.Globalization.NumberStyles.HexNumber);
    }

    public bool IsGameText() {
      return !Name.Equals("Console") && !Name.Equals("Clipboard");
    }
  }
}
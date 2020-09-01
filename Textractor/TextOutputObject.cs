namespace Textractor {
  public class TextOutputObject {
    public readonly ulong handle;
    public readonly ulong pid;
    public readonly ulong addr;
    public readonly ulong ctx;
    public readonly ulong ctx2;
    public readonly string name;
    public readonly string code;
    public string text { get; private set; }

    private TextOutputObject(
      ulong handle, ulong pid, ulong addr,
      ulong ctx, ulong ctx2,
      string name, string code, string text = ""
    ) {
      this.handle = handle;
      this.pid = pid;
      this.addr = addr;
      this.ctx = ctx;
      this.ctx2 = ctx2;
      this.name = name;
      this.code = code;
      this.text = text;
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
      text += "\n" + append;
    }

    private static ulong Hex2Num(string hex) {
      return ulong.Parse(hex, System.Globalization.NumberStyles.HexNumber);
    }
  }
}
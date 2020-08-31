namespace Textreactor {
  public class TextOutputObject {
    private ulong _handle;
    private ulong _pid;
    private ulong _addr;
    private ulong _ctx;
    private ulong _ctx2;
    private string _name;
    private string _code;
    public string Text;

    public TextOutputObject(
      ulong handle, ulong pid, ulong addr,
      ulong ctx, ulong ctx2,
      string name, string code, string text = ""
    ) {
      _handle = handle;
      _pid = pid;
      _addr = addr;
      _ctx = ctx;
      _ctx2 = ctx2;
      _name = name;
      _code = code;
      Text = text;
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
      Text += append + "\n";
    }

    private static ulong Hex2Num(string hex) {
      return ulong.Parse(hex, System.Globalization.NumberStyles.HexNumber);
    }
  }
}
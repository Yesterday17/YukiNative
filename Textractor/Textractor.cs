using System.Data;
using System.Diagnostics;
using System.Text;

namespace Textractor {
  public delegate void TextractorOutputDelegate(TextOutputObject output);

  public class Textractor {
    private readonly Process _cliProcess;
    private TextOutputObject _currentOutput;
    public TextractorOutputDelegate OnTextractorOutput;

    public Textractor(string cli) {
      var cliInfo = new ProcessStartInfo {
        FileName = cli,
        UseShellExecute = false,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = false,
        CreateNoWindow = true,
        StandardOutputEncoding = Encoding.Unicode,
      };

      _cliProcess = new Process {
        StartInfo = cliInfo,
      };
    }

    public void Start() {
      _cliProcess.Start();
      _cliProcess.OutputDataReceived += OnProcessOutput;
      _cliProcess.BeginOutputReadLine();
    }

    public void Hook(int pid, string code) {
      if (!code.Contains("/H") && !code.Contains("/R")) {
        throw new SyntaxErrorException("invalid code");
      }

      Execute($"{code} -P{pid}");
    }

    public void Execute(string cmd) {
      _cliProcess.StandardInput.WriteLine(cmd);
    }

    public void Attach(int pid) {
      Execute($"attach -P{pid}");
    }

    public void Detach(int pid) {
      Execute($"detach -P{pid}");
    }

    public void Stop() {
      _cliProcess.Kill();
    }

    private void OnProcessOutput(object sender, DataReceivedEventArgs e) {
      if (string.IsNullOrEmpty(e.Data)) {
        return;
      }

      OnData(e.Data);
    }

    private void OnData(string line) {
      if (line.StartsWith("Usage")) {
        return;
      }

      if (line.StartsWith("[")) {
        _currentOutput = TextOutputObject.Parse(line);
      }
      else {
        _currentOutput.AppendText(line);
      }

      OnTextractorOutput?.Invoke(_currentOutput);
    }
  }
}
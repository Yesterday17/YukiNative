using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Textractor {
  public delegate void TextractorOutputDelegate(TextOutputObject output);

  public class Textractor {
    private readonly Process _cliProcess;
    private TextOutputObject _currentOutput;

    public TextractorOutputDelegate OnTextractorOutput;
    public EventHandler OnExit;

    public Textractor(string cli = @".\Textractor\") {
      var cliInfo = new ProcessStartInfo {
        FileName = Path.GetFullPath(cli + "TextractorCLI.exe"),
        UseShellExecute = false,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = false,
        CreateNoWindow = true,
        StandardOutputEncoding = Encoding.Unicode,
      };

      _cliProcess = new Process {
        StartInfo = cliInfo,
        EnableRaisingEvents = true,
      };
      _cliProcess.Exited += (sender, args) => { OnExit(sender, args); };
    }

    public void Start() {
      _cliProcess.Start();
      _cliProcess.OutputDataReceived += OnProcessOutput;
      _cliProcess.BeginOutputReadLine();
    }

    public void Inject(int pid, string code) {
      if (string.IsNullOrEmpty(code)) {
        Attach(pid);
      }
      else {
        Hook(pid, code);
      }
    }

    private void Hook(int pid, string code) {
      if (!code.Contains("/H") && !code.Contains("/R")) {
        throw new SyntaxErrorException("invalid code");
      }

      Execute($"{code} -P{pid}");
    }

    private void Attach(int pid) {
      Execute($"attach -P{pid}");
    }

    private void Execute(string cmd) {
      var s = new StreamWriter(_cliProcess.StandardInput.BaseStream, new UnicodeEncoding(false, false));
      s.WriteLine(cmd);
      s.Flush();
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

      Debug.WriteLine(_currentOutput.Text);
      if (_currentOutput.IsGameText()) {
        OnTextractorOutput(_currentOutput);
      }
    }
  }
}
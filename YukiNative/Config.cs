using System.Collections.Generic;
using CommandLine;

namespace YukiNative {
  public class Config {
    [Option('l', "listen", HelpText = "Local address to listen.", Default = "http://localhost:8080/")]
    public string ListenAddr { get; set; }

    [Option('t', "textreactor", Required = true, HelpText = "Location of TextreactorCLI.exe.")]
    public string TextreactorLocation { get; set; }

    [Option('d', "directory", HelpText = "Directories to load dlls.")]
    public IEnumerable<string> DllDirectories { get; set; }
  }
}
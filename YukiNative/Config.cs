using System.Collections.Generic;
using CommandLine;

namespace YukiNative {
  public class Config {
    [Option('l', "listen", HelpText = "Local address to listen.", Default = "http://localhost:8080/")]
    public string ListenAddr { get; set; }

    [Option('t', "textractor", Required = true, HelpText = "Location of TextractorCLI.exe.")]
    public string TextractorLocation { get; set; }

    [Option('d', "directory", HelpText = "Directories to load dlls.")]
    public IEnumerable<string> DllDirectories { get; set; }
  }
}
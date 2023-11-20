using System;
using System.Collections.Generic;

namespace Common_Lib;

public class CommandLineArgs : IDisposable
{
    public CommandLineArgs(string executable = " ")
    {
        var cliArgs = Environment.GetCommandLineArgs();
        if (cliArgs.Length < 1) return;

        if (!cliArgs[0].ToLower().Contains(executable.ToLower())) return;
        for (var i = 1; i < cliArgs.Length; i++)
        {
            var split = cliArgs[i].Split("=");
            if (split.Length < 2) continue;
            Args.Add(split[0], split[1]);
        }

        Success = true;
    }
    public  bool                       Success { get; set; }
    private Dictionary<string, string> Args    { get; } = new();
    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }
    public string Get(string key)
    {
        try
        {
            return string.IsNullOrEmpty(Args[key]) ? Args[key] : string.Empty;
        }
        catch (Exception e)
        {
            return $"{key} not found {e.Message}";
        }
    }
    public int GetLogLevel()
    {
        try
        {
            var checkLl = string.IsNullOrEmpty(Args["ll"]) ? Args["key"] : string.Empty;
            return string.IsNullOrEmpty(checkLl) ? 9 : int.Parse(checkLl);
        }
        catch (Exception)
        {
            return 999999;
        }
    }
}

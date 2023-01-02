using System;
using System.Collections.Generic;

namespace Common_Lib;

public class CommandLineArgs : IDisposable
{
    public CommandLineArgs(string executable = " ")
    {
        var cliArgs = Environment.GetCommandLineArgs();
        if (cliArgs.Length <= 1) return;

        if (!cliArgs[0].ToLower().Contains(executable.ToLower())) return;
        for (var i = 1; i < cliArgs.Length; i++)
        {
            var split = cliArgs[i].Split("=");
            if (split.Length < 2) continue;
            Args.Add(split[0], split[1]);
        }

        Success = true;
    }

    public Dictionary<string, string> Args { get; set; } = new();
    public bool Success { get; set; }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public string Get(string key)
    {
        try
        {
            return Args[key];
        }
        catch (Exception e)
        {
            return $"{key} not found {e.Message}";
        }
    }
}
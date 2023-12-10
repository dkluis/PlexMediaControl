using System;
using System.IO;

namespace Common_Lib;

public class AppInfo
{
    private readonly string   _drive;
    private readonly string   _fullPath;
    private readonly string[] _mediaExtensions;

    public AppInfo(string application, string program, int logLevel = 999, string dbConnection = "DbProduction")
    {
        _drive = Common.EnvInfo.Drive;
        var homeDir = Common.EnvInfo.Os == "Windows" ? Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") : Environment.GetEnvironmentVariable("HOME");

        if (homeDir is not null)
        {
            homeDir = Path.Combine(homeDir, application);
        } else
        {
            Console.WriteLine("Could not determine HomeDir");
            Environment.Exit(666);
        }

        var configFileName = application + ".cnf";
        ConfigPath = homeDir;
        var configFullPath = Path.Combine(homeDir, configFileName);

        if (!File.Exists(configFullPath))
        {
            Console.WriteLine($"Log File Does not Exist {configFullPath}");
            Environment.Exit(666);
        }

        LogLevel = logLevel == 999 ? int.Parse(Common.FindInArray(configFullPath, "LogLevel")) : logLevel;

        var fileName = program + ".log";
        var filePath = Path.Combine(homeDir, "Logs");
        _fullPath        = Path.Combine(filePath, fileName);
        TxtFile          = new TextFileHandler(fileName, program, filePath, LogLevel);
        TvMazeToken      = Common.FindInArray(configFullPath, "TvmazeToken");
        RarbgToken       = Common.FindInArray(configFullPath, "RarbgToken");
        _mediaExtensions = Common.FindInArray(configFullPath, "MediaExtensions").Split(", ");

        ActiveDbConn = dbConnection switch
                       {
                           "DbProduction" => Common.FindInArray(configFullPath, "DbProduction"), "DbTesting" => Common.FindInArray(configFullPath, "DbTesting"), "DbAlternate" => Common.FindInArray(configFullPath, "DbAlternate"), _ => "",
                       };
    }

    public string?         ConfigPath   { get; }
    public string          RarbgToken   { get; }
    public string          TvMazeToken  { get; }
    public TextFileHandler TxtFile      { get; }
    public int             LogLevel     { get; set; }
    public string          ActiveDbConn { get; }
}

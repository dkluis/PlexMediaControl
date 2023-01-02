using System;
using System.IO;

namespace Common_Lib;

public class AppInfo
{
    //public readonly string DbConnection;
    public readonly string DbProdConn;
    public readonly string DbTestConn;
    public readonly string Drive;
    public readonly string FileName;
    public readonly string FilePath;
    public readonly string FullPath;
    public readonly string? HomeDir;

    public readonly string[] MediaExtensions;
    public readonly string Program;
    public readonly string RarbgToken;

    public readonly string TvmazeToken;
    public readonly TextFileHandler TxtFile;

    public AppInfo(string application, string program, string dbConnection = "")
    {
        Application = application;
        Program = program;

        Common.EnvInfo envInfo = new();
        Drive = envInfo.Drive;
        HomeDir = envInfo.Os == "Windows"
            ? Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")
            : Environment.GetEnvironmentVariable("HOME");

        if (HomeDir is not null)
        {
            HomeDir = Path.Combine(HomeDir, Application);
        }
        else
        {
            Console.WriteLine("Could not determine HomeDir");
            Environment.Exit(666);
        }

        ConfigFileName = Application + ".cnf";
        ConfigPath = HomeDir;
        ConfigFullPath = Path.Combine(HomeDir, ConfigFileName);
        if (!File.Exists(ConfigFullPath))
        {
            Console.WriteLine($"Log File Does not Exist {ConfigFullPath}");
            Environment.Exit(666);
        }

        LogLevel = int.Parse(Common.FindInArray(ConfigFullPath, "LogLevel"));

        FileName = Program + ".log";
        FilePath = Path.Combine(HomeDir, "Logs");
        FullPath = Path.Combine(FilePath, FileName);

        TxtFile = new TextFileHandler(FileName, Program, FilePath, LogLevel);

        DbProdConn = Common.FindInArray(ConfigFullPath, "DbProduction");
        DbTestConn = Common.FindInArray(ConfigFullPath, "DbTesting");
        DbAltConn = Common.FindInArray(ConfigFullPath, "DbAlternate");

        TvmazeToken = Common.FindInArray(ConfigFullPath, "TvmazeToken");
        RarbgToken = Common.FindInArray(ConfigFullPath, "RarbgToken");

        MediaExtensions = Common.FindInArray(ConfigFullPath, "MediaExtensions").Split(", ");

        ActiveDbConn = dbConnection switch
        {
            "DbProduction" => DbProdConn,
            "DbTesting" => DbTestConn,
            "DbAlternate" => DbAltConn,
            _ => ""
        };
    }

    public int LogLevel { get; set; }

    public string ActiveDbConn { get; }
    private string Application { get; }
    private string ConfigFileName { get; }
    private string ConfigFullPath { get; }
    public string? ConfigPath { get; }
    private string DbAltConn { get; }
}

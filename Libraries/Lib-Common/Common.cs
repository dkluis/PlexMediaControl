using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Common_Lib;

public static class Common
{
    public static string RemoveSpecialCharsInShowName(string showName)
    {
        showName = showName.Replace("...", "")
            .Replace("..", "")
            .Replace(".", " ")
            .Replace(",", "")
            .Replace("'", "")
            .Replace("   ", " ")
            .Replace("  ", " ")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("/", "")
            .Replace(":", "")
            .Replace("?", "")
            .Replace("|", "")
            .Replace("&#039;", "")
            .Replace("&amp;", "and")
            .Replace("&", "and")
            .Replace("°", "")
            .Trim()
            .ToLower();
        // Was put in for the What If...? situation: showName = showName.Substring(0, showName.Length);
        if (showName.Length <= 7) return showName;
        if (showName.ToLower()[..7] == "what if")
            showName = "What If";
        return showName;
    }

    public static string RemoveSuffixFromShowName(string showName)
    {
        var wrappedYear = Regex.Split(showName, "[(]2[0-2][0-3][0-9][)]", RegexOptions.IgnoreCase);
        if (wrappedYear.Length == 2) return wrappedYear[0];

        var plainYear = Regex.Split(showName, "2[0-2][0-3][0-9]", RegexOptions.IgnoreCase);
        if (plainYear.Length == 2) return plainYear[0];

        var wrappedCountry = Regex.Split(showName, "[(][a-z][a-z][)]", RegexOptions.IgnoreCase);
        return wrappedCountry.Length == 2 ? wrappedCountry[0] : showName;
    }

    public static string BuildSeasonEpisodeString(int seasNum, int epiNum)
    {
        return "s" + seasNum.ToString().PadLeft(2, '0') + "e" + epiNum.ToString().PadLeft(2, '0');
    }

    public static string BuildSeasonOnly(int seasNum)
    {
        return "s" + seasNum.ToString().PadLeft(2, '0');
    }

    public static string ConvertEpochToDate(int epoch)
    {
        DateTime datetime = new(1970, 1, 1, 0, 0, 0);
        datetime = datetime.AddSeconds(epoch);
        var date = datetime.ToString("yyyy-MM-dd");
        return date;
    }

    public static int ConvertDateToEpoch(string date)
    {
        var ts = ConvertDateToDateTime(date) - new DateTime(1970, 1, 1, 0, 0, 0);
        var epoch = Convert.ToInt32(ts.TotalSeconds);
        return epoch;
    }

    public static DateTime ConvertDateToDateTime(string date)
    {
        var dItems = date.Split("-");
        DateTime datetime = new();
        if (dItems.Length != 3) return datetime;
        datetime = new DateTime(int.Parse(dItems[0]), int.Parse(dItems[1]), int.Parse(dItems[2]), 0, 0, 0);
        return datetime;
    }

    public static string AddDaysToDate(string date, int days)
    {
        var calculatedDt = ConvertDateToDateTime(date);
        calculatedDt = calculatedDt.AddDays(days);
        date = calculatedDt.ToString("yyyy-MM-DD");
        return date;
    }

    public static string SubtractDaysFromDate(string date, int days)
    {
        var calculatedDt = ConvertDateToDateTime(date);
        calculatedDt = calculatedDt.AddDays(-days);
        date = calculatedDt.ToString("yyyy-MM-DD");
        return date;
    }

    public static string FindInArray(string fullPath, string find)
    {
        if (!File.Exists(fullPath)) return "";
        var fileText = File.ReadAllText(fullPath);
        var keyValuePair = ConvertStringToJArray(fileText);
        foreach (var rec in keyValuePair)
        {
            if (rec[find] is null) return "";
            if (rec[find]!.ToString() != "") return rec[find]!.ToString();
        }

        return "";
    }

    public static string FindInObject(string fullPath, string find)
    {
        if (!File.Exists(fullPath)) return "";
        var fileText = File.ReadAllText(fullPath);
        var keyValuePair = ConvertStringToJObject(fileText);
        foreach (var rec in keyValuePair)
            if (rec.Key == find)
                return rec.Value!.ToString();
        return "";
    }

    public static JArray ConvertStringToJArray(string message)
    {
        if (message == "")
        {
            JArray empty = new();
            return empty;
        }

        var jA = JArray.Parse(message);
        return jA;
    }

    public static JObject ConvertStringToJObject(string message)
    {
        if (message == "")
        {
            JObject empty = new();
            return empty;
        }

        var jO = JObject.Parse(message);
        return jO;
    }

    public class EnvInfo
    {
        public readonly string Drive;
        public readonly string MachineName;
        public readonly string Os;
        public readonly string UserName;
        public readonly string? WorkingDrive;
        public readonly string WorkingPath;

        public EnvInfo()
        {
            var os = Environment.OSVersion;
            var pid = os.Platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    Os = "Windows";
                    Drive = @"C:\";
                    break;
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    Os = "Linux";
                    Drive = @"/";
                    break;
                default:
                    Os = "Unknown";
                    Drive = "Unknown";
                    break;
            }

            MachineName = Environment.MachineName;
            WorkingPath = Environment.CurrentDirectory;
            WorkingDrive = Path.GetPathRoot(WorkingPath);
            UserName = Environment.UserName;
        }
    }
}

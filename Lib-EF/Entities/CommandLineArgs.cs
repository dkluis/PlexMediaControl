namespace PlexMediaControl.Entities;

public class CommandLineArgs
{
    public static Dictionary<string, string> PrintArgs(string executable)
    {
        var args = Environment.GetCommandLineArgs();
        var resultingArgs = new Dictionary<string, string>();
        if (args.Length <= 1) return resultingArgs;

        if (!args[0].ToLower().Contains(executable.ToLower())) return resultingArgs;
        for (var i = 1; i < args.Length; i++)
        {
            var split = args[i].Split("=");
            if (split.Length < 2) continue;
            resultingArgs.Add(split[0], split[1]);
        }

        return resultingArgs;
    }
}

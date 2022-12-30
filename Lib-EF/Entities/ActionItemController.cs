using Common_Lib;
using PlexMediaControl.Models.MariaDB;

namespace PlexMediaControl.Entities;

public class ActionItemController : ActionItem
{
    public bool Fill(string program, string message)
    {
        Program = program;
        Message = message;
        return Validate();
    }

    public bool Add(TextFileHandler log)
    {
        if (!Validate()) return false;
        try
        {
            //ToDo write the Add Method
        }
        catch (Exception e)
        {
            log.Write($"Error Occured {e.Message} {e.InnerException}", "ActionItem Add", 1);
            return false;
        }

        return true;
    }

    private bool Validate()
    {
        var result = true;
        if (string.IsNullOrEmpty(Message))
            result = false;
        else if (string.IsNullOrEmpty(Program)) result = false;

        return result;
    }
}

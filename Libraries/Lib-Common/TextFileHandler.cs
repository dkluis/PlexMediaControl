﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Common_Lib;

public class TextFileHandler
{
    private readonly string    _app;
    private readonly string    _fullFilePath;
    private readonly int       _level;
    private readonly Stopwatch _timer = new();

    public TextFileHandler(string filename, string application, string inFilePath, int loglevel)
    {
        _timer.Start();
        _app          = application;
        _level        = loglevel;
        _fullFilePath = Path.Combine(inFilePath, filename);
        if (!File.Exists(_fullFilePath)) File.Create(_fullFilePath).Close();
    }

    public void Start()
    {
        EmptyLine();
        Write($"{_app} Started  ########################################################################################", _app, 0);
    }

    public void Elapsed()
    {
        Write($"{_app} Elapsed up to now: {_timer.ElapsedMilliseconds} mSec", "Elapsed Time", 0);
    }

    public void Stop()
    {
        _timer.Stop();
        Write($"{_app} Finished ####################################  in {_timer.ElapsedMilliseconds} mSec  ####################################", _app, 0);
    }

    public void Empty()
    {
        using StreamWriter file = new(_fullFilePath, false);
    }

    public void Write(string message, string function = "", int loglevel = 3, bool append = true)
    {
        if (string.IsNullOrEmpty(function)) function = _app;
        if (function.Length > 19) function           = function[..19];

        if (loglevel > _level) return;
        using StreamWriter file = new(_fullFilePath, append);
        file.WriteLine($"{DateTime.Now}: {function,-20}: {loglevel,-2} --> {message}");
    }

    public void Write(string[] messages, string function = "", int loglevel = 3, bool append = true)
    {
        if (string.IsNullOrEmpty(function)) function = _app;
        if (function.Length > 19) function           = function[..19];

        if (loglevel > _level) return;
        using StreamWriter file = new(_fullFilePath, append);
        foreach (var msg in messages) file.WriteLine($"{DateTime.Now}: {function,-20}: {loglevel,-2}--> {msg}");
    }

    public void EmptyLine(int lines = 1)
    {
        var                idx  = 1;
        using StreamWriter file = new(_fullFilePath, true);

        while (idx <= lines)
        {
            file.WriteLine("");
            idx++;
        }
    }

    public void WriteText(string message, bool newline = true, bool append = true)
    {
        using StreamWriter file = new(_fullFilePath, append);

        if (newline)
            file.WriteLine(message);
        else
            file.Write(message);
    }

    public void WriteText(string[] messages, bool newline = true, bool append = true)
    {
        using StreamWriter file = new(_fullFilePath, append);

        foreach (var msg in messages)
            if (newline)
                file.WriteLine(msg);
            else
                file.Write(msg);
    }

    public List<string> ReturnLogContent()
    {
        var content     = new List<string>();
        var fileContent = File.ReadAllLines(_fullFilePath);
        foreach (var line in fileContent) content.Add(line);

        content.Reverse();

        return content;
    }
}

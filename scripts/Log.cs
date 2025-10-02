using Godot;
using System;
using System.Diagnostics;

public static class Log
{
    private static bool _useRichTextInEditor = true;

    private static (string className, string methodName, int lineNumber) GetCallerInfo()
    {
        var stackTrace = new StackTrace(true);
        for (int i = 1; i < stackTrace.FrameCount; i++)
        {
            var frame = stackTrace.GetFrame(i);
            var method = frame.GetMethod();
            var declaringType = method.DeclaringType;

            if (declaringType == null)
                continue;

            if (declaringType != typeof(Log))
            {
                string className = declaringType.Name;
                string methodName = method.Name;
                int lineNumber = frame.GetFileLineNumber();
                return (className, methodName, lineNumber);
            }
        }

        return ("UnknownClass", "UnknownMethod", 0);
    }


    private static void Print(string message, string godotColor, string ansiColor)
    {
        var (className, methodName, lineNumber) = GetCallerInfo();
        string prefix = "[Game Log]";
        string logMessage = $"{prefix}[{className}.{methodName} @line {lineNumber}] - {message}";

        if (Engine.IsEditorHint() && _useRichTextInEditor)
        {
            GD.PrintRich($"[color={godotColor}]{logMessage}[/color]");
        }
        else
        {
            Console.WriteLine($"\x1b[{ansiColor}m{logMessage}\x1b[0m");
        }
    }

    [Conditional("DEBUG")]
    public static void Info(string message) => Print(message, "lightgreen", "32");
    [Conditional("DEBUG")]
    public static void Warn(string message) => Print(message, "yellow", "33");
    [Conditional("DEBUG")]
    public static void Error(string message) => Print(message, "red", "31");
    [Conditional("DEBUG")]
    public static void Debug(string message) => Print(message, "gray", "90");
}

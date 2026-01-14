using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

public static class LogUtil
{
//formatting
    private static readonly BindingFlags nFlags = BindingFlags.Public | BindingFlags.Instance;

    private static HashSet<object> visited = new();
    private static string ProcessFormattingAClass(int lvl, object obj)
    {
        if(lvl == 0) return obj == null ? "null" : "ref";
        System.Type type = obj.GetType();
        if(visited.Contains(obj)) return $"(Circular Reference: {type.Name})"; //Prevents recursion

        visited.Add(obj);
        string result = "{" + string.Join(", ", type.GetFields(nFlags).Select(f => $"\"{f.Name}\": {Format(lvl - 1, f.GetValue(obj))}")) + "}";
        visited.Remove(obj); // Clean up after formatting
        return result;
    }

    private static string Format(int lvl, object obj) => obj switch
    {
        null => "null",
        string s => s,
        IList list => "[" + string.Join(", ", list.Cast<object>().Select(obj => Format(lvl, obj))) + "]",
        _ when obj.GetType().IsClass => ProcessFormattingAClass(lvl, obj),
        _ => obj.ToString()
    };
//joins
    public static string Join(params object[] args) => string.Join(" ", args.Select(a => a?.ToString() ?? "null"));
    public static string JoinSep(params object[] args) => string.Join(", ", args);
    public static string JoinFormatted(int level, params object[] args) => string.Join(";\n", args.Select(obj => Format(level, obj))) + ";";
//logs
    public static void Log(params object[] args) => Debug.Log(Join(args));
    public static void LogComa(params object[] args) => Debug.Log(JoinSep(args));
    public static void LogForm(int level, params object[] args) => Debug.Log(JoinFormatted(level, args));
    public static void LogRows(params object[] args) => args.ToList().ForEach(arg => Debug.Log(args));
    public static void Warn(params object[] args) => Debug.LogWarning(Join(args));
    public static void Error(params object[] args) => Debug.LogError(Join(args));
//write file
    public static void WriteFirstTime(string path) => Debug.Log($"Writing {Path.GetFileName(path)} for the first time");
//fail to save file
    public static void FailToSave(string msg, string stackTrace = "") => Warn($"Unable to save data due to: {msg} {stackTrace}");
    public static void FailToSave(Exception e) => FailToSave(e.Message, e.StackTrace);
//fail to load file
    public static void FailToLoad(string msg, string stackTrace = "") => Warn($"Failed to load data due to: {msg} {stackTrace}");
    public static void FailToLoad(Exception e) => FailToLoad(e.Message, e.StackTrace);
    public static void FailToLoadNonexistent(string path) => Log($"Cannot load file at {path}. File does not exist. Expecting write...");

//exeptions
    public static T Exeption<T>(params object[] args)
    {
        Debug.LogException(new Exception(Join(args)));
        return default;
    }
    public static List<T> ExeptionList<T>(params object[] args)
    {
        Debug.LogException(new Exception(Join(args)));
        return new();
    }
}
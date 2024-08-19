using System;
using Cysharp.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class YOLogger : MonoBehaviour
{
    public static bool IsLogsEnabled = true;

    [Conditional ("YO_LOG_ENABLE")]
    public static void Log(string mess)
    {
        if (!IsLogsEnabled)
            return;

        UnityEngine.Debug.Log(mess);
    }

    [Conditional ("YO_LOG_ENABLE")]
    public static void Log(string mess, Color color)
    {
        if (!IsLogsEnabled)
            return;

        string colorString = ColorUtility.ToHtmlStringRGB(color);
        // string colorMess = $"<color=#{colorString}>{mess}</color>";
        string colorMess = ZString.Format("<color=#{0}>{1}</color>", colorString, mess);
        UnityEngine.Debug.Log(colorMess);
    }
    
    [Conditional("YO_LOG_ENABLE")]
    public static void LogTemporaryChannel(string channelName, string mess)
    {
        if (!IsLogsEnabled)
            return;

        string finalMess = ZString.Format("#{0}# {1}", channelName, mess);
        UnityEngine.Debug.Log(finalMess);
    }

    [Conditional("YO_LOG_ENABLE")]
    public static void LogTemporaryChannel(string channelName, string mess, Color color)
    {
        if (!IsLogsEnabled)
            return;

        string colorString = ColorUtility.ToHtmlStringRGB(color);
        string colorMess = ZString.Format("<color=#{0}>{1}</color>", colorString, mess);
        string finalMess = ZString.Format("#{0}# {1}", channelName, colorMess);
        UnityEngine.Debug.Log(finalMess);
    }

    [Conditional("YO_LOG_ENABLE")]
    public static void Warning(string mess)
    {
        UnityEngine.Debug.LogWarning(mess);
    }

    [Conditional("YO_LOG_ENABLE")]
    public static void Error(string mess)
    {
        UnityEngine.Debug.LogError(mess);
    }

    public static void Exception(Exception exception)
    {
        Debug.LogException(exception);
    }
}

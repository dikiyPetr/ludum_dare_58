using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consoleText;
    private bool isVisible = true;
    private Queue<string> lastLogs = new Queue<string>();
    private const int MaxLines = 3;

    void Awake()
    {
     
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (consoleText != null)
        {
            string color = GetColorForLogType(type);
            string prefix = GetPrefixForLogType(type);
            string formattedLog = $"<color={color}>[{prefix}] {logString}</color>";

            lastLogs.Enqueue(formattedLog);

            if (lastLogs.Count > MaxLines)
            {
                lastLogs.Dequeue();
            }

            consoleText.text = string.Join("\n", lastLogs);
        }
    }

    void ToggleConsole()
    {
        isVisible = !isVisible;
        if (consoleText != null)
        {
            consoleText.gameObject.SetActive(isVisible);
        }
    }

    string GetColorForLogType(LogType type)
    {
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                return "#FF4444";
            case LogType.Warning:
                return "#FFAA00";
            case LogType.Log:
                return "#FFFFFF";
            default:
                return "#CCCCCC";
        }
    }

    string GetPrefixForLogType(LogType type)
    {
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                return "ERROR";
            case LogType.Warning:
                return "WARN";
            case LogType.Log:
                return "LOG";
            default:
                return "INFO";
        }
    }
}
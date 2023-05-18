using System.Text;
using UnityEngine;
using TMPro;

namespace Mmi
{
public class DebugManager : MonoBehaviour
{
    // Editor Fields
    public TextMeshProUGUI textDebug;

    

    // Variables
    private static int maxScreenString = 16 * 1024;
    private StringBuilder logStringBuilder = new StringBuilder();

    private void OnEnable()
    {
        SetEvents();
    }

    private void OnDisable()
    {
        ClearEvents();
    }
    
    private void SetEvents()
    {
        Application.logMessageReceived += OnLogMessage;
    }
    private void ClearEvents()
    {
        Application.logMessageReceived -= OnLogMessage;
    }

    private void OnLogMessage(string pCondition, string pStackTrace, LogType pLogType)
    {
        logStringBuilder.Append(pCondition + "\n");
        if (logStringBuilder.Length > maxScreenString)
        {
            logStringBuilder.Clear();
        }
        textDebug.text = logStringBuilder.ToString();
    }
}
}
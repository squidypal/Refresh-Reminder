using UnityEngine;
using UnityEditor;

[System.Serializable]
public class RefreshReminderSettings
{
    private const string PREFS_PREFIX = "RefreshReminder_";

    // Timing Settings
    public bool enableTimeReminder = true;
    public float timeThresholdMinutes = 15f;

    // Code Change Settings
    public bool enableChangeReminder = true;
    public int changeThreshold = 5;

    // Visual Settings
    public Color backgroundColor = new Color(0.15f, 0.15f, 0.18f, 0.98f);
    public Color borderColor = new Color(0.3f, 0.5f, 0.8f, 0.5f);
    public Color titleColor = new Color(0.9f, 0.9f, 0.9f);
    public Color subtitleColor = new Color(0.6f, 0.6f, 0.6f);
    public Vector2 windowSize = new Vector2(280, 100);
    public float windowYPosition = 120f;

    // Behavior Settings
    public bool playSound = false;
    public bool autoFocusUnity = false;

    private static RefreshReminderSettings _instance;
    public static RefreshReminderSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RefreshReminderSettings();
                _instance.Load();
            }
            return _instance;
        }
    }

    public void Save()
    {
        EditorPrefs.SetBool(PREFS_PREFIX + "enableTimeReminder", enableTimeReminder);
        EditorPrefs.SetFloat(PREFS_PREFIX + "timeThresholdMinutes", timeThresholdMinutes);
        EditorPrefs.SetBool(PREFS_PREFIX + "enableChangeReminder", enableChangeReminder);
        EditorPrefs.SetInt(PREFS_PREFIX + "changeThreshold", changeThreshold);

        EditorPrefs.SetString(PREFS_PREFIX + "backgroundColor", ColorUtility.ToHtmlStringRGBA(backgroundColor));
        EditorPrefs.SetString(PREFS_PREFIX + "borderColor", ColorUtility.ToHtmlStringRGBA(borderColor));
        EditorPrefs.SetString(PREFS_PREFIX + "titleColor", ColorUtility.ToHtmlStringRGBA(titleColor));
        EditorPrefs.SetString(PREFS_PREFIX + "subtitleColor", ColorUtility.ToHtmlStringRGBA(subtitleColor));

        EditorPrefs.SetFloat(PREFS_PREFIX + "windowSizeX", windowSize.x);
        EditorPrefs.SetFloat(PREFS_PREFIX + "windowSizeY", windowSize.y);
        EditorPrefs.SetFloat(PREFS_PREFIX + "windowYPosition", windowYPosition);

        EditorPrefs.SetBool(PREFS_PREFIX + "playSound", playSound);
        EditorPrefs.SetBool(PREFS_PREFIX + "autoFocusUnity", autoFocusUnity);
    }

    public void Load()
    {
        enableTimeReminder = EditorPrefs.GetBool(PREFS_PREFIX + "enableTimeReminder", true);
        timeThresholdMinutes = EditorPrefs.GetFloat(PREFS_PREFIX + "timeThresholdMinutes", 15f);
        enableChangeReminder = EditorPrefs.GetBool(PREFS_PREFIX + "enableChangeReminder", true);
        changeThreshold = EditorPrefs.GetInt(PREFS_PREFIX + "changeThreshold", 5);

        if (EditorPrefs.HasKey(PREFS_PREFIX + "backgroundColor"))
            ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(PREFS_PREFIX + "backgroundColor"), out backgroundColor);
        if (EditorPrefs.HasKey(PREFS_PREFIX + "borderColor"))
            ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(PREFS_PREFIX + "borderColor"), out borderColor);
        if (EditorPrefs.HasKey(PREFS_PREFIX + "titleColor"))
            ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(PREFS_PREFIX + "titleColor"), out titleColor);
        if (EditorPrefs.HasKey(PREFS_PREFIX + "subtitleColor"))
            ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(PREFS_PREFIX + "subtitleColor"), out subtitleColor);

        windowSize.x = EditorPrefs.GetFloat(PREFS_PREFIX + "windowSizeX", 280f);
        windowSize.y = EditorPrefs.GetFloat(PREFS_PREFIX + "windowSizeY", 100f);
        windowYPosition = EditorPrefs.GetFloat(PREFS_PREFIX + "windowYPosition", 120f);

        playSound = EditorPrefs.GetBool(PREFS_PREFIX + "playSound", false);
        autoFocusUnity = EditorPrefs.GetBool(PREFS_PREFIX + "autoFocusUnity", false);
    }

    public void ResetToDefaults()
    {
        enableTimeReminder = true;
        timeThresholdMinutes = 15f;
        enableChangeReminder = true;
        changeThreshold = 5;

        backgroundColor = new Color(0.15f, 0.15f, 0.18f, 0.98f);
        borderColor = new Color(0.3f, 0.5f, 0.8f, 0.5f);
        titleColor = new Color(0.9f, 0.9f, 0.9f);
        subtitleColor = new Color(0.6f, 0.6f, 0.6f);
        windowSize = new Vector2(280, 100);
        windowYPosition = 120f;

        playSound = false;
        autoFocusUnity = false;

        Save();
    }
}

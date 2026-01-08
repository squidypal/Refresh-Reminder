using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[InitializeOnLoad]
public static class RefreshReminder
{
    private const string LAST_REFRESH_KEY = "RefreshReminder_LastRefreshTime";
    private const string LAST_CHANGE_COUNT_KEY = "RefreshReminder_LastChangeCount";
    private const string DISMISSED_KEY = "RefreshReminder_Dismissed";

    private static RefreshReminderSettings Settings => RefreshReminderSettings.Instance;

    private static DateTime lastRefreshTime;
    private static int lastChangeCount;
    private static bool isDismissed;
    private static FileSystemWatcher watcher;
    private static int currentChangeCount;

    static RefreshReminder()
    {
        LoadState();
        SetupFileWatcher();
        EditorApplication.update += OnEditorUpdate;
        AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReload;
    }

    private static void LoadState()
    {
        string timeStr = EditorPrefs.GetString(LAST_REFRESH_KEY, "");
        if (DateTime.TryParse(timeStr, out DateTime savedTime))
            lastRefreshTime = savedTime;
        else
            lastRefreshTime = DateTime.Now;

        lastChangeCount = EditorPrefs.GetInt(LAST_CHANGE_COUNT_KEY, 0);
        isDismissed = EditorPrefs.GetBool(DISMISSED_KEY, false);
        currentChangeCount = lastChangeCount;
    }

    private static void SaveState()
    {
        EditorPrefs.SetString(LAST_REFRESH_KEY, lastRefreshTime.ToString());
        EditorPrefs.SetInt(LAST_CHANGE_COUNT_KEY, currentChangeCount);
        EditorPrefs.SetBool(DISMISSED_KEY, isDismissed);
    }

    private static void SetupFileWatcher()
    {
        string scriptsPath = Path.Combine(Application.dataPath, "Scripts");
        if (!Directory.Exists(scriptsPath))
            scriptsPath = Application.dataPath;

        try
        {
            watcher = new FileSystemWatcher(scriptsPath);
            watcher.Filter = "*.cs";
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Changed += OnFileChanged;
            watcher.Created += OnFileChanged;
            watcher.EnableRaisingEvents = true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"RefreshReminder: Could not setup file watcher: {e.Message}");
        }
    }

    private static void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        currentChangeCount++;
        SaveState();
    }

    private static void OnAssemblyReload()
    {
        lastRefreshTime = DateTime.Now;
        currentChangeCount = 0;
        isDismissed = false;
        SaveState();
    }

    private static void OnEditorUpdate()
    {
        if (isDismissed) return;
        if (EditorApplication.isCompiling) return;

        bool shouldRemind = false;
        string reason = "";

        double minutesSinceRefresh = (DateTime.Now - lastRefreshTime).TotalMinutes;
        int changesSinceRefresh = currentChangeCount - lastChangeCount;

        if (Settings.enableTimeReminder && minutesSinceRefresh >= Settings.timeThresholdMinutes)
        {
            shouldRemind = true;
            reason = $"{(int)minutesSinceRefresh} minutes since last refresh";
        }
        else if (Settings.enableChangeReminder && changesSinceRefresh >= Settings.changeThreshold)
        {
            shouldRemind = true;
            reason = $"{changesSinceRefresh} code changes detected";
        }

        if (shouldRemind)
        {
            RefreshReminderWindow.ShowNotification(reason);
            isDismissed = true;
            SaveState();
        }
    }

    public static void PerformRefresh()
    {
        AssetDatabase.Refresh();
        lastRefreshTime = DateTime.Now;
        lastChangeCount = currentChangeCount;
        isDismissed = false;
        SaveState();
    }

    public static void Dismiss()
    {
        isDismissed = true;
        SaveState();
    }
}

public class RefreshReminderWindow : EditorWindow
{
    private static RefreshReminderWindow instance;
    private static string currentReason;
    private static float fadeAlpha = 0f;

    private static RefreshReminderSettings Settings => RefreshReminderSettings.Instance;

    private GUIStyle backgroundStyle;
    private GUIStyle titleStyle;
    private GUIStyle reasonStyle;
    private GUIStyle buttonStyle;
    private GUIStyle dismissStyle;
    private Texture2D bgTexture;

    public static void ShowNotification(string reason)
    {
        if (instance != null)
        {
            instance.Close();
        }

        currentReason = reason;
        fadeAlpha = 0f;

        instance = CreateInstance<RefreshReminderWindow>();

        Vector2 size = Settings.windowSize;
        float yPos = Settings.windowYPosition;
        Vector2 screenCenter = new Vector2(Screen.currentResolution.width / 2f, yPos);

        instance.ShowPopup();
        instance.position = new Rect(screenCenter.x - size.x / 2, screenCenter.y, size.x, size.y);
        instance.minSize = size;
        instance.maxSize = size;

        if (Settings.playSound)
        {
            EditorApplication.Beep();
        }

        if (Settings.autoFocusUnity)
        {
            EditorUtility.FocusProjectWindow();
        }
    }

    private void OnEnable()
    {
        EditorApplication.update += OnUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnUpdate;
        if (bgTexture != null)
        {
            DestroyImmediate(bgTexture);
        }
        instance = null;
    }

    private void OnUpdate()
    {
        fadeAlpha = Mathf.MoveTowards(fadeAlpha, 1f, 0.05f);
        Repaint();
    }

    private void InitStyles()
    {
        if (bgTexture != null)
        {
            DestroyImmediate(bgTexture);
        }

        backgroundStyle = new GUIStyle();
        
        bgTexture = new Texture2D(1, 1);
        bgTexture.SetPixel(0, 0, Settings.backgroundColor);
        bgTexture.Apply();
        backgroundStyle.normal.background = bgTexture;

        titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Settings.titleColor }
        };

        reasonStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 10,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Settings.subtitleColor },
            wordWrap = true
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            fixedHeight = 26
        };

        dismissStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 10,
            fixedHeight = 22
        };
    }

    private void OnGUI()
    {
        InitStyles();

        Color prevColor = GUI.color;
        GUI.color = new Color(1, 1, 1, fadeAlpha);

        GUI.Box(new Rect(0, 0, position.width, position.height), "", backgroundStyle);

        Color borderCol = Settings.borderColor;
        borderCol.a *= fadeAlpha;
        Handles.color = borderCol;
        
        Handles.DrawSolidRectangleWithOutline(
            new Rect(0, 0, position.width, position.height),
            Color.clear,
            Settings.borderColor
        );

        GUILayout.BeginVertical();
        GUILayout.Space(12);

        GUILayout.Label("Refresh Recommended", titleStyle);
        GUILayout.Space(2);
        GUILayout.Label(currentReason, reasonStyle);

        GUILayout.Space(8);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Refresh Now", buttonStyle, GUILayout.Width(120)))
        {
            RefreshReminder.PerformRefresh();
            Close();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Dismiss", dismissStyle, GUILayout.Width(100)))
        {
            RefreshReminder.Dismiss();
            Close();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUI.color = prevColor;
    }
}

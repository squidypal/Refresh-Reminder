using UnityEngine;
using UnityEditor;

public class RefreshReminderSettingsWindow : EditorWindow
{
    private RefreshReminderSettings settings;
    private Vector2 scrollPosition;
    private bool showTimingSettings = true;
    private bool showVisualSettings = true;
    private bool showBehaviorSettings = true;

    private GUIStyle headerStyle;
    private GUIStyle boxStyle;

    [MenuItem("Tools/Refresh Reminder/Settings")]
    public static void ShowWindow()
    {
        var window = GetWindow<RefreshReminderSettingsWindow>("Refresh Reminder");
        window.minSize = new Vector2(320, 400);
    }

    [MenuItem("Tools/Refresh Reminder/Test Notification")]
    public static void TestNotification()
    {
        RefreshReminderWindow.ShowNotification("This is a test notification");
    }

    private void OnEnable()
    {
        settings = RefreshReminderSettings.Instance;
    }

    private void InitStyles()
    {
        if (headerStyle != null) return;

        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12
        };

        boxStyle = new GUIStyle("box")
        {
            padding = new RectOffset(10, 10, 10, 10),
            margin = new RectOffset(0, 0, 5, 5)
        };
    }

    private void OnGUI()
    {
        InitStyles();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.Space(10);

        // Header
        EditorGUILayout.LabelField("Refresh Reminder Settings", new GUIStyle(EditorStyles.boldLabel) { fontSize = 16 });
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Configure when and how reminders appear.", EditorStyles.miniLabel);

        EditorGUILayout.Space(15);

        // Timing Settings
        showTimingSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showTimingSettings, "Timing Settings");
        if (showTimingSettings)
        {
            EditorGUILayout.BeginVertical(boxStyle);

            settings.enableTimeReminder = EditorGUILayout.Toggle(
                new GUIContent("Enable Time Reminder", "Show reminder after a set amount of time"),
                settings.enableTimeReminder
            );

            EditorGUI.BeginDisabledGroup(!settings.enableTimeReminder);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time Threshold", GUILayout.Width(120));
            settings.timeThresholdMinutes = EditorGUILayout.Slider(settings.timeThresholdMinutes, 1f, 60f);
            EditorGUILayout.LabelField("min", GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(10);

            settings.enableChangeReminder = EditorGUILayout.Toggle(
                new GUIContent("Enable Change Reminder", "Show reminder after code files change"),
                settings.enableChangeReminder
            );

            EditorGUI.BeginDisabledGroup(!settings.enableChangeReminder);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Change Threshold", GUILayout.Width(120));
            settings.changeThreshold = EditorGUILayout.IntSlider(settings.changeThreshold, 1, 50);
            EditorGUILayout.LabelField("files", GUILayout.Width(35));
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(5);

        // Visual Settings
        showVisualSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showVisualSettings, "Visual Settings");
        if (showVisualSettings)
        {
            EditorGUILayout.BeginVertical(boxStyle);

            settings.backgroundColor = EditorGUILayout.ColorField(
                new GUIContent("Background Color", "Notification background color"),
                settings.backgroundColor
            );

            settings.borderColor = EditorGUILayout.ColorField(
                new GUIContent("Border Color", "Notification border accent color"),
                settings.borderColor
            );

            settings.titleColor = EditorGUILayout.ColorField(
                new GUIContent("Title Color", "Main title text color"),
                settings.titleColor
            );

            settings.subtitleColor = EditorGUILayout.ColorField(
                new GUIContent("Subtitle Color", "Reason/subtitle text color"),
                settings.subtitleColor
            );

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Window Size", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Width", GUILayout.Width(50));
            settings.windowSize.x = EditorGUILayout.Slider(settings.windowSize.x, 200f, 500f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Height", GUILayout.Width(50));
            settings.windowSize.y = EditorGUILayout.Slider(settings.windowSize.y, 80f, 200f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Y Position", GUILayout.Width(70));
            settings.windowYPosition = EditorGUILayout.Slider(settings.windowYPosition, 50f, 500f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(5);

        // Behavior Settings
        showBehaviorSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showBehaviorSettings, "Behavior Settings");
        if (showBehaviorSettings)
        {
            EditorGUILayout.BeginVertical(boxStyle);

            settings.playSound = EditorGUILayout.Toggle(
                new GUIContent("Play Sound", "Play a sound when notification appears"),
                settings.playSound
            );

            settings.autoFocusUnity = EditorGUILayout.Toggle(
                new GUIContent("Auto-Focus Unity", "Bring Unity to front when notification appears"),
                settings.autoFocusUnity
            );

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(20);

        // Action Buttons
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Test Notification", GUILayout.Height(30)))
        {
            settings.Save();
            RefreshReminderWindow.ShowNotification("This is a test notification");
        }

        if (GUILayout.Button("Save Settings", GUILayout.Height(30)))
        {
            settings.Save();
            ShowNotification(new GUIContent("Settings saved!"));
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset to Defaults", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Reset Settings",
                "Are you sure you want to reset all settings to defaults?",
                "Reset", "Cancel"))
            {
                settings.ResetToDefaults();
                ShowNotification(new GUIContent("Settings reset!"));
            }
        }

        if (GUILayout.Button("Refresh Now", GUILayout.Height(25)))
        {
            RefreshReminder.PerformRefresh();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            settings.Save();
        }
    }
}

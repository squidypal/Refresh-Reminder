# Refresh Reminder

<img width="210" height="80" alt="image" src="https://github.com/user-attachments/assets/735dfcca-1efc-4683-b208-d6d331b96fd6" /> <img width="278" height="99" alt="image" src="https://github.com/user-attachments/assets/90495a02-2909-4cef-9cad-6c160610e108" />


A Unity Editor addon that reminds you to refresh your project after time has passed or code changes are detected.

## Why?

If you're like me and prefer to work in unity with auto refresh OFF
You could maybe sometimes forget to refresh the domain (which I have done personally) 

## Features

- Time-based reminders (configurable threshold)
- Code change detection via file watcher
- Customizable notification appearance
- Optional sound and auto-focus

## Usage

Access settings via **Tools > Refresh Reminder > Settings**

### Settings

| Setting | Description |
|---------|-------------|
| Time Threshold | Minutes before time-based reminder (1-60) |
| Change Threshold | File changes before reminder (1-50) |
| Colors | Background, border, title, subtitle |
| Window Size | Notification dimensions |
| Play Sound | Beep on notification |
| Auto-Focus | Bring Unity to front |

## Files

- `RefreshReminder.cs` - Core logic
- `RefreshReminderSettings.cs` - Settings data
- `RefreshReminderSettingsWindow.cs` - Settings GUI

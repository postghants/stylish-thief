using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneNotes.Editor
{
    internal static class SceneNotesSettingsProvider
    {
        private const string SettingsPath = "Project/Scene Notes";

        private const float LabelWidth = 250;
        private static SceneNotesSettings.NoteIconStyle _lastIconStyle;

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            SettingsProvider provider = new(SettingsPath, SettingsScope.Project)
            {
                activateHandler = OnActivate,
                keywords = new HashSet<string>
                {
                    "Scene", "Notes", "Title", "Colour", "Icon", "Gizmo", "Folder"
                }
            };

            _lastIconStyle = SceneNotesSettings.noteIconStyle;

            return provider;
        }

        private static void OnActivate(string searchContext, VisualElement rootElement)
        {
            VisualElement container = new()
            {
                style =
                {
                    marginTop = 2,
                    marginLeft = 9
                }
            };

            Label title = new("Scene Notes")
            {
                style =
                {
                    fontSize = 18,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = 12
                }
            };
            container.Add(title);

            // Project
            container.Add(CreateCategoryHeader(SceneNotesSettings.ProjectCategory));
            container.Add(CreateTextField(SceneNotesSettings.unityFolder,
                SceneNotesSettings.NotesFolderName, SceneNotesSettings.NotesFolderDesc));
            container.Add(CreateToggle(SceneNotesSettings.saveScreenshotsAsSubassets,
                SceneNotesSettings.SaveScreenshotsAsSubassetsName, SceneNotesSettings.SaveScreenshotsAsSubassetsDesc));

            // Visualisation Preferences
            container.Add(CreateCategoryHeader(SceneNotesSettings.VisualisationCategory));
            container.Add(CreateToggle(SceneNotesSettings.displayTitleInScene,
                SceneNotesSettings.DisplayTitleInSceneName, SceneNotesSettings.DisplayTitleInSceneDesc));
            container.Add(CreateIntField(SceneNotesSettings.maxTitleLength,
                SceneNotesSettings.MaxTitleLengthName, SceneNotesSettings.MaxTitleLengthDesc));
            container.Add(CreateColorField(SceneNotesSettings.titleColour,
                SceneNotesSettings.TitleColourName, SceneNotesSettings.TitleColourDesc));
            container.Add(CreateColorField(SceneNotesSettings.titleBackgroundColour,
                SceneNotesSettings.TitleBackgroundColourName, SceneNotesSettings.TitleBackgroundColourDesc));
            container.Add(CreateEnumField(SceneNotesSettings.noteIconStyle,
                SceneNotesSettings.NoteIconStyleName, SceneNotesSettings.NoteIconStyleDesc));

            rootElement.Add(container);
        }

        private static Label CreateCategoryHeader(string text)
        {
            return new Label(text)
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginTop = 8,
                    marginBottom = 4
                }
            };
        }

        private static Toggle CreateToggle(UserPref<bool> setting, string label, string tooltip)
        {
            Toggle toggle = new(label)
            {
                value = setting.value,
                tooltip = tooltip
            };
            toggle.labelElement.style.minWidth = LabelWidth;

            toggle.RegisterValueChangedCallback(evt =>
            {
                setting.SetValue(evt.newValue, true);
                OnSettingsSaved();
            });

            return toggle;
        }

        private static Toggle CreateToggle(PackageSetting<bool> setting, string label, string tooltip)
        {
            Toggle toggle = new(label)
            {
                value = setting.value,
                tooltip = tooltip
            };
            toggle.labelElement.style.minWidth = LabelWidth;

            toggle.RegisterValueChangedCallback(evt =>
            {
                setting.SetValue(evt.newValue, true);
                OnSettingsSaved();
            });

            return toggle;
        }

        private static IntegerField CreateIntField(UserPref<int> setting, string label, string tooltip)
        {
            IntegerField field = new(label)
            {
                value = setting.value,
                tooltip = tooltip
            };
            field.labelElement.style.minWidth = LabelWidth;

            field.RegisterValueChangedCallback(evt =>
            {
                setting.SetValue(Mathf.Clamp(evt.newValue, 5, 200), true);
                OnSettingsSaved();
            });

            return field;
        }

        private static ColorField CreateColorField(UserPref<Color> setting, string label, string tooltip)
        {
            ColorField field = new(label)
            {
                value = setting.value,
                tooltip = tooltip
            };
            field.labelElement.style.minWidth = LabelWidth;

            field.RegisterValueChangedCallback(evt =>
            {
                setting.SetValue(evt.newValue, true);
                OnSettingsSaved();
            });

            return field;
        }

        private static EnumField CreateEnumField(UserPref<SceneNotesSettings.NoteIconStyle> setting, string label,
            string tooltip)
        {
            EnumField field = new(label, setting.value)
            {
                tooltip = tooltip
            };
            field.labelElement.style.minWidth = LabelWidth;

            field.RegisterValueChangedCallback(evt =>
            {
                setting.SetValue((SceneNotesSettings.NoteIconStyle)evt.newValue, true);
                OnSettingsSaved();
            });

            return field;
        }

        private static TextField CreateTextField(PackageSetting<string> setting, string label, string tooltip)
        {
            TextField field = new(label)
            {
                value = setting.value,
                tooltip = tooltip
            };
            field.labelElement.style.minWidth = LabelWidth;

            field.RegisterValueChangedCallback(evt =>
            {
                setting.SetValue(evt.newValue, true);
                OnSettingsSaved();
            });

            return field;
        }

        private static void OnSettingsSaved()
        {
            SaveSettingsRuntimeSide();

            SceneNotesSettings.NoteIconStyle iconStyle = SceneNotesSettings.noteIconStyle;

            if (iconStyle != _lastIconStyle)
            {
                NotesDatabase.instance.RefreshAllNotesIcons();
                SceneNotesSettings.NoteIconStyleChanged?.Invoke(iconStyle);
                _lastIconStyle = iconStyle;
            }
        }

        [InitializeOnLoadMethod]
        private static void SaveSettingsRuntimeSide()
        {
            SceneNoteBehaviour.DisplayTitleInScene = SceneNotesSettings.displayTitleInScene;
            SceneNoteBehaviour.TitleColour = SceneNotesSettings.titleColour;
            SceneNoteBehaviour.TitleBackgroundColour = SceneNotesSettings.titleBackgroundColour;
            SceneNoteBehaviour.MaxTitleLength = SceneNotesSettings.maxTitleLength;
        }
    }
}
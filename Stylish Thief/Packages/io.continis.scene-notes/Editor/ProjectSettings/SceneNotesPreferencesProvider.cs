using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneNotes.Editor
{
    internal static class SceneNotesPreferencesProvider
    {
        private const string PreferencesPath = "Preferences/Scene Notes";

        private const float LabelWidth = 250;

        [SettingsProvider]
        private static SettingsProvider CreatePreferencesProvider()
        {
            SettingsProvider provider = new(PreferencesPath, SettingsScope.User)
            {
                activateHandler = OnActivate,
                keywords = new HashSet<string> { "Scene", "Notes", "Author", "Gizmo", "Screenshot" }
            };

            return provider;
        }

        private static void OnActivate(string searchContext, VisualElement rootElement)
        {
            VisualElement prefsContainer = new()
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
            prefsContainer.Add(title);

            prefsContainer.Add(CreateTextField(SceneNotesSettings.authorName,
                SceneNotesSettings.AuthorNameName,
                SceneNotesSettings.AuthorNameDesc));

            prefsContainer.Add(CreateToggle(SceneNotesSettings.includeGizmosInScreenshots,
                SceneNotesSettings.IncludeGizmosInScreenshotsName,
                SceneNotesSettings.IncludeGizmosInScreenshotsDesc));

            prefsContainer.Add(CreateEnumField(SceneNotesSettings.createNoteDefaultAction,
                SceneNotesSettings.CreateNoteDefaultActionName,
                SceneNotesSettings.CreateNoteDefaultActionDesc));

            rootElement.Add(prefsContainer);
        }

        private static TextField CreateTextField(UserPref<string> setting, string label, string tooltip)
        {
            TextField field = new(label)
            {
                value = setting.value,
                tooltip = tooltip
            };
            field.labelElement.style.minWidth = LabelWidth;

            field.RegisterValueChangedCallback(evt => { setting.SetValue(evt.newValue, true); });

            return field;
        }

        private static Toggle CreateToggle(UserPref<bool> setting, string label, string tooltip)
        {
            Toggle toggle = new(label)
            {
                value = setting.value,
                tooltip = tooltip
            };
            toggle.labelElement.style.minWidth = LabelWidth;

            toggle.RegisterValueChangedCallback(evt => { setting.SetValue(evt.newValue, true); });

            return toggle;
        }

        private static EnumField CreateEnumField(UserPref<SceneNotesSettings.CreateNoteAction> setting, string label, string tooltip)
        {
            EnumField field = new(label, setting.value)
            {
                tooltip = tooltip
            };
            field.labelElement.style.minWidth = LabelWidth;

            field.RegisterValueChangedCallback(evt =>
            {
                setting.SetValue((SceneNotesSettings.CreateNoteAction)evt.newValue, true);
            });

            return field;
        }
    }
}
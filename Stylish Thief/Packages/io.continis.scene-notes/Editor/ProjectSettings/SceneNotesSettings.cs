using System;
using UnityEditor;
using UnityEngine;

namespace SceneNotes.Editor
{
    public class SceneNotesSettings
    {
        public enum NoteIconStyle
        {
            [Tooltip("The note's icon display its state: Not Started, In Progress, Done.")]
            State,

            [Tooltip(
                "The note's icon displays its category. Assign a category icon in the NoteCategories ScriptableObject.")]
            Category
        }

        public enum CreateNoteAction
        {
            [Tooltip("Creates a new note at the Scene View's pivot point, with no link to any GameObject.")]
            IndividualNote,

            [Tooltip("Creates a new note linked to the currently selected GameObject. Falls back to Individual Note if no single GameObject is selected.")]
            LinkedToSelected
        }

        // Categories
        internal const string VisualisationCategory = "Visualisation Preferences";
        internal const string ProjectCategory = "Project";

        // Setting Names
        internal const string DisplayTitleInSceneName = "Display Title In Scene";
        internal const string MaxTitleLengthName = "Maximum Title Length";
        internal const string TitleColourName = "Title Colour";
        internal const string TitleBackgroundColourName = "Title Background Colour";
        internal const string NoteIconStyleName = "Icon Displays";
        internal const string NotesFolderName = "Notes Folder";
        internal const string AuthorNameName = "Author Name";
        internal const string IncludeGizmosInScreenshotsName = "Include Gizmos in Screenshots";
        internal const string CreateNoteDefaultActionName = "Default Create Note Action";
        internal const string SaveScreenshotsAsSubassetsName = "Save Screenshots as Sub-assets";

        // Descriptions
        internal const string DisplayTitleInSceneDesc =
            "Shows the title of each note in Scene View, under the note's icon.";

        internal const string MaxTitleLengthDesc =
            "If titles are displayed in the Scene View, only displays up to this length.";

        internal const string TitleColourDesc =
            "The colour to use for the note's title. Only used if the title is displayed.";

        internal const string TitleBackgroundColourDesc =
            "The colour to use for the note's title background. Only used if the title displayed.";

        internal const string NoteIconStyleDesc =
            "Whether note icons in the Scene View display the note's state, or its category.";

        internal const string NotesFolderDesc =
            "This is the folder from which Scene Notes loads data, and in which it saves new notes.";

        internal const string AuthorNameDesc =
            "This is the name used to sign notes and comments.";

        internal const string IncludeGizmosInScreenshotsDesc =
            "When enabled, gizmos will be visible in screenshots taken from the Scene View.";

        internal const string CreateNoteDefaultActionDesc =
            "The default action when clicking the Create Note button in the toolbar. 'Linked to Selected' creates a note that is right away linked to the selected GameObject. 'Linked to Selected' falls back to 'Independent Note' if no single GameObject is selected.";

        internal const string SaveScreenshotsAsSubassetsDesc =
            "When enabled, new screenshots are stored as sub-assets inside the note's .asset file. " +
            "They are guaranteed to be deleted with the note and are hidden from the Texture2D object picker, " +
            "but the .asset stores them uncompressed, so its file size and git diffs grow accordingly. " +
            "When disabled, screenshots are saved as standalone PNG files in " +
            "<unityFolder>/Screenshots/<sceneGuid>/<noteGuid>/, preserving PNG compression on disk.";

        internal static Action<NoteIconStyle> NoteIconStyleChanged;

        public static PackageSetting<bool> WelcomeWindowSeen = new("general.welcomeWindowSeen", false);

        // Preferences (per-user, shown in Preferences window)
        public static UserPref<string> authorName = new("preferences.authorName", CloudProjectSettings.userName);
        public static UserPref<bool> includeGizmosInScreenshots = new("preferences.includeGizmosInScreenshots", false);
        public static UserPref<CreateNoteAction> createNoteDefaultAction = new("preferences.createNoteDefaultAction", CreateNoteAction.IndividualNote);

        // Visualisation (per-user, shown in Project Settings)
        public static UserPref<bool> displayTitleInScene = new("preferences.displayTitleInScene", true);
        public static UserPref<int> maxTitleLength = new("preferences.maxTitleLength", 30);
        public static UserPref<Color> titleColour = new("preferences.titleColour", Color.white); 

        public static UserPref<Color> titleBackgroundColour =
            new("preferences.titleBackgroundColour", new Color(0f, 0f, 0f, .1f));

        public static UserPref<NoteIconStyle> noteIconStyle = new("preferences.noteIconStyle", NoteIconStyle.State);

        // Project settings (shared)
        public static PackageSetting<string> unityFolder = new("general.unityFolder", "SceneNotes");
        public static PackageSetting<bool> saveScreenshotsAsSubassets = new("project.saveScreenshotsAsSubassets", true);
    }
}
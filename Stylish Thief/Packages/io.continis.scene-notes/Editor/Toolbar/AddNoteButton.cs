using System.IO;
using SceneNotes.Editor;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneNotes.Toolbar
{
    [EditorToolbarElement(ID, typeof(SceneView))]
    public class AddNoteButton : EditorToolbarButton
    {
        public const string ID = "SceneNotes.AddNoteButton";

        public AddNoteButton()
        {
            name = ID;
            string imagesPath = Path.Combine(Constants.packageAssetsFolder, Constants.uiImagesFolder, Constants.toolbarIconsFolder);
            EditorApplication.delayCall += () => icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(imagesPath, $"{Constants.imagePrefix}Plus.png"));
            tooltip = "Adds a new scene note.";
            clicked += Clicked;
        }

        private void Clicked()
        {
            if (SceneNotesSettings.createNoteDefaultAction == SceneNotesSettings.CreateNoteAction.LinkedToSelected)
                CreateNoteLinkedToSelected();
            else
                CreateNoteInScene();
        }

        public static void CreateNoteInScene()
        {
            Scene s = SceneManager.GetActiveScene();
            if (!string.IsNullOrEmpty(s.path))
            {
                GUID guid = AssetDatabase.GUIDFromAssetPath(s.path);
                NotesDatabase.instance.CreateNewNote(guid.ToString());
            }
            else
            {
                Debug.LogWarning($"{Constants.packagePrefix} {Constants.creatingNoteInUnsavedSceneError}");
            }
        }

        public static void CreateNoteLinkedToSelected()
        {
            // Fall back to CreateNoteInScene if selection is not exactly one GameObject
            if (Selection.gameObjects.Length != 1)
            {
                CreateNoteInScene();
                return;
            }

            GameObject selected = Selection.activeGameObject;
            Scene s = SceneManager.GetActiveScene();
            if (!string.IsNullOrEmpty(s.path))
            {
                GUID guid = AssetDatabase.GUIDFromAssetPath(s.path);
                NotesDatabase.instance.CreateNewNoteLinkedTo(guid.ToString(), selected);
            }
            else
            {
                Debug.LogWarning($"{Constants.packagePrefix} {Constants.creatingNoteInUnsavedSceneError}");
            }
        }
    }
}

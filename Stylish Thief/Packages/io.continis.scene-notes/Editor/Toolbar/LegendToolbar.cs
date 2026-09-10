using System;
using System.Collections.Generic;
using System.IO;
using SceneNotes.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace SceneNotes.Toolbar
{
    [Overlay(typeof(SceneView), "Scene Notes Categories",
        defaultDockPosition = DockPosition.Bottom, defaultDockZone = DockZone.RightColumn, defaultLayout = Layout.HorizontalToolbar)]
    [Icon(Constants.packageAssetsFolder + "/" + Constants.uiImagesFolder + "/" + Constants.toolbarIconsFolder + "/CategoryOff.png")]
    public class LegendToolbar : Overlay, ICreateHorizontalToolbar, ICreateVerticalToolbar
    {
        private VisualElement _contentContainer;
        private readonly Dictionary<string, VisualElement> _rowsByCategoryName = new();
        private Action _filtersChangedHandler;
        
        public override void OnCreated()
        {
            Constants.imagePrefix = EditorGUIUtility.isProSkin ? "d_" : "";
            NotesDatabase.instance.CategoriesUpdated += () => CreateContents(_contentContainer);
            
            base.OnCreated();
        }

        private void CreateContents(VisualElement visualElement)
        {
            _contentContainer = visualElement;

            if (_contentContainer == null) return;

            _contentContainer.Clear();
            _rowsByCategoryName.Clear();

            if (_filtersChangedHandler != null)
                CategoryDropdown.FiltersChanged -= _filtersChangedHandler;
            _filtersChangedHandler = RefreshAllRows;
            CategoryDropdown.FiltersChanged += _filtersChangedHandler;
            _contentContainer.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                if (_filtersChangedHandler != null)
                {
                    CategoryDropdown.FiltersChanged -= _filtersChangedHandler;
                    _filtersChangedHandler = null;
                }
            });
            
            _contentContainer.RegisterCallback<AttachToPanelEvent>(evt =>
            {
                string stylePath = Path.Combine(Constants.packageAssetsFolder, Constants.uiToolkitTemplatesFolder, "ToolbarStyles.uss");
                StyleSheet styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylePath);
                VisualElement overlayContainer = _contentContainer.parent.parent.parent;
                VisualElement panel = _contentContainer.parent.parent;
                
                overlayContainer.styleSheets.Add(styles);
                overlayContainer.style.width = new StyleLength(StyleKeyword.Auto);

                // EditorApplication.delayCall += () =>
                // {
                //     Color currentColor = panel.resolvedStyle.backgroundColor;
                //     Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, .4f);
                //     panel.style.backgroundColor = newColor;
                // };
            });

            Button categoriesButton = new Button(OnCategoriesButtonClicked);
            categoriesButton.AddToClassList("categories-button");
            categoriesButton.tooltip = "Select the NoteCategories ScriptableObject.";

#if UNITY_6000_0_OR_NEWER
            if(activeLayout != Layout.HorizontalToolbar)
            {
                categoriesButton.style.maxWidth = 38;
            }
#endif

            Image buttonIcon = new Image();
            buttonIcon.AddToClassList("categories-button-icon");
            string iconPath = Path.Combine(Constants.packageAssetsFolder, Constants.uiImagesFolder, Constants.toolbarIconsFolder, $"{Constants.imagePrefix}CategoryOff.png");
            buttonIcon.image = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            categoriesButton.Add(buttonIcon);
            _contentContainer.Add(categoriesButton);
            
            VisualElement spacer = new();
            spacer.AddToClassList("spacer");
            _contentContainer.Add(spacer);

            for (int i = 0; i < NoteCategories.Instance.Categories.Count; i++)
            {
                int categoryIndex = i;
                NoteCategories.NoteCategory noteCategory = NoteCategories.Instance.Categories[i];
                string categoryName = noteCategory.name;

                VisualElement categoryLine = new();
                categoryLine.AddToClassList("legend-category-line");
                categoryLine.tooltip = "Click to toggle this category's visibility in the scene.";

                Image image = new Image();
                image.image = noteCategory.icon;
                image.AddToClassList("legend-icon");

                Label label = new Label(categoryName);
                label.AddToClassList("legend-label");

                categoryLine.Add(image);
                categoryLine.Add(label);

                UpdateRowVisualState(categoryLine, IsCategoryVisible(categoryName));

                categoryLine.AddManipulator(new Clickable(() =>
                {
                    ToggleCategoryVisibility(categoryName, categoryIndex);
                }));

                _rowsByCategoryName[categoryName] = categoryLine;
                _contentContainer.Add(categoryLine);

                spacer = new();
                spacer.AddToClassList("spacer");
                _contentContainer.Add(spacer);
            }
        }

        private static bool IsCategoryVisible(string categoryName)
        {
            if (!EditingSession.GetBool(EditingSession.FilteringDecks)) return true;
            if (CategoryDropdown.VisibleCategories == null) return true;
            return !CategoryDropdown.VisibleCategories.TryGetValue(categoryName, out bool visible) || visible;
        }

        private static void UpdateRowVisualState(VisualElement row, bool visible)
        {
            if (visible) row.RemoveFromClassList("legend-category-line-hidden");
            else row.AddToClassList("legend-category-line-hidden");
        }

        private void RefreshAllRows()
        {
            foreach (KeyValuePair<string, VisualElement> kvp in _rowsByCategoryName)
            {
                UpdateRowVisualState(kvp.Value, IsCategoryVisible(kvp.Key));
            }
        }

        private static void ToggleCategoryVisibility(string categoryName, int categoryIndex)
        {
            Dictionary<string, bool> visibleCategories = CategoryDropdown.VisibleCategories;
            if (visibleCategories == null || !visibleCategories.ContainsKey(categoryName)) return;

            bool masterWasOn = EditingSession.GetBool(EditingSession.FilteringDecks);

            // With master OFF, the overlay shows everything as visible, so wipe any
            // remembered filtering before applying the click — the click should toggle
            // from the "all on" state the user is actually looking at.
            if (!masterWasOn)
            {
                foreach (string key in new List<string>(visibleCategories.Keys))
                    visibleCategories[key] = true;
            }

            bool newValue = !visibleCategories[categoryName];
            visibleCategories[categoryName] = newValue;

            if (!masterWasOn)
            {
                int i = 0;
                foreach (KeyValuePair<string, bool> kvp in visibleCategories)
                {
                    NotesDatabase.instance.ShowHideNotesByCategory(i, kvp.Value);
                    i++;
                }
            }
            else
            {
                NotesDatabase.instance.ShowHideNotesByCategory(categoryIndex, newValue);
            }

            PersistCategoryFilterSession(visibleCategories);
            CategoryDropdown.NotifyFiltersChanged();
        }

        private static void PersistCategoryFilterSession(Dictionary<string, bool> visibleCategories)
        {
            int[] array = new int[visibleCategories.Count];
            int i = 0;
            foreach (string key in visibleCategories.Keys)
            {
                array[i++] = visibleCategories[key] ? 1 : 0;
            }
            EditingSession.SetArray(EditingSession.DecksArray, array);
            EditingSession.SetBool(EditingSession.FilteringDecks, visibleCategories.ContainsValue(false));
        }

        private void OnCategoriesButtonClicked()
        {
            EditorGUIUtility.PingObject(NoteCategories.Instance);
            Selection.activeObject = NoteCategories.Instance;
        }

        public override VisualElement CreatePanelContent()
        {
            VisualElement container = new();
            CreateContents(container);
            return container;
        }

        public OverlayToolbar CreateHorizontalToolbarContent()
        {
            OverlayToolbar overlayToolbar = new();
            CreateContents(overlayToolbar);
            return overlayToolbar;
        }

        public OverlayToolbar CreateVerticalToolbarContent()
        {
            OverlayToolbar overlayToolbar = new();
            CreateContents(overlayToolbar);
            return overlayToolbar;
        }
    }
}
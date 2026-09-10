using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Object = UnityEngine.Object;

namespace SceneNotes.Editor.CustomEditors
{
    [CustomEditor(typeof(SceneNote))]
    [CanEditMultipleObjects]
    public partial class SceneNoteEditor : UnityEditor.Editor
    {
        public VisualTreeAsset template;
        public VisualTreeAsset commentTemplate;
        
        private VisualElement _colorBar;
        private Button _startBtn;
        private Button _completeBtn;
        private SceneNote _sceneNote;
        private ObjectField _sceneField;
        private DropdownField _categoriesDropdown;
        private Label _editedOn;
        
        private bool _readyToSave;
        private bool _changesToSave;
        private VisualElement _commentDraft;
        private TextField _commentDraftContents;
        private TextField _commentDraftAuthor;
        private ListView _commentsList;
        private SerializedProperty _commentsProperty;
        private Button _locateBtn;
        private Vector3Field _localOffsetField;
        private VisualElement _localOffsetRow;
        private ObjectField _connectedObjectField;
        private Button _unlinkButton;
        private Vector3Field _worldPositionField;
        private bool _multiSelect;
        private SerializedProperty _screenshotsProperty;
        private readonly List<Texture2D> _screenshotsSnapshot = new();

        private const string EditCategoriesItem = "Edit categories...";

        public static ParentConstraint ParentConstraint;

        public override VisualElement CreateInspectorGUI()
        {
            return CreateInspectorGUI(false);
        }

        public VisualElement CreateInspectorGUI(bool asPartOfBehaviour)
        {
            VisualElement customInspector = new();
            template.CloneTree(customInspector);

            _sceneNote = (SceneNote)target;
            _multiSelect = targets.Length > 1;
            
            ObjectField sceneNoteField = customInspector.Q<ObjectField>("SceneNoteField");
            if(asPartOfBehaviour) sceneNoteField.SetEnabled(false);
            else sceneNoteField.style.display = DisplayStyle.None;

            _locateBtn = customInspector.Q<Button>("LocateBtn");
            if(_multiSelect) _locateBtn.SetEnabled(false);
            else if (asPartOfBehaviour) _locateBtn.style.display = DisplayStyle.None;
            else _locateBtn.clicked += LocateNoteInScene;

            _colorBar = customInspector.Q<VisualElement>("ColorBar");
            if(!_multiSelect) UpdateColorBar();

            TextField title = customInspector.Q<TextField>("Title");
            
            _startBtn = customInspector.Q<Button>("StartBtn");
            if(_multiSelect) _startBtn.SetEnabled(false);
            else
            {
                _startBtn.clicked += StartUnstart;
                _startBtn.text = _sceneNote.state == SceneNote.State.InProgress ? "Stop" : "Start";
            }
            
            _completeBtn = customInspector.Q<Button>("CompleteBtn");
            if(_multiSelect) _completeBtn.SetEnabled(false);
            else
            {
                _completeBtn.clicked += Complete;
                if (_sceneNote.state == SceneNote.State.Done) _completeBtn.SetEnabled(false);
            }

            Button deleteBtn = customInspector.Q<Button>("DeleteBtn");
            deleteBtn.clicked += Delete;
            
            Button takeScreenshot = customInspector.Q<Button>("TakeScreenshotBtn");
            takeScreenshot.clicked += EnterScreenshotMode;

            _screenshotsProperty = serializedObject.FindProperty("screenshots");
            ListView screenshotsList = customInspector.Q<ListView>("ScreenshotsList");
            screenshotsList.makeItem += OnMakeScreenshotItem;
            screenshotsList.bindItem += OnBindScreenshotItem;
            screenshotsList.unbindItem += OnUnbindScreenshotItem;
            RebuildScreenshotsSnapshot();
            screenshotsList.itemsRemoved += OnScreenshotsRemoved;
            customInspector.TrackSerializedObjectValue(serializedObject, _ => RebuildScreenshotsSnapshot());

            Label authorName = customInspector.Q<VisualElement>("Author").Q<Label>("Name");
            authorName.text = _sceneNote.author;

            Label createdOn = customInspector.Q<VisualElement>("CreatedOn").Q<Label>("Date");
            CheckDateValidity(nameof(SceneNote.creationDate));
            createdOn.text = Utilities.HumanReadableDate_Short(_sceneNote.creationDate);
            
            _editedOn = customInspector.Q<VisualElement>("EditedOn").Q<Label>("Date");
            CheckDateValidity(nameof(SceneNote.modifiedDate));
            _editedOn.text = Utilities.HumanReadableDate_Short(_sceneNote.modifiedDate);

            _worldPositionField = customInspector.Q<Vector3Field>("PositionField");
            
            _connectedObjectField = customInspector.Q<ObjectField>("ConnectedObjectField");
            ContextualMenuManipulator contextualMenuManipulator = new(MenuBuilder);
            void MenuBuilder(ContextualMenuPopulateEvent evt)
            {
                ContextualMenu.BuildRightClickMenu(evt, false, false, true, true, false, false, true);
            }
            _connectedObjectField.AddManipulator(contextualMenuManipulator);
            
            _unlinkButton = customInspector.Q<Button>("UnlinkButton");
            _unlinkButton.AddToClassList(EditorGUIUtility.isProSkin ? "unlink-btn-icon-dark" : "unlink-btn-icon-light");
            
            GameObject retrievedGO = null;
            if(_multiSelect)
            {
                _connectedObjectField.SetEnabled(false);
                _worldPositionField.SetEnabled(false);
                _unlinkButton.SetEnabled(false);
            }
            else
            {
                if(!string.IsNullOrEmpty(_sceneNote.connectedObjectGlobalObjectID))
                {
                    ResolveReferencedObject(serializedObject, out retrievedGO);
                    if (asPartOfBehaviour &&
                        _sceneNote.ConnectedTransform != null)
                    {
                        SetupParentConstraint(_sceneNote);
                        EnableParentConstraint(false);
                    }
                    _connectedObjectField.SetValueWithoutNotify(retrievedGO);
                }
                
                _worldPositionField.SetEnabled(retrievedGO == null);
                _unlinkButton.SetEnabled(retrievedGO != null);
            }

            _localOffsetRow = customInspector.Q<VisualElement>("LocalOffsetRow");
            _localOffsetField = _localOffsetRow.Q<Vector3Field>("LocalOffsetField");
            _localOffsetRow.SetEnabled(_connectedObjectField.value != null);
            _localOffsetRow.AddToClassList(_connectedObjectField.value != null ? "visible-field" : "hidden-field");

            Button resetLocalOffsetBtn = _localOffsetRow.Q<Button>("ResetLocalOffsetBtn");
            resetLocalOffsetBtn.clicked += () => _localOffsetField.value = Vector3.zero;

            if(NoteCategories.Instance == null) NotesDatabase.WarmUpCategories();
            int safeCategoryId = NoteCategories.GetSafeId(serializedObject.FindProperty(nameof(SceneNote.categoryId)).intValue);
            _categoriesDropdown = customInspector.Q<DropdownField>("CategoriesDropdown");
            _categoriesDropdown.choices = NoteCategories.Instance.Categories.Select(cat => cat.name).ToList();
            _categoriesDropdown.choices.Add("");
            _categoriesDropdown.choices.Add(EditCategoriesItem);
            _categoriesDropdown.SetValueWithoutNotify(_categoriesDropdown.choices[safeCategoryId]);
            if (_multiSelect)
            {
                int firstValue = ((SceneNote)target).categoryId;
                foreach (Object obj in targets)
                {
                    SceneNote currentSceneNote = (SceneNote)obj;
                    int nextValue = currentSceneNote.categoryId;
                    if (firstValue != nextValue)
                    {
                        _categoriesDropdown.showMixedValue = true;
                        break;
                    }
                }
            }

            _sceneField = customInspector.Q<ObjectField>("SceneField");
            _sceneField.RegisterCallback<GeometryChangedEvent>(SceneFieldReady);

            Foldout screenshotsFoldout = customInspector.Q<Foldout>("ScreenshotsFoldout");
            screenshotsFoldout.SetEnabled(!_multiSelect);
            if (_multiSelect) EditorApplication.delayCall = () => screenshotsFoldout.value = false;
            
            Foldout moreFoldout = customInspector.Q<Foldout>("More");
            moreFoldout.SetEnabled(!_multiSelect);
            if (_multiSelect) EditorApplication.delayCall = () => moreFoldout.value = false;

            _commentsProperty = serializedObject.FindProperty("comments");
            _commentsList = customInspector.Q<ListView>("CommentsList");
            _commentsList.Q<TextField>("unity-list-view__size-field").SetEnabled(false); // Disable list length
            
            _commentsList.makeItem += OnMakeCommentItem;
            _commentsList.bindItem += OnBindCommentItem;
            _commentsList.unbindItem += OnUnbindCommentItem;
            _commentsList.destroyItem += DestroyItem;

            Foldout commentsFoldout = _commentsList.Q<Foldout>("unity-list-view__foldout-header");
            commentsFoldout.SetEnabled(!_multiSelect);
            if (_multiSelect) EditorApplication.delayCall = () => commentsFoldout.value = false;
            
            _commentDraft = customInspector.Q<VisualElement>("CommentDraft");
            _commentDraftContents = _commentDraft.Q<TextField>("Contents");
            _commentDraft.Q<Button>("AddCommentBtn").clicked += AddComment;
            _commentDraftScreenshotPreview = _commentDraft.Q<Image>("CommentDraftScreenshotPreview");
            // Click the preview to discard the pending capture before committing.
            _commentDraftScreenshotPreview.RegisterCallback<ClickEvent>(_ => DiscardPendingCommentScreenshot());
            _commentDraft.Q<Button>("CommentDraftTakeScreenshotBtn").clicked +=
                () => EnterScreenshotMode(ScreenshotTarget.CommentDraft);
            UpdateCommentDraftPreview();
            _commentDraft.style.display = DisplayStyle.None;

            EditorApplication.delayCall += () =>
            {
                // Value Changed Callbacks
                title.RegisterValueChangedCallback(OnTitleChanged);
                _connectedObjectField.RegisterValueChangedCallback(OnConnectedObjectChanged);
                _worldPositionField.RegisterValueChangedCallback(OnWorldPositionChanged);
                _localOffsetField.RegisterValueChangedCallback(OnLocalOffsetChanged);
                _categoriesDropdown.RegisterValueChangedCallback(OnCategoryChanged);
                _unlinkButton.clicked += OnUnlinkButtonClicked;
            };
            commentsFoldout.RegisterValueChangedCallback(OnCommentsListOpenClose);
            
            customInspector.RegisterCallback<InputEvent>(OnTextChanged);
            customInspector.RegisterCallback<FocusOutEvent>(OnFocusLost);
            customInspector.RegisterCallback<DetachFromPanelEvent>(OnDetach);

            EditorApplication.delayCall += () => _readyToSave = true;

            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            return customInspector;
        }

        private void OnUndoRedoPerformed()
        {
            if (target == null || _multiSelect) return;
            serializedObject.Update();

            // Re-resolve the connected object from the (potentially reverted) GlobalObjectId
            // without going through ResolveReferencedObject (which mutates the SO).
            string stringGoID = serializedObject.FindProperty(nameof(SceneNote.connectedObjectGlobalObjectID)).stringValue;
            GameObject retrievedGO = null;
            if (!string.IsNullOrEmpty(stringGoID) && GlobalObjectId.TryParse(stringGoID, out GlobalObjectId goID))
                retrievedGO = (GameObject)GlobalObjectId.GlobalObjectIdentifierToObjectSlow(goID);
            _sceneNote.ConnectedTransform = retrievedGO != null ? retrievedGO.transform : null;
            _connectedObjectField.SetValueWithoutNotify(retrievedGO);

            bool isConnectedToSomething = retrievedGO != null;
            _unlinkButton.SetEnabled(isConnectedToSomething);
            _localOffsetRow.SetEnabled(isConnectedToSomething);
            _worldPositionField.SetEnabled(!isConnectedToSomething);
            AnimateFieldVisibility(_localOffsetRow, isConnectedToSomething);

            // State-derived widgets
            UpdateColorBar();
            _startBtn.text = _sceneNote.state == SceneNote.State.InProgress ? "Stop" : "Start";
            _completeBtn.SetEnabled(_sceneNote.state != SceneNote.State.Done);

            // Category dropdown
            int safeCategoryId = NoteCategories.GetSafeId(_sceneNote.categoryId);
            _categoriesDropdown.SetValueWithoutNotify(_categoriesDropdown.choices[safeCategoryId]);

            // Scene icon
            UpdateIcon();
        }

        private void OnUnlinkButtonClicked()
        {
            _connectedObjectField.value = null;
        }

        private void OnDetach(DetachFromPanelEvent evt)
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            if(ParentConstraint != null) EnableParentConstraint(true);
            ParentConstraint = null;
            DiscardPendingCommentScreenshot();
        }

        public static void ResolveReferencedObject(SerializedObject serObj, out GameObject retrievedGO)
        {
            SceneNote targetObject = (SceneNote)serObj.targetObject;
            serObj.Update();
            retrievedGO = null;

            SerializedProperty connectedObjProp = serObj.FindProperty(nameof(SceneNote.connectedObjectGlobalObjectID));
            string stringGoID = connectedObjProp.stringValue;
            if (!string.IsNullOrEmpty(stringGoID))
            {
                if (GlobalObjectId.TryParse(stringGoID, out GlobalObjectId goID))
                {
                    retrievedGO = (GameObject)GlobalObjectId.GlobalObjectIdentifierToObjectSlow(goID);
                    targetObject.ConnectedTransform = retrievedGO?.transform;

                    if (retrievedGO == null)
                    {
                        Debug.LogError($"{Constants.packagePrefix} Connected object not found in the scene.");
                        connectedObjProp.stringValue = string.Empty;
                        serObj.ApplyModifiedProperties();
                    }
                }
                else
                {
                    Debug.LogError($"{Constants.packagePrefix} Connected object not found. Invalid ID.");
                    connectedObjProp.stringValue = string.Empty;
                    serObj.ApplyModifiedProperties();
                }
            }

            PositionNote(serObj);
        }

        private static void PositionNote(SerializedObject serObj, bool useLocalOffset = true)
        {
            SceneNote targetObject = (SceneNote)serObj.targetObject;
            serObj.Update();
            
            Vector3 finalPosition;

            SerializedProperty worldPosProp = serObj.FindProperty(nameof(SceneNote.worldPosition));
            SerializedProperty localOffsetProp = serObj.FindProperty(nameof(SceneNote.localOffset));

            if(targetObject.ConnectedTransform == null)
            {
                finalPosition = worldPosProp.vector3Value;
                localOffsetProp.vector3Value = Vector3.zero;
            }
            else
            {
                if (useLocalOffset)
                {
                    // Use local to calculate world. Used when local position matters because there's a connected object
                    Vector3 localOffset = localOffsetProp.vector3Value;
                    finalPosition = targetObject.ConnectedTransform.TransformPoint(localOffset);
                    worldPosProp.vector3Value = finalPosition;
                }
                else
                {
                    // Use world to calculate new local. Used when the connected object is newly connected, so we preserve the world position.
                    finalPosition = worldPosProp.vector3Value;
                    localOffsetProp.vector3Value = targetObject.ConnectedTransform.InverseTransformPoint(worldPosProp.vector3Value);
                    
                    if (ParentConstraint != null)
                    {
                        Vector3 scaler = targetObject.ConnectedTransform.localScale;
                        ParentConstraint.SetTranslationOffset(0, Vector3.Scale(localOffsetProp.vector3Value, scaler));
                    }
                }
                
            }
            
            serObj.ApplyModifiedProperties();

            if (targetObject.referencingNoteBehaviour != null)
                targetObject.referencingNoteBehaviour.transform.position = finalPosition;
        }

        public static void SetupParentConstraint(SceneNote sceneNote, bool asLocked = true, bool registerUndo = false)
        {
            if (sceneNote.ConnectedTransform == null)
            {
                // Note is not connected to any object
                ParentConstraint existing = sceneNote.referencingNoteBehaviour.GetComponent<ParentConstraint>();
                if (existing != null)
                {
                    if (registerUndo) Undo.DestroyObjectImmediate(existing);
                    else DestroyImmediate(existing);
                }
                return;
            }

            // There is an object connected
            bool componentExisted = sceneNote.referencingNoteBehaviour.TryGetComponent(out ParentConstraint);
            if (!componentExisted)
            {
                // If comp is not already present, add it
                GameObject goInScene = sceneNote.referencingNoteBehaviour.gameObject;
                ParentConstraint = registerUndo
                    ? Undo.AddComponent<ParentConstraint>(goInScene)
                    : goInScene.AddComponent<ParentConstraint>();
            }
            else if (registerUndo)
            {
                Undo.RecordObject(ParentConstraint, "Re-link Scene Note");
            }
            ParentConstraint.hideFlags = HideFlags.HideInInspector | HideFlags.DontSaveInEditor;

            ConstraintSource constraintSource = new ConstraintSource
            {
                sourceTransform = sceneNote.ConnectedTransform,
                weight = 1f
            };
            if (ParentConstraint.sourceCount == 0)
                ParentConstraint.AddSource(constraintSource);
            else
                ParentConstraint.SetSource(0, constraintSource);

            Vector3 scaler = sceneNote.ConnectedTransform.localScale;
            ParentConstraint.SetTranslationOffset(0, Vector3.Scale(sceneNote.localOffset, scaler));
            ParentConstraint.constraintActive = true;
            ParentConstraint.locked = asLocked;
        }

        private static void EnableParentConstraint(bool on) => ParentConstraint.locked = on;

        private void OnConnectedObjectChanged(ChangeEvent<Object> evt)
        {
            _sceneNote.ConnectedTransform = (Transform)evt.newValue;
            serializedObject.Update();

            bool isConnectedToSomething = evt.newValue != null;
            string newGoID = "";
            if(isConnectedToSomething)
            {
                GlobalObjectId goID = GlobalObjectId.GetGlobalObjectIdSlow(_sceneNote.ConnectedTransform.gameObject);
                if(goID.identifierType != 2)
                {
                    // TODO: Complain
                    return;
                }
                newGoID = goID.ToString();
            }

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName(isConnectedToSomething ? "Link Scene Note" : "Unlink Scene Note");

            PositionNote(serializedObject, false);
            SetupParentConstraint(_sceneNote, asLocked: false, registerUndo: true);

            serializedObject.FindProperty(nameof(SceneNote.connectedObjectGlobalObjectID)).stringValue = newGoID;
            serializedObject.ApplyModifiedProperties();

            Undo.CollapseUndoOperations(undoGroup);

            // Tweak fields
            _unlinkButton.SetEnabled(isConnectedToSomething);
            _localOffsetRow.SetEnabled(isConnectedToSomething);
            _worldPositionField.SetEnabled(!isConnectedToSomething);
            AnimateFieldVisibility(_localOffsetRow, isConnectedToSomething);
        }

        private void AnimateFieldVisibility(VisualElement field, bool visible)
        {
            if (visible)
            {
                field.RemoveFromClassList("hidden-field");
                field.AddToClassList("visible-field");
            }
            else
            {
                field.RemoveFromClassList("visible-field");
                field.AddToClassList("hidden-field");
            }
        }

        private void CheckDateValidity(string propName)
        {
            string dateProp = serializedObject.FindProperty(propName).stringValue;
            if(string.IsNullOrEmpty(dateProp) || !DateTime.TryParseExact(dateProp, Utilities.DateFormat, null, DateTimeStyles.None, out DateTime _))
            {
                WriteDateNow(propName);
            }
        }

        private void CheckIfCanLocate()
        {
            bool canLocate = _sceneField.value != null && SceneManager.GetSceneByName(((SceneAsset)_sceneField.value).name).isLoaded;
            _locateBtn.SetEnabled(!_multiSelect && canLocate);
        }

        private void OnTitleChanged(ChangeEvent<string> evt)
        {
            foreach (Object obj in targets)
            {
                SceneNote currentNote = (SceneNote)obj;
                SceneNoteBehaviour snb = currentNote.referencingNoteBehaviour;
                
                string oldName = currentNote.name;
                string assetPath = AssetDatabase.GetAssetPath(currentNote);
                if (string.IsNullOrEmpty(currentNote.suffix))
                {
                    serializedObject.Update();
                    serializedObject.FindProperty("suffix").stringValue = Utilities.FourNumbers();
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }
                string newName = $"{Utilities.Hyphenate(evt.newValue)}_{currentNote.suffix}";
                if (oldName == newName) return;
                
                //Check for duplicates
                string directoryName = Path.GetDirectoryName(assetPath);
                if (File.Exists(Path.Combine(directoryName!, newName + ".asset")))
                {
                    currentNote.suffix = Utilities.FourNumbers();
                    newName = $"{Utilities.Hyphenate(evt.newValue)}_{currentNote.suffix}";
                }
                
                string result = AssetDatabase.RenameAsset(assetPath, newName);
                if (!string.IsNullOrEmpty(result))
                {
                    Debug.LogError($"{Constants.packagePrefix} {Constants.errorRenaming}");
                }
                else
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                // This gets lost during database refresh, so it needs to be reconnected
                currentNote.referencingNoteBehaviour = snb;
            }
        }

        private VisualElement OnMakeScreenshotItem()
        {
            VisualElement container = new();
            PropertyField propertyField = new() { name = "ScreenshotProperty" };
            propertyField.SetEnabled(false);
            container.Add(propertyField);
            
            VisualElement screenshotContainer = new() { name = "ScreenshotContainer" };
            screenshotContainer.AddToClassList("screenshot-container");

            Image thumbnail = new()
            {
                name = "Thumbnail",
                scaleMode = ScaleMode.ScaleToFit
            };
            thumbnail.AddToClassList("screenshot-thumbnail");
            screenshotContainer.Add(thumbnail);
            
            container.Add(screenshotContainer);

            return container;
        }

        private void OnBindScreenshotItem(VisualElement element, int i)
        {
            SerializedProperty itemProperty = _screenshotsProperty.GetArrayElementAtIndex(i);
            PropertyField propertyField = element.Q<PropertyField>("ScreenshotProperty");
            propertyField.BindProperty(itemProperty);

            Image thumbnail = element.Q<Image>("Thumbnail");
            Texture2D tex = itemProperty.objectReferenceValue as Texture2D;
            thumbnail.image = tex;
            thumbnail.style.display = tex != null ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnUnbindScreenshotItem(VisualElement element, int i)
        {
            PropertyField propertyField = element.Q<PropertyField>("ScreenshotProperty");
            propertyField.Unbind();
        }

        private void OnScreenshotsRemoved(IEnumerable<int> indices)
        {
            // itemsRemoved fires after the bound array is mutated; use the pre-removal snapshot.
            foreach (int index in indices)
            {
                if (index < 0 || index >= _screenshotsSnapshot.Count) continue;
                DeleteScreenshotAsset(_screenshotsSnapshot[index]);
            }
        }

        private void RebuildScreenshotsSnapshot()
        {
            _screenshotsSnapshot.Clear();
            if (_sceneNote == null || _sceneNote.screenshots == null) return;
            _screenshotsSnapshot.AddRange(_sceneNote.screenshots);
        }

        public static void DeleteLinkedScreenshots(SceneNote note)
        {
            if (note == null) return;
            if (note.screenshots != null)
            {
                foreach (Texture2D tex in note.screenshots)
                {
                    if (tex == null) continue;
                    // Sub-assets travel with the note, so no work needed here.
                    if (!AssetDatabase.IsMainAsset(tex)) continue;
                    DeleteScreenshotAsset(tex);
                }
            }
            if (note.comments != null)
            {
                foreach (NoteComment comment in note.comments)
                {
                    Texture2D tex = comment.screenshot;
                    if (tex == null) continue;
                    if (!AssetDatabase.IsMainAsset(tex)) continue;
                    DeleteScreenshotAsset(tex);
                }
            }
        }

        private static void DeleteScreenshotAsset(Texture2D tex)
        {
            if (tex == null) return;
            string path = AssetDatabase.GetAssetPath(tex);
            if (string.IsNullOrEmpty(path)) return;

            if (!AssetDatabase.IsMainAsset(tex))
            {
                // Sub-asset of the note's .asset: unlink without trashing the containing file.
                Object parent = AssetDatabase.LoadMainAssetAtPath(path);
                AssetDatabase.RemoveObjectFromAsset(tex);
                if (parent != null) AssetDatabase.SaveAssetIfDirty(parent);
                return;
            }

            // Safety net: a screenshot should always be a loose .png when stored as a main asset.
            // Refuse to trash anything else (prevents nuking the note's .asset by mistake).
            if (!path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) return;

            AssetDatabase.MoveAssetToTrash(path);

            // Walk up the noteGuid then sceneGuid folder; remove each if it became empty.
            string noteFolder = Path.GetDirectoryName(path);
            if (!TryDeleteIfEmpty(noteFolder)) return;
            string sceneFolder = Path.GetDirectoryName(noteFolder);
            TryDeleteIfEmpty(sceneFolder);
        }

        private static bool TryDeleteIfEmpty(string folderRelPath)
        {
            if (string.IsNullOrEmpty(folderRelPath)) return false;
            string abs = Utilities.MakePathAbsolute(folderRelPath);
            if (!Directory.Exists(abs)) return false;
            if (Directory.GetFileSystemEntries(abs).Length > 0) return false;
            return AssetDatabase.DeleteAsset(folderRelPath);
        }

        private VisualElement OnMakeCommentItem()
        {
            TemplateContainer templateContainer = commentTemplate.CloneTree();
            templateContainer.Q<Button>("DeleteCommentBtn").clicked += () => RemoveComment(templateContainer);
            templateContainer.AddToClassList("CommentTemplate");
            
            return templateContainer;
        }

        private void DestroyItem(VisualElement element) { }

        private void OnBindCommentItem(VisualElement element, int i)
        {
            TemplateContainer propField = (TemplateContainer)element;
            SerializedProperty commentProp = _commentsProperty.GetArrayElementAtIndex(i);
            propField.BindProperty(commentProp);
            propField.userData = i;

            string createdOn = commentProp.FindPropertyRelative("date").stringValue;
            string authorName = commentProp.FindPropertyRelative("author").stringValue;
            element.Q<Label>("Name").text = authorName;
            element.Q<Label>("Date").text = Utilities.HumanReadableDate_Short(createdOn);

            Image screenshot = element.Q<Image>("CommentScreenshot");
            Texture2D tex = commentProp.FindPropertyRelative("screenshot").objectReferenceValue as Texture2D;
            screenshot.image = tex;
            screenshot.style.display = tex != null ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnUnbindCommentItem(VisualElement element, int i)
        {
            TemplateContainer propField = (TemplateContainer)element;
            propField.Unbind();
        }

        private void RemoveComment(VisualElement btn)
        {
            serializedObject.Update();
            int index = (int)btn.userData;
            SerializedProperty commentProp = _commentsProperty.GetArrayElementAtIndex(index);
            Texture2D attached = commentProp.FindPropertyRelative("screenshot").objectReferenceValue as Texture2D;
            DeleteScreenshotAsset(attached);
            _commentsProperty.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }

        private void AddComment()
        {
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Add Scene Note Comment");

            // Persist a pending screenshot (if any) before writing the comment, so the array entry
            // sees a final, committed Texture2D reference.
            Texture2D persistedScreenshot = null;
            if (_pendingCommentScreenshot != null)
            {
                persistedScreenshot = PersistScreenshot(_pendingCommentScreenshot);
                _pendingCommentScreenshot = null;
                UpdateCommentDraftPreview();
            }

            serializedObject.Update();
            int newIndex = _commentsProperty.arraySize;
            _commentsProperty.InsertArrayElementAtIndex(newIndex);
            SerializedProperty arrayProp = _commentsProperty.GetArrayElementAtIndex(newIndex);
            arrayProp.FindPropertyRelative("contents").stringValue = _commentDraftContents.value;
            arrayProp.FindPropertyRelative("author").stringValue = SceneNotesSettings.authorName;
            arrayProp.FindPropertyRelative("date").stringValue = Utilities.DateToString(DateTime.Now);
            arrayProp.FindPropertyRelative("screenshot").objectReferenceValue = persistedScreenshot;
            serializedObject.ApplyModifiedProperties();

            Undo.CollapseUndoOperations(undoGroup);

            _commentDraftContents.value = "";
        }

        private void OnCommentsListOpenClose(ChangeEvent<bool> evt)
        {
            Foldout foldout = (Foldout)evt.target;
            _commentDraft.style.display = foldout.value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnTextChanged(InputEvent evt)
        {
            if(!_readyToSave) return;
            if(evt.previousData != evt.newData) _changesToSave = true;
        }

        private void OnFocusLost(FocusOutEvent focusOutEvent)
        {
            if(_changesToSave && _readyToSave) SaveModifiedDate();
        }

        private void SaveModifiedDate()
        {
            if(!_readyToSave) return;
            
            WriteDateNow("modifiedDate");
            _editedOn.text = Utilities.HumanReadableDate_Short(_sceneNote.modifiedDate);
            _changesToSave = false;
        }

        private void WriteDateNow(string propName)
        {
            serializedObject.Update();
            serializedObject.FindProperty(propName).stringValue = Utilities.DateToString(DateTime.Now);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void OnCategoryChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == EditCategoriesItem)
            {
                _categoriesDropdown.SetValueWithoutNotify(evt.previousValue);
                EditorGUIUtility.PingObject(NoteCategories.Instance);
                Selection.activeObject = NoteCategories.Instance;
                return;
            }
            
            serializedObject.Update();
            serializedObject.FindProperty("categoryId").intValue = _categoriesDropdown.index;
            serializedObject.ApplyModifiedProperties();
            SaveModifiedDate();

            UpdateIcon();
            ReParentInScene();
        }

        private void SceneFieldReady(GeometryChangedEvent evt)
        {
            _sceneField.UnregisterCallback<GeometryChangedEvent>(SceneFieldReady);
            _sceneField.RegisterValueChangedCallback(OnSceneChanged);
            CheckIfCanLocate();
        }

        private void OnSceneChanged(ChangeEvent<Object> evt)
        {
            if(evt.newValue == evt.previousValue
               || evt.newValue == null) return;

            serializedObject.FindProperty(nameof(SceneNote.scene)).objectReferenceValue = evt.newValue;

            serializedObject.ApplyModifiedProperties();
            SaveModifiedDate();

            foreach (Object obj in targets)
            {
                SceneNote currentNote = (SceneNote)obj;
                
                // Move ScriptableObject and GameObject
                SceneAsset newScene = (SceneAsset)evt.newValue;
                string currentFilePath = AssetDatabase.GetAssetPath(currentNote);
                string newSceneGuid = Utilities.AssetToGuid(newScene);
                
                string notesFolderPath = Utilities.GetRelNotesFolderPath();
                string newFolderPath = Path.Combine(notesFolderPath, newSceneGuid);
                
                string absoluteNewFolderPath = Utilities.MakePathAbsolute(newFolderPath);
                
                if (!Directory.Exists(absoluteNewFolderPath))
                {
                    AssetDatabase.CreateFolder(notesFolderPath, newSceneGuid);
                    AssetDatabase.Refresh();
                }
                
                string assetName = $"{currentNote.name}.asset";
                string newFilePath = Path.Combine(newFolderPath, assetName);

                if (string.Equals(currentFilePath, newFilePath)) return;

                string moveMsg = AssetDatabase.MoveAsset(currentFilePath, newFilePath);
                if (moveMsg != string.Empty)
                {
                    Debug.Log($"{Constants.packagePrefix} {Constants.errorMovingFile} {moveMsg}");
                }

                // Move the GameObject or destroy it
                if(NotesDatabase.instance.SceneIsCurrentlyLoaded(newSceneGuid)) ReParentInScene();
                else if(currentNote.referencingNoteBehaviour != null)
                {
                    GameObject behaviourGO = currentNote.referencingNoteBehaviour.gameObject;
#if UNITY_6000_4_OR_NEWER
                    NotesDatabase.instance.UntrackNoteGO(behaviourGO.GetEntityId());
#else
                    NotesDatabase.instance.UntrackNoteGO(behaviourGO.GetInstanceID());
#endif
                    DestroyImmediate(behaviourGO);
                }

                // Cleanup previous folder
                if (evt.previousValue == null) return;
                
                string oldSceneGuid = Utilities.AssetToGuid(evt.previousValue);
                string oldFolderPath = Path.Combine(notesFolderPath, oldSceneGuid);
                string absoluteOldFolderPath = Utilities.MakePathAbsolute(oldFolderPath);
                
                if(Directory.GetFiles(absoluteOldFolderPath).Length == 0)
                {
                    AssetDatabase.DeleteAsset(oldFolderPath);
                    AssetDatabase.Refresh();
                }
            }

            CheckIfCanLocate();
            RefreshBrowserWindowIfExists();
        }

        private void ReParentInScene()
        {
            if (_sceneNote.referencingNoteBehaviour == null) return; // The note is not actually shown in the scene

            GUID guid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(_sceneNote.scene));
            NotesDatabase.instance.ParentNoteGOAccordingly(_sceneNote.referencingNoteBehaviour.gameObject, _sceneNote,
                guid.ToString(), registerUndo: true);
        }

        private void OnWorldPositionChanged(ChangeEvent<Vector3> evt)
        {
            if(_worldPositionField.enabledSelf &&
               _sceneNote.referencingNoteBehaviour != null)
            {
                PositionNote(serializedObject);
            }
        }

        private void OnLocalOffsetChanged(ChangeEvent<Vector3> evt)
        {
            if (_localOffsetField.enabledInHierarchy &&
                _sceneNote.ConnectedTransform != null)
            {
                PositionNote(serializedObject);
            }
        }

        private void UpdateColorBar()
        {
            _colorBar.ClearClassList();
            switch (_sceneNote.state)
            {
                case SceneNote.State.NotStarted:
                    string suffix = EditorGUIUtility.isProSkin ? "_Dark" : "_Light";
                    _colorBar.AddToClassList($"NotStarted{suffix}");
                    break;
                case SceneNote.State.InProgress:
                    _colorBar.AddToClassList("InProgress");
                    break;
                case SceneNote.State.Done:
                    _colorBar.AddToClassList("Done");
                    break;
            }
        }

        private void StartUnstart()
        {
            serializedObject.Update();
            SerializedProperty stateProperty = serializedObject.FindProperty(nameof(SceneNote.state));
            if (stateProperty.enumValueIndex == (int)SceneNote.State.InProgress)
            {
                stateProperty.enumValueIndex = (int)SceneNote.State.NotStarted;
                _startBtn.text = "Start";
            }
            else
            {
                stateProperty.enumValueIndex = (int)SceneNote.State.InProgress;
                _startBtn.text = "Stop";
            }
            _completeBtn.SetEnabled(stateProperty.enumValueIndex != (int)SceneNote.State.Done);
            
            serializedObject.ApplyModifiedProperties();
            
            UpdateColorBar();
            UpdateIcon();
            RefreshBrowserWindowIfExists();
        }

        private void Complete()
        {
            serializedObject.Update();
            SerializedProperty stateProperty = serializedObject.FindProperty(nameof(SceneNote.state));
            stateProperty.enumValueIndex = (int)SceneNote.State.Done;
            
            _startBtn.text = "Start";
            _completeBtn.SetEnabled(stateProperty.enumValueIndex != (int)SceneNote.State.Done);
            
            serializedObject.ApplyModifiedProperties();
            
            UpdateColorBar();
            UpdateIcon();
            RefreshBrowserWindowIfExists();
        }

        private void UpdateIcon()
        {
            // Todo: This doesn't work in multi-selection
            if (_sceneNote.referencingNoteBehaviour != null) NotesDatabase.instance.SetNoteIcon(_sceneNote);
        }

        private void Delete()
        {
            int count = targets.Length;
            string message = count == 1
                ? $"Delete the note \"{((SceneNote)target).title}\"? This cannot be undone."
                : $"Delete {count} notes? This cannot be undone.";
            if (!EditorUtility.DisplayDialog("Delete Scene Note", message, "Delete", "Cancel"))
                return;

            foreach (Object obj in targets)
            {
                SceneNote currentNote = (SceneNote)obj;

                GameObject behaviourGO = currentNote.referencingNoteBehaviour.gameObject;
#if UNITY_6000_4_OR_NEWER
                NotesDatabase.instance.UntrackNoteGO(behaviourGO.GetEntityId());
#else
                NotesDatabase.instance.UntrackNoteGO(behaviourGO.GetInstanceID());
#endif
                DestroyImmediate(behaviourGO);
                DeleteLinkedScreenshots(currentNote);
                string path = AssetDatabase.GetAssetPath(currentNote);
                AssetDatabase.MoveAssetToTrash(path);
            }

            RefreshBrowserWindowIfExists();
        }

        private void LocateNoteInScene()
        {
            if (_sceneNote.referencingNoteBehaviour == null)
            {
                Debug.LogError($"{Constants.packagePrefix} {Constants.noteNotFound}");
                return;
            }
            
            Selection.activeObject = _sceneNote.referencingNoteBehaviour.gameObject;
            SceneView.lastActiveSceneView.FrameSelected();
        }

        private void RefreshBrowserWindowIfExists()
        {
            if(NotesBrowser.IsOpen) NotesBrowser.RefreshNotes();
        }
    }
}
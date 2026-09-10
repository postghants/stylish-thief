#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif
using UnityEngine;

namespace SceneNotes
{
    public class SceneNoteBehaviour : MonoBehaviour
    {
        public static bool DisplayTitleInScene;
        public static Color TitleColour;
        public static Color TitleBackgroundColour;
        public static int MaxTitleLength;

        public SceneNote note;

#if UNITY_EDITOR
        [SerializeField] private float _maxWidth = 100f;
        
        private void OnDrawGizmos()
        {
            if (!DisplayTitleInScene) return;
            
            Camera sceneCamera = SceneView.lastActiveSceneView.camera;
            Vector3 viewPos = sceneCamera.WorldToViewportPoint(transform.position);
            
            if (viewPos.z <= 0 || viewPos.x < -0.1f || viewPos.x > 1.1f || viewPos.y < -0.1f || viewPos.y > 1.1f)
                return;
            
            float handleSize = HandleUtility.GetHandleSize(transform.position);
            float iconSize = GizmoUtility.iconSize;
            
            int fontSize = GizmoUtility.use3dIcons ? Mathf.Clamp(Mathf.RoundToInt(iconSize * 900f / handleSize), 1, 14) : 11;
            
            Color titleColour = TitleColour;
            Color titleBackgroundColour = TitleBackgroundColour;
            if (fontSize < 6)
            {
                float fadeFactor = (fontSize-1) * .18f;
                titleColour.a *= fadeFactor;
                titleBackgroundColour.a *= fadeFactor;
            }

            GUIStyle textStyle = new(EditorStyles.whiteLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize,
                wordWrap = true,
                normal = new GUIStyleState { textColor = titleColour }
            };

            Vector3 offset = SceneView.lastActiveSceneView.camera.transform.up * (GizmoUtility.use3dIcons ? iconSize * 18f : handleSize / 4.4f);
            Vector3 labelPosition = transform.position - offset;

            string noteTitle = note.title;
            if (noteTitle.Length > MaxTitleLength)
            {
                noteTitle = noteTitle.Remove(MaxTitleLength, noteTitle.Length - MaxTitleLength);
                noteTitle += "...";
            }
            GUIContent content = new GUIContent(noteTitle);
            
            if (Event.current.type != EventType.Repaint) return;

            if (GizmoUtility.use3dIcons && fontSize >= 2 && IsOccluded(sceneCamera, labelPosition))
                return;

            Handles.BeginGUI();
            
            float scaledMaxWidth = _maxWidth * fontSize / 11f;
            
            Vector2 textSize = textStyle.CalcSize(content);
            if (textSize.x > scaledMaxWidth)
            {
                float height = textStyle.CalcHeight(content, scaledMaxWidth);
                textSize = new Vector2(scaledMaxWidth, height);
            }
            
            float padding = fontSize * 0.05f;
            Vector2 boxSize = new Vector2(textSize.x + padding * 2f, textSize.y + padding);
            
            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(labelPosition);
            Rect boxRect = new Rect(guiPosition.x - boxSize.x * 0.5f, guiPosition.y, boxSize.x, boxSize.y);
            
            GUIStyle roundedStyle = new GUIStyle();
            roundedStyle.normal.background = EditorGUIUtility.whiteTexture;
            
            Color originalColor = GUI.color;
            GUI.color = titleBackgroundColour;
            
            GUI.Box(boxRect, GUIContent.none, roundedStyle);
            GUI.color = originalColor;
            
            GUI.Label(boxRect, content, textStyle);
            
            Handles.EndGUI();
        }

        private static MethodInfo _intersectRayMesh;
        private static MeshRenderer[] _cachedRenderers;
        private static int _cachedRenderersFrame = -1;

        private bool IsOccluded(Camera sceneCamera, Vector3 targetPosition)
        {
            if (_intersectRayMesh == null)
            {
                _intersectRayMesh = typeof(HandleUtility).GetMethod(
                    "IntersectRayMesh",
                    BindingFlags.Static | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(Ray), typeof(Mesh), typeof(Matrix4x4), typeof(RaycastHit).MakeByRefType() },
                    null);
                if (_intersectRayMesh == null)
                    return false;
            }

            int frame = Time.frameCount;
            if (_cachedRenderersFrame != frame || _cachedRenderers == null)
            {
#if UNITY_6000_4_OR_NEWER
                _cachedRenderers = FindObjectsByType<MeshRenderer>();
#else
                _cachedRenderers = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
#endif
                _cachedRenderersFrame = frame;
            }

            Vector3 cameraPos = sceneCamera.transform.position;
            Vector3 toTarget = targetPosition - cameraPos;
            float targetDistance = toTarget.magnitude;
            if (targetDistance <= 0.0001f)
                return false;

            Ray ray = new Ray(cameraPos, toTarget / targetDistance);
            object[] args = new object[4];
            args[0] = ray;

            foreach (MeshRenderer renderer in _cachedRenderers)
            {
                if (renderer == null || !renderer.enabled || renderer.gameObject == gameObject)
                    continue;
                if (!renderer.bounds.IntersectRay(ray, out float boundsDistance) || boundsDistance >= targetDistance)
                    continue;

                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                if (meshFilter == null || meshFilter.sharedMesh == null)
                    continue;

                args[1] = meshFilter.sharedMesh;
                args[2] = meshFilter.transform.localToWorldMatrix;
                args[3] = null;
                if (!(bool)_intersectRayMesh.Invoke(null, args))
                    continue;

                if (((RaycastHit)args[3]).distance < targetDistance - 0.001f)
                    return true;
            }

            return false;
        }
#endif
    }
}
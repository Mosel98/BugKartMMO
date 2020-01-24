/****************************************
 * Copyright © Jan Schmitz 2018         *
 * Date: 12/25/2018 2:24:35 PM			*
 ****************************************/

using UnityEngine;
using UnityEditor;

namespace UI.Editor
{
    [InitializeOnLoad]
    public class SetAnchors
    {
        private const string PATH = "Tools/UI/Auto Set Anchors";
        private static bool s_isActive = true;

        private static GameObject s_activeObject;
        private static RectTransform s_activeTransform;

        static SetAnchors()
        {
            s_isActive = EditorPrefs.GetBool(PATH, true);
            EditorApplication.delayCall += SetMarkDelayed;
        }

        private static void SetMarkDelayed()
        {
            Menu.SetChecked(PATH, s_isActive);
            SceneView.duringSceneGui -= SetAnchorPositions;
            if (s_isActive)
            {
                SceneView.duringSceneGui += SetAnchorPositions;
            }
        }

        [MenuItem(PATH)]
        private static void Toggle()
        {
            s_isActive = !s_isActive;
            EditorPrefs.SetBool(PATH, s_isActive);
            Menu.SetChecked(PATH, s_isActive);

            SceneView.duringSceneGui -= SetAnchorPositions;
            if (s_isActive)
            {
                SceneView.duringSceneGui += SetAnchorPositions;
            }
        }

        private static void SetAnchorPositions(SceneView sceneView)
        {
            if (s_activeObject != Selection.activeGameObject)
            {
                s_activeObject = Selection.activeGameObject;
                if (null == s_activeObject)
                {
                    return;
                }
                s_activeTransform = s_activeObject.GetComponent<RectTransform>();
            }
            if (null == s_activeTransform)
            {
                return;
            }

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event e = Event.current;
            if (e.GetTypeForControl(controlID) == EventType.MouseUp && e.isMouse)
            {
                if (e.button == 0)
                {
                    SingleSnap(s_activeTransform);
                }
            }
        }

        private static void SingleSnap(RectTransform _transform)
        {
            RectTransform parent = _transform.parent as RectTransform;

            if (parent == null)
            {
                return;
            }

            Vector2 vectorMin = new Vector2(_transform.anchorMin.x + _transform.offsetMin.x / parent.rect.width,
                                            _transform.anchorMin.y + _transform.offsetMin.y / parent.rect.height);
            Vector2 vectorMax = new Vector2(_transform.anchorMax.x + _transform.offsetMax.x / parent.rect.width,
                                            _transform.anchorMax.y + _transform.offsetMax.y / parent.rect.height);
            _transform.anchorMin = vectorMin;
            _transform.anchorMax = vectorMax;

            _transform.offsetMin = Vector2.zero;
            _transform.offsetMax = Vector2.zero;
        }
    }
}

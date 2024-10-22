using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    using Settings = MotionBlurModel.Settings;

    [PostProcessingModelEditor(typeof(MotionBlurModel))]
    public class MotionBlurModelEditor : PostProcessingModelEditor
    {
        private SerializedProperty m_ShutterAngle;
        private SerializedProperty m_SampleCount;
        private SerializedProperty m_FrameBlending;
        private GraphDrawer m_GraphDrawer;

        private class GraphDrawer
        {
            private const float k_Height = 32f;
            private readonly Texture m_BlendingIcon;
            private readonly GUIStyle m_LowerCenterStyle;
            private readonly GUIStyle m_MiddleCenterStyle;
            private Color m_ColorDark;
            private Color m_ColorGray;
            private readonly Vector3[] m_RectVertices = new Vector3[4];

            public GraphDrawer()
            {
                m_BlendingIcon = EditorResources.Load<Texture>("UI/MotionBlendingIcon.png");

                m_LowerCenterStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.LowerCenter };
                m_MiddleCenterStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };

                if (EditorGUIUtility.isProSkin)
                {
                    m_ColorDark = new Color(0.18f, 0.18f, 0.18f);
                    m_ColorGray = new Color(0.43f, 0.43f, 0.43f);
                }
                else
                {
                    m_ColorDark = new Color(0.64f, 0.64f, 0.64f);
                    m_ColorGray = new Color(0.92f, 0.92f, 0.92f);
                }
            }

            public void DrawShutterGraph(float angle)
            {
                Vector2 center = GUILayoutUtility.GetRect(128, k_Height).center;

                // Parameters used to make transitions smooth.
                float zeroWhenOff = Mathf.Min(1f, angle * 0.1f);
                float zeroWhenFull = Mathf.Min(1f, (360f - angle) * 0.02f);

                // Shutter angle graph
                Vector2 discCenter = center - new Vector2(k_Height * 2.4f, 0f);
                // - exposure duration indicator
                DrawDisc(discCenter, k_Height * Mathf.Lerp(0.5f, 0.38f, zeroWhenFull), m_ColorGray);
                // - shutter disc
                DrawDisc(discCenter, k_Height * 0.16f * zeroWhenFull, m_ColorDark);
                // - shutter blade
                DrawArc(discCenter, k_Height * 0.5f, 360f - angle, m_ColorDark);
                // - shutter axis
                DrawDisc(discCenter, zeroWhenOff, m_ColorGray);

                // Shutter label (off/full)
                Vector2 labelSize = new Vector2(k_Height, k_Height);
                Vector2 labelOrigin = discCenter - labelSize * 0.5f;
                Rect labelRect = new Rect(labelOrigin, labelSize);

                if (Mathf.Approximately(angle, 0f))
                {
                    GUI.Label(labelRect, "Off", m_MiddleCenterStyle);
                }
                else if (Mathf.Approximately(angle, 360f))
                {
                    GUI.Label(labelRect, "Full", m_MiddleCenterStyle);
                }

                // Exposure time bar graph
                Vector2 outerBarSize = new Vector2(4.75f, 0.5f) * k_Height;
                Vector2 innerBarSize = outerBarSize;
                innerBarSize.x *= angle / 360f;

                Vector2 barCenter = center + new Vector2(k_Height * 0.9f, 0f);
                Vector2 barOrigin = barCenter - outerBarSize * 0.5f;

                DrawRect(barOrigin, outerBarSize, m_ColorDark);
                DrawRect(barOrigin, innerBarSize, m_ColorGray);

                string barText = "Exposure time = " + (angle / 3.6f).ToString("0") + "% of ΔT";
                GUI.Label(new Rect(barOrigin, outerBarSize), barText, m_MiddleCenterStyle);
            }

            public void DrawBlendingGraph(float strength)
            {
                Vector2 center = GUILayoutUtility.GetRect(128, k_Height).center;

                Vector2 iconSize = new Vector2(k_Height, k_Height);
                Vector2 iconStride = new Vector2(k_Height * 0.9f, 0f);
                Vector2 iconOrigin = center - iconSize * 0.5f - iconStride * 2f;

                for (int i = 0; i < 5; i++)
                {
                    float weight = BlendingWeight(strength, i / 60f);
                    Rect rect = new Rect(iconOrigin + iconStride * i, iconSize);

                    Color color = m_ColorGray;
                    color.a = weight;

                    GUI.color = color;
                    GUI.Label(rect, m_BlendingIcon);

                    GUI.color = Color.white;
                    GUI.Label(rect, (weight * 100).ToString("0") + "%", m_LowerCenterStyle);
                }
                // EditorGUIUtility.isProSkin
            }

            // Weight function for multi frame blending
            private float BlendingWeight(float strength, float time)
            {
                if (strength > 0f || Mathf.Approximately(time, 0f))
                {
                    return Mathf.Exp(-time * Mathf.Lerp(80f, 10f, strength));
                }

                return 0;
            }

            // Draw a solid disc in the graph rect.
            private void DrawDisc(Vector2 center, float radius, Color fill)
            {
                Handles.color = fill;
                Handles.DrawSolidDisc(center, Vector3.forward, radius);
            }

            // Draw an arc in the graph rect.
            private void DrawArc(Vector2 center, float radius, float angle, Color fill)
            {
                Vector2 start = new Vector2(
                        -Mathf.Cos(Mathf.Deg2Rad * angle / 2f),
                        Mathf.Sin(Mathf.Deg2Rad * angle / 2f)
                        );

                Handles.color = fill;
                Handles.DrawSolidArc(center, Vector3.forward, start, angle, radius);
            }

            // Draw a rectangle in the graph rect.
            private void DrawRect(Vector2 origin, Vector2 size, Color color)
            {
                Vector2 p0 = origin;
                Vector2 p1 = origin + size;

                m_RectVertices[0] = p0;
                m_RectVertices[1] = new Vector2(p1.x, p0.y);
                m_RectVertices[2] = p1;
                m_RectVertices[3] = new Vector2(p0.x, p1.y);

                Handles.color = Color.white;
                Handles.DrawSolidRectangleWithOutline(m_RectVertices, color, Color.clear);
            }
        }

        public override void OnEnable()
        {
            m_ShutterAngle = FindSetting((Settings x) => x.shutterAngle);
            m_SampleCount = FindSetting((Settings x) => x.sampleCount);
            m_FrameBlending = FindSetting((Settings x) => x.frameBlending);
        }

        public override void OnInspectorGUI()
        {
            if (m_GraphDrawer == null)
            {
                m_GraphDrawer = new GraphDrawer();
            }

            EditorGUILayout.LabelField("Shutter Speed Simulation", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            m_GraphDrawer.DrawShutterGraph(m_ShutterAngle.floatValue);
            EditorGUILayout.PropertyField(m_ShutterAngle);
            EditorGUILayout.PropertyField(m_SampleCount);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Multiple Frame Blending", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            float fbValue = m_FrameBlending.floatValue;
            m_GraphDrawer.DrawBlendingGraph(fbValue);
            EditorGUILayout.PropertyField(m_FrameBlending);

            if (fbValue > 0f)
            {
                EditorGUILayout.HelpBox("Multi-Frame Blending lowers precision of the final picture for optimization purposes.", MessageType.Info);
            }

            EditorGUI.indentLevel--;
        }
    }
}

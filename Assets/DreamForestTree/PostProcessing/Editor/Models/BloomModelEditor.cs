using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    using Settings = BloomModel.Settings;

    [PostProcessingModelEditor(typeof(BloomModel))]
    public class BloomModelEditor : PostProcessingModelEditor
    {
        private struct BloomSettings
        {
            public SerializedProperty intensity;
            public SerializedProperty threshold;
            public SerializedProperty softKnee;
            public SerializedProperty radius;
            public SerializedProperty antiFlicker;
        }

        private struct LensDirtSettings
        {
            public SerializedProperty texture;
            public SerializedProperty intensity;
        }

        private BloomSettings m_Bloom;
        private LensDirtSettings m_LensDirt;

        public override void OnEnable()
        {
            m_Bloom = new BloomSettings
            {
                intensity = FindSetting((Settings x) => x.bloom.intensity),
                threshold = FindSetting((Settings x) => x.bloom.threshold),
                softKnee = FindSetting((Settings x) => x.bloom.softKnee),
                radius = FindSetting((Settings x) => x.bloom.radius),
                antiFlicker = FindSetting((Settings x) => x.bloom.antiFlicker)
            };

            m_LensDirt = new LensDirtSettings
            {
                texture = FindSetting((Settings x) => x.lensDirt.texture),
                intensity = FindSetting((Settings x) => x.lensDirt.intensity)
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            PrepareGraph();
            DrawGraph();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_Bloom.intensity);
            EditorGUILayout.PropertyField(m_Bloom.threshold, EditorGUIHelper.GetContent("Threshold (Gamma)"));
            EditorGUILayout.PropertyField(m_Bloom.softKnee);
            EditorGUILayout.PropertyField(m_Bloom.radius);
            EditorGUILayout.PropertyField(m_Bloom.antiFlicker);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dirt", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_LensDirt.texture);
            EditorGUILayout.PropertyField(m_LensDirt.intensity);
            EditorGUI.indentLevel--;
        }

        #region Graph

        private float m_GraphThreshold;
        private float m_GraphKnee;
        private float m_GraphIntensity;

        // Number of vertices in curve
        private const int k_CurveResolution = 48;

        // Vertex buffers
        private readonly Vector3[] m_RectVertices = new Vector3[4];
        private readonly Vector3[] m_LineVertices = new Vector3[2];
        private readonly Vector3[] m_CurveVertices = new Vector3[k_CurveResolution];
        private Rect m_RectGraph;
        private float m_RangeX;
        private float m_RangeY;

        private float ResponseFunction(float x)
        {
            float rq = Mathf.Clamp(x - m_GraphThreshold + m_GraphKnee, 0, m_GraphKnee * 2);
            rq = rq * rq * 0.25f / m_GraphKnee;
            return Mathf.Max(rq, x - m_GraphThreshold) * m_GraphIntensity;
        }

        // Transform a point into the graph rect
        private Vector3 PointInRect(float x, float y)
        {
            x = Mathf.Lerp(m_RectGraph.x, m_RectGraph.xMax, x / m_RangeX);
            y = Mathf.Lerp(m_RectGraph.yMax, m_RectGraph.y, y / m_RangeY);
            return new Vector3(x, y, 0);
        }

        // Draw a line in the graph rect
        private void DrawLine(float x1, float y1, float x2, float y2, float grayscale)
        {
            m_LineVertices[0] = PointInRect(x1, y1);
            m_LineVertices[1] = PointInRect(x2, y2);
            Handles.color = Color.white * grayscale;
            Handles.DrawAAPolyLine(2.0f, m_LineVertices);
        }

        // Draw a rect in the graph rect
        private void DrawRect(float x1, float y1, float x2, float y2, float fill, float line)
        {
            m_RectVertices[0] = PointInRect(x1, y1);
            m_RectVertices[1] = PointInRect(x2, y1);
            m_RectVertices[2] = PointInRect(x2, y2);
            m_RectVertices[3] = PointInRect(x1, y2);

            Handles.DrawSolidRectangleWithOutline(
                m_RectVertices,
                fill < 0 ? Color.clear : Color.white * fill,
                line < 0 ? Color.clear : Color.white * line
                );
        }

        // Update internal state with a given bloom instance
        public void PrepareGraph()
        {
            BloomModel bloom = (BloomModel)target;
            m_RangeX = 5f;
            m_RangeY = 2f;

            m_GraphThreshold = bloom.settings.bloom.thresholdLinear;
            m_GraphKnee = bloom.settings.bloom.softKnee * m_GraphThreshold + 1e-5f;

            // Intensity is capped to prevent sampling errors
            m_GraphIntensity = Mathf.Min(bloom.settings.bloom.intensity, 10f);
        }

        // Draw the graph at the current position
        public void DrawGraph()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(EditorGUI.indentLevel * 15f);
                m_RectGraph = GUILayoutUtility.GetRect(128, 80);
            }

            // Background
            DrawRect(0, 0, m_RangeX, m_RangeY, 0.1f, 0.4f);

            // Soft-knee range
            DrawRect(m_GraphThreshold - m_GraphKnee, 0, m_GraphThreshold + m_GraphKnee, m_RangeY, 0.25f, -1);

            // Horizontal lines
            for (int i = 1; i < m_RangeY; i++)
            {
                DrawLine(0, i, m_RangeX, i, 0.4f);
            }

            // Vertical lines
            for (int i = 1; i < m_RangeX; i++)
            {
                DrawLine(i, 0, i, m_RangeY, 0.4f);
            }

            // Label
            Handles.Label(
                PointInRect(0, m_RangeY) + Vector3.right,
                "Brightness Response (linear)", EditorStyles.miniLabel
                );

            // Threshold line
            DrawLine(m_GraphThreshold, 0, m_GraphThreshold, m_RangeY, 0.6f);

            // Response curve
            int vcount = 0;
            while (vcount < k_CurveResolution)
            {
                float x = m_RangeX * vcount / (k_CurveResolution - 1);
                float y = ResponseFunction(x);
                if (y < m_RangeY)
                {
                    m_CurveVertices[vcount++] = PointInRect(x, y);
                }
                else
                {
                    if (vcount > 1)
                    {
                        // Extend the last segment to the top edge of the rect.
                        Vector3 v1 = m_CurveVertices[vcount - 2];
                        Vector3 v2 = m_CurveVertices[vcount - 1];
                        float clip = (m_RectGraph.y - v1.y) / (v2.y - v1.y);
                        m_CurveVertices[vcount - 1] = v1 + (v2 - v1) * clip;
                    }
                    break;
                }
            }

            if (vcount > 1)
            {
                Handles.color = Color.white * 0.9f;
                Handles.DrawAAPolyLine(2.0f, vcount, m_CurveVertices);
            }
        }

        #endregion
    }
}

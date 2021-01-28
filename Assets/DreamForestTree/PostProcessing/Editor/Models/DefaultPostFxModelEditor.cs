using System.Collections.Generic;

namespace UnityEditor.PostProcessing
{
    public class DefaultPostFxModelEditor : PostProcessingModelEditor
    {
        private readonly List<SerializedProperty> m_Properties = new List<SerializedProperty>();

        public override void OnEnable()
        {
            System.Collections.IEnumerator iter = m_SettingsProperty.Copy().GetEnumerator();
            while (iter.MoveNext())
            {
                m_Properties.Add(((SerializedProperty)iter.Current).Copy());
            }
        }

        public override void OnInspectorGUI()
        {
            foreach (SerializedProperty property in m_Properties)
            {
                EditorGUILayout.PropertyField(property);
            }
        }
    }
}

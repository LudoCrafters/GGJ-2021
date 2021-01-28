using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    [CustomPropertyDrawer(typeof(GetSetAttribute))]
    internal sealed class GetSetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GetSetAttribute attribute = (GetSetAttribute)base.attribute;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);

            if (EditorGUI.EndChangeCheck())
            {
                attribute.dirty = true;
            }
            else if (attribute.dirty)
            {
                object parent = ReflectionUtils.GetParentObject(property.propertyPath, property.serializedObject.targetObject);

                System.Type type = parent.GetType();
                System.Reflection.PropertyInfo info = type.GetProperty(attribute.name);

                if (info == null)
                {
                    Debug.LogError("Invalid property name \"" + attribute.name + "\"");
                }
                else
                {
                    info.SetValue(parent, fieldInfo.GetValue(parent), null);
                }

                attribute.dirty = false;
            }
        }
    }
}

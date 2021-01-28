using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    //[CanEditMultipleObjects]
    [CustomEditor(typeof(PostProcessingProfile))]
    public class PostProcessingInspector : Editor
    {
        private static readonly GUIContent s_PreviewTitle = new GUIContent("Monitors");

        private PostProcessingProfile m_ConcreteTarget => target as PostProcessingProfile;

        private int m_CurrentMonitorID
        {
            get => m_ConcreteTarget.monitors.currentMonitorID;
            set => m_ConcreteTarget.monitors.currentMonitorID = value;
        }

        private List<PostProcessingMonitor> m_Monitors;
        private GUIContent[] m_MonitorNames;
        private readonly Dictionary<PostProcessingModelEditor, PostProcessingModel> m_CustomEditors = new Dictionary<PostProcessingModelEditor, PostProcessingModel>();

        public bool IsInteractivePreviewOpened { get; private set; }

        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }

            // Aggregate custom post-fx editors
            Assembly assembly = Assembly.GetAssembly(typeof(PostProcessingInspector));

            IEnumerable<Type> editorTypes = assembly.GetTypes()
                .Where(x => x.IsDefined(typeof(PostProcessingModelEditorAttribute), false));

            Dictionary<Type, PostProcessingModelEditor> customEditors = new Dictionary<Type, PostProcessingModelEditor>();
            foreach (Type editor in editorTypes)
            {
                PostProcessingModelEditorAttribute attr = (PostProcessingModelEditorAttribute)editor.GetCustomAttributes(typeof(PostProcessingModelEditorAttribute), false)[0];
                Type effectType = attr.type;
                bool alwaysEnabled = attr.alwaysEnabled;

                PostProcessingModelEditor editorInst = (PostProcessingModelEditor)Activator.CreateInstance(editor);
                editorInst.alwaysEnabled = alwaysEnabled;
                editorInst.profile = target as PostProcessingProfile;
                editorInst.inspector = this;
                customEditors.Add(effectType, editorInst);
            }

            // ... and corresponding models
            Type baseType = target.GetType();
            SerializedProperty property = serializedObject.GetIterator();

            while (property.Next(true))
            {
                if (!property.hasChildren)
                {
                    continue;
                }

                Type type = baseType;
                object srcObject = ReflectionUtils.GetFieldValueFromPath(serializedObject.targetObject, ref type, property.propertyPath);

                if (srcObject == null)
                {
                    continue;
                }

                if (customEditors.TryGetValue(type, out PostProcessingModelEditor editor))
                {
                    PostProcessingModel effect = (PostProcessingModel)srcObject;

                    if (editor.alwaysEnabled)
                    {
                        effect.enabled = editor.alwaysEnabled;
                    }

                    m_CustomEditors.Add(editor, effect);
                    editor.target = effect;
                    editor.serializedProperty = property.Copy();
                    editor.OnPreEnable();
                }
            }

            // Prepare monitors
            m_Monitors = new List<PostProcessingMonitor>();

            List<PostProcessingMonitor> monitors = new List<PostProcessingMonitor>
            {
                new HistogramMonitor(),
                new WaveformMonitor(),
                new ParadeMonitor(),
                new VectorscopeMonitor()
            };

            List<GUIContent> monitorNames = new List<GUIContent>();

            foreach (PostProcessingMonitor monitor in monitors)
            {
                if (monitor.IsSupported())
                {
                    monitor.Init(m_ConcreteTarget.monitors, this);
                    m_Monitors.Add(monitor);
                    monitorNames.Add(monitor.GetMonitorTitle());
                }
            }

            m_MonitorNames = monitorNames.ToArray();

            if (m_Monitors.Count > 0)
            {
                m_ConcreteTarget.monitors.onFrameEndEditorOnly = OnFrameEnd;
            }
        }

        private void OnDisable()
        {
            if (m_CustomEditors != null)
            {
                foreach (PostProcessingModelEditor editor in m_CustomEditors.Keys)
                {
                    editor.OnDisable();
                }

                m_CustomEditors.Clear();
            }

            if (m_Monitors != null)
            {
                foreach (PostProcessingMonitor monitor in m_Monitors)
                {
                    monitor.Dispose();
                }

                m_Monitors.Clear();
            }

            if (m_ConcreteTarget != null)
            {
                m_ConcreteTarget.monitors.onFrameEndEditorOnly = null;
            }
        }

        private void OnFrameEnd(RenderTexture source)
        {
            if (!IsInteractivePreviewOpened)
            {
                return;
            }

            if (m_CurrentMonitorID < m_Monitors.Count)
            {
                m_Monitors[m_CurrentMonitorID].OnFrameData(source);
            }

            IsInteractivePreviewOpened = false;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Handles undo/redo events first (before they get used by the editors' widgets)
            Event e = Event.current;
            if (e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed")
            {
                foreach (KeyValuePair<PostProcessingModelEditor, PostProcessingModel> editor in m_CustomEditors)
                {
                    editor.Value.OnValidate();
                }
            }

            if (!m_ConcreteTarget.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.None))
            {
                EditorGUILayout.HelpBox("A debug view is currently enabled. Changes done to an effect might not be visible.", MessageType.Info);
            }

            foreach (KeyValuePair<PostProcessingModelEditor, PostProcessingModel> editor in m_CustomEditors)
            {
                EditorGUI.BeginChangeCheck();

                editor.Key.OnGUI();

                if (EditorGUI.EndChangeCheck())
                {
                    editor.Value.OnValidate();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override GUIContent GetPreviewTitle()
        {
            return s_PreviewTitle;
        }

        public override bool HasPreviewGUI()
        {
            return GraphicsUtils.supportsDX11 && m_Monitors.Count > 0;
        }

        public override void OnPreviewSettings()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (m_CurrentMonitorID < m_Monitors.Count)
                {
                    m_Monitors[m_CurrentMonitorID].OnMonitorSettings();
                }

                GUILayout.Space(5);
                m_CurrentMonitorID = EditorGUILayout.Popup(m_CurrentMonitorID, m_MonitorNames, FxStyles.preDropdown, GUILayout.MaxWidth(100f));
            }
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            IsInteractivePreviewOpened = true;

            if (m_CurrentMonitorID < m_Monitors.Count)
            {
                m_Monitors[m_CurrentMonitorID].OnMonitorGUI(r);
            }
        }
    }
}

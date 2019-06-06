using IVRCommon.Keyboard.Widet;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(KeyCodeButton), false)]
[CanEditMultipleObjects]
public class KeyCodeButtonEditor : ButtonEditor
{
    SerializedProperty m_LabelProperty;
    SerializedProperty m_TypeProperty;
    SerializedProperty m_NormalColorProperty;
    SerializedProperty m_HoverColorProperty;
    SerializedProperty m_callFunProperty;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_LabelProperty = serializedObject.FindProperty("label");
        m_TypeProperty = serializedObject.FindProperty("type");
        m_NormalColorProperty = serializedObject.FindProperty("normalColor");
        m_HoverColorProperty = serializedObject.FindProperty("hoverColor");
        m_callFunProperty = serializedObject.FindProperty("callFun");
    } 

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_TypeProperty);
        EditorGUILayout.PropertyField(m_LabelProperty);
        EditorGUILayout.PropertyField(m_NormalColorProperty);
        EditorGUILayout.PropertyField(m_HoverColorProperty);
        EditorGUILayout.PropertyField(m_callFunProperty);
        serializedObject.ApplyModifiedProperties();
    }
}

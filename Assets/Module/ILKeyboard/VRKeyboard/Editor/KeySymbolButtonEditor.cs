using IVRCommon.Keyboard.Widet;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(KeySymbolButton), false)]
[CanEditMultipleObjects]
public class KeySymbolButtonEditor : KeyCodeButtonEditor
{
    SerializedProperty m_ImageProperty;
    SerializedProperty m_AddImageProperty;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_ImageProperty = serializedObject.FindProperty("lowerIcon");
        m_AddImageProperty = serializedObject.FindProperty("upperIcon");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_ImageProperty);
        EditorGUILayout.PropertyField(m_AddImageProperty);
        serializedObject.ApplyModifiedProperties();
    }
}

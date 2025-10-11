using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BlackboardSystem
{
    [CustomEditor(typeof(BlackboardData))]
    public class BlackboardDataEditor : Editor
    {
        ReorderableList entryList;

        private void OnEnable()
        {
            entryList = new ReorderableList(serializedObject, serializedObject.FindProperty("entries"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight), "Key");
                    EditorGUI.LabelField(new Rect(rect.x = rect.width * 0.3f + 10, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight), "Type");
                    EditorGUI.LabelField(new Rect(rect.x = rect.width * 0.6f + 5, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight), "Value");

                }
            };

            entryList.drawElementCallback = (rect, index, isActive, isFocused) => {
                var element = entryList.serializedProperty.GetArrayElementAtIndex(index);

                rect.y += 2;
                var keyName = element.FindPropertyRelative("keyName");
                var type = element.FindPropertyRelative("type");
                var value = element.FindPropertyRelative("value");

                var keyNameRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                var valueTypeRect = new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                var valueRect = new Rect(rect.x + rect.width * 0.6f, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(keyNameRect, keyName, GUIContent.none);
                EditorGUI.PropertyField(valueTypeRect, type, GUIContent.none);

                switch ((AnyValue.Type)type.enumValueIndex)
                {
                    case AnyValue.Type.Int:
                        var intValue = value.FindPropertyRelative("intType");
                        EditorGUI.PropertyField(valueRect, intValue, GUIContent.none);
                        break;
                    case AnyValue.Type.Float:
                        var floatValue = value.FindPropertyRelative("floatType");
                        EditorGUI.PropertyField(valueRect, floatValue, GUIContent.none);
                        break;
                    case AnyValue.Type.Bool:
                        var boolValue = value.FindPropertyRelative("boolType");
                        EditorGUI.PropertyField(valueRect, boolValue, GUIContent.none);
                        break;
                    case AnyValue.Type.String:
                        var stringValue = value.FindPropertyRelative("stringType");
                        EditorGUI.PropertyField(valueRect, stringValue, GUIContent.none);
                        break;
                    case AnyValue.Type.Vector3:
                        var vector3Value = value.FindPropertyRelative("vector3Type");
                        EditorGUI.PropertyField(valueRect, vector3Value, GUIContent.none);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            entryList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}

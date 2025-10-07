using External;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

    [CustomPropertyDrawer(typeof(BaseUtils.WeightedList<>))]
    public class WeightedListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Find the 'items' list property within your custom class
            SerializedProperty listProperty = property.FindPropertyRelative("list");

            // Use EditorGUI.PropertyField to draw the list
            // You can customize how the list is drawn here
            EditorGUI.PropertyField(position, listProperty, label, true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Calculate the height needed for the list
            SerializedProperty listProperty = property.FindPropertyRelative("list");
            return EditorGUI.GetPropertyHeight(listProperty, label, true);
        }
    }
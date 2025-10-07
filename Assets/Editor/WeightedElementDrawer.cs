using External;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

    [CustomPropertyDrawer(typeof(BaseUtils.WeightedElement<>))]
    public class WeightedElementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Example: Dividing into two equal halves
            Rect leftHalf = new Rect(position.x, position.y, position.width / 2f, position.height);
            Rect rightHalf = new Rect(position.x + position.width / 2f, position.y, position.width / 2f, position.height);
            
            EditorGUI.PropertyField(leftHalf, property.FindPropertyRelative("value"), GUIContent.none);
            EditorGUI.PropertyField(rightHalf, property.FindPropertyRelative("weight"), GUIContent.none);
        }
    }

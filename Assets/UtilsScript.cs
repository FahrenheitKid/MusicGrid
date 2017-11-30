using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace utils
{
    [System.Serializable]
    public class AnimationCurve3
    {
        public AnimationCurve x;
        public AnimationCurve y;
        public AnimationCurve z;
    }

    [CustomPropertyDrawer(typeof(MatrixData))]
    public class CustomMatrixData : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PrefixLabel(position, label);
            Rect newPosition = position;
            newPosition.y += 18f;

            SerializedProperty rows = property.FindPropertyRelative("rows");

            for (int i = 0; i < 10; i++)
            {
                SerializedProperty row = rows.GetArrayElementAtIndex(i).FindPropertyRelative("row");

                newPosition.height = 20;
                newPosition.width = 20;

                EditorGUI.PrefixLabel(newPosition, new GUIContent(i.ToString()));
                newPosition.x += 15;
                for (int j = 0; j < 10; j++)
                {
                    EditorGUI.PropertyField(newPosition, row.GetArrayElementAtIndex(j), GUIContent.none);
                    newPosition.x += newPosition.width + 2.5f;
                }

                newPosition.x = position.x;
                newPosition.y += newPosition.height + 2.5f;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20 * 12;
        }
    }

    [System.Serializable]
    public class MatrixData
    {
        [System.Serializable]
        public struct rowData
        {
            public int[] row;
        }

        public rowData[] rows = new rowData[10];
    }
}
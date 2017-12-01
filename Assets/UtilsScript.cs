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
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MatrixData))]
    public class CustomMatrixData : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIStyle style = new GUIStyle();

            EditorGUI.PrefixLabel(position, label);

            Rect newPosition = position;
            newPosition.y += 18f;

            SerializedProperty rows = property.FindPropertyRelative("rows");

            for (int i = 0; i < rows.arraySize; i++)
            {
                SerializedProperty row = rows.GetArrayElementAtIndex(i).FindPropertyRelative("row");

                newPosition.height = 20;
                newPosition.width = 20;

                //EditorGUI.PrefixLabel(newPosition, new GUIContent(i.ToString()));
                EditorGUI.TextArea(newPosition, i.ToString(), new GUIStyle());
                newPosition.x += 15;

                for (int j = 0; j < row.arraySize; j++)
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
            int i = property.FindPropertyRelative("rows").arraySize;
            if (i > 0)
                return (i * 20) + 40;
            else return 12;
        }
    }
#endif
    [System.Serializable]
    public class MatrixData
    {
        [System.Serializable]
        public struct rowData
        {
            public int[] row;
        }

        public rowData[] rows;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">Lines</param>
        /// <param name="y">Columns</param>
        public MatrixData(int x, int y)
        {
            rows = new rowData[x];

            for (int i = 0; i < rows.Length; i++)
                rows[i].row = new int[y];
        }
    }
}
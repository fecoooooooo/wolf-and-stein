using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    public Object mapData;
    string levelStr = "1";

    public override void OnInspectorGUI()
    {
        Map map = (Map)target;

        DrawDefaultInspector();

        GUILayout.Label("Map data (number of level):");
        levelStr = EditorGUILayout.TextField(levelStr);

        if (GUILayout.Button("Generate"))
        {
            int level = int.Parse(levelStr);
            map.Generate(level);
        }
    }
}

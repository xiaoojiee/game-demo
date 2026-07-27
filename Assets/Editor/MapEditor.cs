using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        if (GUILayout.Button("MakerMap"))
        {
            ((Map)target).MakerMap();
        }
        if (GUILayout.Button("CleanMap"))
        {
            ((Map)target).CleanMap();
        }
    }
}

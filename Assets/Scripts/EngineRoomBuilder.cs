using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(EngineRoom))]
public class EngineRoomBuilder : Editor {


    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Build")) {
            Build();
        }

    }

    void Build() {
        Debug.Log("Building");
    }
}

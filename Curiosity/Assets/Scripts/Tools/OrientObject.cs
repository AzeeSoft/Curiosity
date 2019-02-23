using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class OrientObject : EditorWindow
{
    [MenuItem("Window/Orient Object")]
    public static void ShowWindow()
    {
        GetWindow<OrientObject>("Orient Object");
    }

    private void OnGUI()
    {
        GUILayout.Label("Orient Towards Object", EditorStyles.boldLabel);

        if (GUILayout.Button("Orient"))
        {
            Orient();
        }
    }

    private void Orient()
    {
        Debug.Log("Orienting..");
        GameObject[] selectedObjects = Selection.gameObjects;
        //selectedObjects[0].transform.rot = new Vector3(0,1,1);
        selectedObjects[1].transform.LookAt(selectedObjects[0].transform, Vector3.up);
        //selectedObjects[1].transform.rotation = Quaternion.LookRotation(selectedObjects[0].transform.position);
        Debug.Log("Oriented: " + selectedObjects[0].name + " towards " + selectedObjects[1].name);
    }
}

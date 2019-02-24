using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class RiggingTools : EditorWindow
{
    [MenuItem("Window/Rigging Tools")]
    public static void ShowWindow()
    {
        GetWindow<RiggingTools>("Rigging Tools");
    }

    string prefix = "Blank";
    string find = "Blank";
    string replace = "Blank";

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        GUILayout.Label("Orient Towards Object", EditorStyles.boldLabel);

        if (GUILayout.Button("Orient"))
        {
            Orient();
        }

        GUILayout.Label("Copy Position", EditorStyles.boldLabel);

        if (GUILayout.Button("Copy Position"))
        {
            Copy();
        }

        GUILayout.Label("Add Prefix to Selection", EditorStyles.boldLabel);
        
        prefix = EditorGUILayout.TextField("New Prefix", prefix);

        if (GUILayout.Button("Add Prefix"))
        {
            
            AddPrefix(prefix);
        }

        GUILayout.Label("Find And Replace Name", EditorStyles.boldLabel);

        find = EditorGUILayout.TextField("Find", find);
        replace = EditorGUILayout.TextField("Replace", replace);

        if (GUILayout.Button("Find And Replace"))
        {

            FindAndReplace(find, replace);
        }

    }

    private void Orient()
    {
        Debug.Log("Orienting..");


        GameObject[] selectedObjects = Selection.gameObjects;

        Undo.RecordObject(selectedObjects[1].transform, "Changed Orient");

        //selectedObjects[0].transform.rot = new Vector3(0,1,1);
        selectedObjects[0].transform.LookAt(selectedObjects[1].transform, Vector3.up);
        //selectedObjects[1].transform.rotation = Quaternion.LookRotation(selectedObjects[0].transform.position);
        Debug.Log("Oriented: " + selectedObjects[0].name + " towards " + selectedObjects[1].name);
    }

    private void Copy()
    {

        GameObject[] selectedObjects = Selection.gameObjects;

        Undo.RecordObject(selectedObjects[1].transform, "Changed Position");

        //selectedObjects[0].transform.rot = new Vector3(0,1,1);
        selectedObjects[0].transform.position = selectedObjects[1].transform.position;

        Debug.Log("Moved: " + selectedObjects[0].name + " towards " + selectedObjects[1].name);
    }

    private void AddPrefix(string _Prefix)
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            Undo.RecordObject(selectedObjects[i].gameObject, "Changed Name");
            selectedObjects[i].name = _Prefix + selectedObjects[i].name;
        }
    }

    private void FindAndReplace(string _find, string _replace)
    {
        Debug.Log("Finding and Replacing..");
        GameObject[] selectedObjects = Selection.gameObjects;

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            Undo.RecordObject(selectedObjects[i].gameObject, "Changed Name");
            string name = selectedObjects[i].name;
            string newName = name.Replace(_find, _replace);
            selectedObjects[i].name = newName;
            Debug.Log("Replaced " + _find + " with " + _replace);
        }
    }
}

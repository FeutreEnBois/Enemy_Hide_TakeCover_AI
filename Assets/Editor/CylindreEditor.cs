using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cylindre))]
public class CylindreEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Color"))
        {
            Debug.Log("PRESSED");
        }
    }
}

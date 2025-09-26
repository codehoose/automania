using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CounterHudController))]
public class CounterHudControllerEditor : Editor
{
    int counter;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var scoreController = (CounterHudController)target;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Counter (Debug)");
        counter = EditorGUILayout.IntField(counter);
        if (GUILayout.Button("Update"))
        {
            scoreController.Counter = counter;
        }
        EditorGUILayout.EndHorizontal();
    }
}

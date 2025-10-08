using UnityEditor;


[CustomEditor(typeof(TimeHudController))]
public class TimeHudControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var timeController = (TimeHudController)target;
        if (!timeController) return;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Percent (Debug)");
        var percent = timeController.Current;
        percent = EditorGUILayout.Slider(percent, 0, 1f);
        timeController.Current = percent;
        EditorGUILayout.EndHorizontal();
    }
}

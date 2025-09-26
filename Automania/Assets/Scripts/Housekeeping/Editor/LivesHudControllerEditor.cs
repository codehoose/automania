using UnityEditor;

[CustomEditor(typeof(LivesHudController))]
public class LivesHudControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        
        var livesController = (LivesHudController)target;
        var lives = livesController.Lives;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Lives (Debug)");
        var newLives = EditorGUILayout.IntField(lives);
        EditorGUILayout.EndHorizontal();

        if (newLives != lives)
        {
            livesController.Lives = newLives;
        }
    }
}

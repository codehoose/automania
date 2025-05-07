using UnityEngine;

[CreateAssetMenu(fileName = "ScreenObject", menuName = "Automania/Screen Object")]
public class ScreenObjectPosition : ScriptableObject
{
    public GameObject prefab;
    public Vector3 position;
}

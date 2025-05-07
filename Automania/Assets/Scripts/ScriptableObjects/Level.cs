using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Automania/Level")]
public class Level : ScriptableObject
{
    public HoistMapScreen hoistMapScreen;
    public WorkshopMapScreen workshopMapScreen;
}

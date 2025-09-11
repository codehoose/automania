using UnityEngine;

[CreateAssetMenu(fileName ="collectables.asset", menuName = "Automania/Collectables List")]
public class CollectablesList : ScriptableObject
{
    public int paletteIndex;
    public HoistCar carPrefab;
    public Sprite[] sprites;
}

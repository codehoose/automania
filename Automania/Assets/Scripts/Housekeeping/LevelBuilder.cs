using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] Level[] levels;
    [SerializeField] private int currentLevel;

    [SerializeField] private Transform hoistRoomRoot;
    [SerializeField] private Transform warehouseRoomRoot;

    private void Start()
    {
        var level = levels[currentLevel];

        foreach (var objPos in level.hoistMapScreen.commonElements)
        {
            var go = Instantiate(objPos.prefab, hoistRoomRoot);
            go.transform.localPosition = objPos.position;
        }
    }
}

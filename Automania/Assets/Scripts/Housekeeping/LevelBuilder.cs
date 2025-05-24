using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    List<ZXObject> zxObjects;

    [SerializeField] Level[] levels;
    [SerializeField] private int currentLevel;

    [SerializeField] private Transform hoistRoomRoot;
    [SerializeField] private Transform warehouseRoomRoot;

    public List<ZXObject> ZXObjects => zxObjects;

    private void Start()
    {
        zxObjects = new();

        var level = levels[currentLevel];

        foreach (var objPos in level.hoistMapScreen.commonElements)
        {
            var go = Instantiate(objPos.prefab, hoistRoomRoot);
            go.transform.localPosition = objPos.position;
            var zxObject = go.GetComponent<ZXObject>();
            if (zxObject)
            {
                zxObjects.Add(zxObject);
            }
        }

        ZXSpectrumScreen.Instance.AddObjects(ZXObjects);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    List<ZXObject> zxObjects;

    [SerializeField] Level[] levels;
    [SerializeField] private int currentLevel;
    [SerializeField] private bool inWarehouse;

    [SerializeField] private Transform hoistRoomRoot;
    [SerializeField] private Vector3 hoistRoomWallyStart = new Vector3(248, -152);
    [SerializeField] private Vector3 warehouseWallyStart = new Vector3(16, -152);

    [SerializeField] private GameObject wallyMob;
    private List<GameObject> mobs;

    public List<ZXObject> ZXObjects => zxObjects;

    private void Start()
    {
        zxObjects = new();
        mobs = new();
        DescribeRoom();
    }

    public void ChangeRoom()
    {
        inWarehouse = !inWarehouse;
        DescribeRoom();
    }

    public void DescribeRoom()
    {
        ZXSpectrumScreen.Instance.RemoveAll();
        foreach (var obj in zxObjects)
        {
            Destroy(obj.gameObject);
        }
        zxObjects.Clear();

        foreach (var mob in mobs)
        {
            Destroy(mob);
        }
        mobs.Clear();

        var level = levels[currentLevel];
        var commonElements = inWarehouse ? level.workshopMapScreen.commonElements : level.hoistMapScreen.commonElements;

        foreach (var objPos in commonElements)
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

        var wallyStart = inWarehouse ? warehouseWallyStart : hoistRoomWallyStart;
        mobs.Add(Instantiate(wallyMob, wallyStart, Quaternion.identity));
    }
}

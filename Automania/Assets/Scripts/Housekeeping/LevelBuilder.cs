using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    private Color[] palette;
    private List<ZXObject> zxObjects;
    private List<GameObject> mobs;
    private TiledMapFile tiledMap;

    [SerializeField] private int currentLevel = 0;
    [SerializeField] private bool inWarehouse;
    [SerializeField] private Transform hoistRoomRoot;
    [SerializeField] private Vector3 hoistRoomWallyStart = new Vector3(248, -152);
    [SerializeField] private Vector3 warehouseWallyStart = new Vector3(16, -152);
    [SerializeField] private Sprite[] blockSprites;
    [SerializeField] private ZXObject blockPrefab;
    [SerializeField] private GameObject wallyMob;
    [SerializeField] private Texture2D zxSpectrumColours;
    [SerializeField] private TextAsset[] mapFiles;

    public List<ZXObject> ZXObjects => zxObjects;

    private void Start()
    {
        zxObjects = new();
        palette = new Color[8];
        mobs = new();
        tiledMap = TiledMapDeserializer.Load(mapFiles[currentLevel]);

        for (int i = 0; i < 8; i++)
        {
            palette[i] = zxSpectrumColours.GetPixel(i * 8, 0);
        }

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

        var tiledMapGroup = inWarehouse ? tiledMap.GetWorkshop() : tiledMap.GetHoist();
        var inkGroup = tiledMapGroup.GetInk().data;
        var paperGroup = tiledMapGroup.GetPaper().data;
        var blockGroup = tiledMapGroup.GetBlocks().data;

        for (int y = 0; y < 22; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                int index = y * 32 + x;
                var inkId = Mathf.Max(0, inkGroup[index] - 97) % 8;
                var paperId = Mathf.Max(0, paperGroup[index] - 97) % 8;
                var blockId = blockGroup[index] - 1;
                if (blockId < 0) continue;

                var zxObject = Instantiate(blockPrefab, hoistRoomRoot);
                zxObject.transform.localPosition = new Vector3(x * 8, y * -8);
                zxObject.Init(blockSprites[blockId], palette[inkId], palette[paperId]);
                zxObjects.Add(zxObject);
            }
        }

        //var wallyStart = inWarehouse ? warehouseWallyStart : hoistRoomWallyStart;
        //mobs.Add(Instantiate(wallyMob, wallyStart, Quaternion.identity));

        // Add all the objects at the end
        ZXSpectrumScreen.Instance.AddObjects(ZXObjects);
    }
}

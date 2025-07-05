using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    private Color[] palette;
    private List<ZXObject> zxObjects;
    private List<Conveyor> conveyors;
    private List<GameObject> mobs;
    private TiledMapFile tiledMap;

    [SerializeField] private int currentLevel = 0;
    [SerializeField] private bool inWarehouse;
    [SerializeField] private Transform hoistRoomRoot;
    [SerializeField] private Vector3 hoistRoomWallyStart = new Vector3(248, -152);
    [SerializeField] private Vector3 warehouseWallyStart = new Vector3(16, -152);
    [SerializeField] private GameObject[] killObjects;
    [SerializeField] private Sprite[] blockSprites;
    [SerializeField] private ZXObject blockPrefab;
    [SerializeField] private GameObject wallyMob;
    [SerializeField] private Conveyor conveyorPrefab;
    [SerializeField] private Texture2D zxSpectrumColours;
    [SerializeField] private TextAsset[] mapFiles;
    [SerializeField] private ConveyorBlockStapes leftToRight;
    [SerializeField] private ConveyorBlockStapes rightToLeft;

    public List<ZXObject> ZXObjects => zxObjects;

    private void Start()
    {
        zxObjects = new();
        conveyors = new();
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
        //foreach (var obj in zxObjects)
        //{
        //    Destroy(obj.gameObject);
        //}
        //zxObjects.Clear();

        foreach (var mob in mobs)
        {
            Destroy(mob);
        }
        mobs.Clear();

        foreach (var c in conveyors)
        {
            Destroy(c.gameObject);
        }
        conveyors.Clear();

        var tiledMapGroup = inWarehouse ? tiledMap.GetWorkshop() : tiledMap.GetHoist();
        var inkGroup = tiledMapGroup.GetInk().data;
        var paperGroup = tiledMapGroup.GetPaper().data;
        var blockGroup = tiledMapGroup.GetBlocks().data;

        var inkColours = new Color[22 * 32];
        var paperColours = new Color[22 * 32];

        for (int y = 0; y < 22; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                int index = y * 32 + x;
                var inkId = Mathf.Max(0, inkGroup[index] - 49) % 8;
                var paperId = Mathf.Max(0, paperGroup[index] - 49) % 8;
                var blockId = blockGroup[index] - 1;

                var newIndex = (21 - y) * 32 + x;

                inkColours[newIndex] = palette[inkId];
                paperColours[newIndex] = palette[paperId];

                if (blockId < 0) continue;
                var zxObject = Instantiate(blockPrefab, hoistRoomRoot);
                zxObject.transform.localPosition = new Vector3(x * 8, y * -8);
                zxObject.Init(blockSprites[blockId]);
            }
        }

        CreateConveyors(tiledMapGroup.GetConveyors());
        CreateKillerObjects(tiledMapGroup.GetKillerObjects());

        ZXSpectrumScreen.Instance.SetStaticColours(inkColours, paperColours);


        //var wallyStart = inWarehouse ? warehouseWallyStart : hoistRoomWallyStart;
        //mobs.Add(Instantiate(wallyMob, wallyStart, Quaternion.identity));

        // Add all the objects at the end
        //ZXSpectrumScreen.Instance.AddObjects(ZXObjects);
    }

    private void CreateKillerObjects(TileMapObject[] tileMapObjects)
    {
        if (tileMapObjects == null) return;
        foreach (var o in tileMapObjects)
        {
            var index = (int)o.properties[0].value; // TODO: Make this safer
            var go = Instantiate(killObjects[index], new Vector3(o.x, -o.y - 16), Quaternion.identity);
        }
    }

    private void CreateConveyors(TileMapObject[] conveyorPositions)
    {
        if (conveyorPositions != null)
        {
            foreach (var c in conveyorPositions)
            {
                var dir = 1;
                var direction = c.properties.FirstOrDefault(p => p.name == "dir");
                if (direction != null)
                {
                    dir = (int)direction.value;
                }

                var startx = dir > 0 ? c.x : c.x + c.width - 8;
                var sprites = dir > 0 ? leftToRight : rightToLeft;

                var newConveyor = Instantiate(conveyorPrefab, new Vector3(startx, -c.y - 16, 0), Quaternion.identity);
                newConveyor.Init(dir, c.width / 8, sprites.blocks);
                conveyors.Add(newConveyor);
            }
        }
    }
}

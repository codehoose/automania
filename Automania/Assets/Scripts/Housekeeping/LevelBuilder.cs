using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    private static int ScreenTopOffset = -16;
    private static int CellSizePixels = 8;
    private static int PlayScreenColumnCount = 32;
    private static int PlayScreenRowCount = 22;

    private bool describingRoom;
    private Color[] palette;
    private List<ZXObject> zxObjects;
    private List<Conveyor> conveyors;
    private List<Enemy> enemies;
    private TiledMapFile tiledMap;

    [Header("General")]
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private bool inWarehouse;
    [SerializeField] private Transform hoistRoomRoot;
    [SerializeField] private Vector3 hoistRoomWallyStart = new Vector3(248, -152);
    [SerializeField] private Vector3 warehouseWallyStart = new Vector3(16, -152);

    [Header("Map Files")]
    [SerializeField] private TextAsset[] mapFiles;

    [Header("Prefabs")]
    [SerializeField] private GameObject wallyMob;
    [SerializeField] private Conveyor conveyorPrefab;
    [SerializeField] private Ladder ladderPrefab;
    [SerializeField] private ZXObject blockPrefab;
    [SerializeField] private GameObject[] killObjects;
    [SerializeField] private Door[] doorPrefabs;
    [SerializeField] private Enemy[] enemyPrefabs;

    [Header("ZX Spectrum")]
    [SerializeField] private Texture2D zxSpectrumColours;
    

    [Header("Sprites")]
    [SerializeField] private Sprite[] blockSprites;
    [SerializeField] private ConveyorBlockStapes leftToRight;
    [SerializeField] private ConveyorBlockStapes rightToLeft;

    private Color[] staticInkColours;
    private Color[] staticPaperColours;

    public List<ZXObject> ZXObjects => zxObjects;

    private void Start()
    {
        zxObjects = new();
        conveyors = new();
        enemies = new();
        palette = new Color[8];
        tiledMap = TiledMapDeserializer.Load(mapFiles[currentLevel]);

        for (int i = 0; i < 8; i++)
        {
            palette[i] = zxSpectrumColours.GetPixel(i * CellSizePixels, 0);
        }

        DescribeRoom();
    }

    private void Update()
    {
        if (describingRoom) return;

        var inkColours = new Color[PlayScreenRowCount * PlayScreenColumnCount];
        var paperColours = new Color[PlayScreenRowCount * PlayScreenColumnCount];
        Array.Copy(staticInkColours, inkColours, PlayScreenRowCount * PlayScreenColumnCount);
        Array.Copy(staticPaperColours, paperColours, PlayScreenRowCount * PlayScreenColumnCount);

        foreach (var enemy in enemies)
        {
            float rawX = enemy.transform.position.x / CellSizePixels;
            float rawY = enemy.transform.position.y / CellSizePixels;

            int xx = enemy.Direction < 0 ? Mathf.FloorToInt(rawX) : Mathf.CeilToInt(rawX);
            int yy = 23 - Mathf.Abs(Mathf.CeilToInt(rawY));

            int sx = ((int)(rawX)) / 4 > 0 ? -1 : 0;
            int ex = ((int)(rawX + 16)) / 4 > 0 ? 3 : 2;

            for (int yo = 0; yo < 2; yo++)
            {
                for (int xo = sx; xo < ex; xo++)
                {
                    int index = (yy - yo) * 32 + xx + xo;
                    if (index < 0 || index >= 768) continue;

                    inkColours[index] = palette[6];
                    paperColours[index] = palette[0];
                }
            }
        }

        ZXSpectrumScreen.Instance.SetStaticColours(inkColours, paperColours);
    }

    public void ChangeRoom()
    {
        inWarehouse = !inWarehouse;
        DescribeRoom();
    }

    public void DescribeRoom()
    {
        describingRoom = true;
        ZXSpectrumScreen.Instance.RemoveAll();

        foreach(var e in enemies)
        {
            Destroy(e.gameObject);
        }
        enemies.Clear();

        foreach (var c in conveyors)
        {
            Destroy(c.gameObject);
        }
        conveyors.Clear();

        var tiledMapGroup = inWarehouse ? tiledMap.GetWorkshop() : tiledMap.GetHoist();
        var inkGroup = tiledMapGroup.GetInk().data;
        var paperGroup = tiledMapGroup.GetPaper().data;
        var blockGroup = tiledMapGroup.GetBlocks().data;

        staticInkColours = new Color[PlayScreenRowCount * PlayScreenColumnCount];
        staticPaperColours = new Color[PlayScreenRowCount * PlayScreenColumnCount];

        for (int y = 0; y < PlayScreenRowCount; y++)
        {
            for (int x = 0; x < PlayScreenColumnCount; x++)
            {
                int index = y * PlayScreenColumnCount + x;
                var inkId = Mathf.Max(0, inkGroup[index] - 49) % CellSizePixels;
                var paperId = Mathf.Max(0, paperGroup[index] - 49) % CellSizePixels;
                var blockId = blockGroup[index] - 1;

                var newIndex = (PlayScreenRowCount - 1 - y) * PlayScreenColumnCount + x;

                staticInkColours[newIndex] = palette[inkId];
                staticPaperColours[newIndex] = palette[paperId];

                if (blockId < 0) continue;
                var zxObject = Instantiate(blockPrefab, hoistRoomRoot);
                zxObject.transform.localPosition = new Vector3(x * CellSizePixels, y * -CellSizePixels);
                zxObject.Init(blockSprites[blockId]);
            }
        }

        CreateConveyors(tiledMapGroup.GetConveyors());
        CreateKillerObjects(tiledMapGroup.GetKillerObjects());
        CreateLadderObjects(tiledMapGroup.GetLadders());
        CreateEnemyObjects(tiledMapGroup.GetEnemies());
        CreateDoor(tiledMapGroup.GetDoor());

        ZXSpectrumScreen.Instance.SetStaticColours(staticInkColours, staticPaperColours);

        //var wallyStart = inWarehouse ? warehouseWallyStart : hoistRoomWallyStart;
        //mobs.Add(Instantiate(wallyMob, wallyStart, Quaternion.identity));

        // Add all the objects at the end
        //ZXSpectrumScreen.Instance.AddObjects(ZXObjects);
        describingRoom = false;
    }

    private void CreateEnemyObjects(TileMapObject[] tileMapObjects)
    {
        if (tileMapObjects == null) return;

        foreach(var e in tileMapObjects)
        {
            var prefab = enemyPrefabs.FirstOrDefault(p => p.name == e.name);
            if (prefab == null) continue;

            var enemy = Instantiate(prefab, new Vector3(e.x, -e.y + ScreenTopOffset), Quaternion.identity);
            var waitTime = e.properties.FirstOrDefault(p => p.name == "wait");
            var waitInSecs = waitTime == null ? 0 : waitTime.value;

            enemy.Init(e.properties.FirstOrDefault(p => p.name == "dir").value, waitInSecs);

            enemies.Add(enemy);
        }
    }

    private void CreateDoor(TileMapObject tileMapObject)
    {
        if (tileMapObject == null) return;
        var d = tileMapObject;

        var door = Instantiate(doorPrefabs[inWarehouse ? 0 : 1], new Vector3(d.x, -d.y + ScreenTopOffset), Quaternion.identity);
    }

    private void CreateLadderObjects(TileMapObject[] tileMapObjects)
    {
        if (tileMapObjects == null) return;
        foreach (var o in tileMapObjects)
        {
            var numRungs = (o.height / CellSizePixels) - 4; // MAGIC NUMBERS PLEASE REPLACE!!
            var ladder = Instantiate(ladderPrefab, new Vector3(o.x, -o.y + ScreenTopOffset), Quaternion.identity);
            ladder.Init(numRungs);
        }
    }

    private void CreateKillerObjects(TileMapObject[] tileMapObjects)
    {
        if (tileMapObjects == null) return;
        foreach (var o in tileMapObjects)
        {
            var index = (int)o.properties[0].value; // TODO: Make this safer
            var go = Instantiate(killObjects[index], new Vector3(o.x, -o.y + ScreenTopOffset), Quaternion.identity);
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

                var startx = dir > 0 ? c.x : c.x + c.width - CellSizePixels;
                var sprites = dir > 0 ? leftToRight : rightToLeft;

                var newConveyor = Instantiate(conveyorPrefab, new Vector3(startx, -c.y + ScreenTopOffset, 0), Quaternion.identity);
                newConveyor.Init(dir, c.width / CellSizePixels, sprites.blocks);
                conveyors.Add(newConveyor);
            }
        }
    }
}

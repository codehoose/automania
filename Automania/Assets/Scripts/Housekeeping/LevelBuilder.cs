using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    class TempAttr
    {
        public float x;
        public float y;
        public float dir;
        public int height;
        public int ink = -1;
        public int width;
        public int centerOffset;
    }

    private static int ScreenTopOffset = -16;
    private static int CellSizePixels = 8;
    private static int PlayScreenColumnCount = 32;
    private static int PlayScreenRowCount = 22;
    private static int WallyHeight = 32;
    private static int WallyWidth = 16;

    private bool describingRoom;
    private Color[] palette;
    private List<ZXObject> charBlocks;
    private List<Conveyor> conveyors;
    private List<Collectable> collectables;
    private List<Enemy> enemies;
    private List<Ladder> ladders;
    private List<GameObject> killerObjects;
    private TiledMapFile tiledMap;
    private CollectablesList currentCollectables;
    private WallyMob wallyInstance;
    private Door doorInstance;

    [Header("General")]
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private bool inWarehouse;
    [SerializeField] private Transform hoistRoomRoot;

    [Header("Map Files")]
    [SerializeField] private TextAsset[] mapFiles;
    [SerializeField] private CollectablesList[] collectablesList;

    [Header("Prefabs")]
    [SerializeField] private WallyMob wallyMob;
    [SerializeField] private Conveyor conveyorPrefab;
    [SerializeField] private Ladder ladderPrefab;
    [SerializeField] private ZXObject blockPrefab;
    [SerializeField] private Collectable collectablePrefab;
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

    private void Start()
    {
        charBlocks = new();
        conveyors = new();
        enemies = new();
        collectables = new();
        ladders = new();
        killerObjects = new();
        palette = new Color[8];
        tiledMap = TiledMapDeserializer.Load(mapFiles[currentLevel]);
        currentCollectables = collectablesList[currentLevel];

        for (int i = 0; i < 8; i++)
        {
            palette[i] = zxSpectrumColours.GetPixel(i * CellSizePixels, 0);
        }

        DescribeRoom();
        CreateCollectables();
    }

    private void Update()
    {
        if (describingRoom) return;

        var inkColours = new Color[PlayScreenRowCount * PlayScreenColumnCount];
        var paperColours = new Color[PlayScreenRowCount * PlayScreenColumnCount];
        Array.Copy(staticInkColours, inkColours, PlayScreenRowCount * PlayScreenColumnCount);
        Array.Copy(staticPaperColours, paperColours, PlayScreenRowCount * PlayScreenColumnCount);

        var objects = new List<TempAttr>();

        objects.AddRange(collectables.Where(c => !c.collected && inWarehouse).Select(c => new TempAttr
        {
            x = c.transform.position.x,
            y = c.transform.position.y,
            ink = currentCollectables.paletteIndex,
            dir = 1,
            height = 2,
            width = 2,
        }));

        objects.AddRange(enemies.Select(e => new TempAttr
        {
            x = e.transform.position.x,
            y = e.transform.position.y,
            dir = e.Direction,
            height = 2,
            width = 2
        }));

        if (wallyInstance)
            objects.Add(new TempAttr()
            {
                x = wallyInstance.transform.position.x,
                y = wallyInstance.transform.position.y + WallyHeight,
                dir = wallyInstance.GetComponent<SpriteRenderer>().flipX ? -1 : 1,
                height = WallyHeight / CellSizePixels,
                width = WallyWidth / CellSizePixels,
                centerOffset = CellSizePixels / 2
            });

        foreach (var obj in objects)
        {
            float rawX = obj.x / CellSizePixels;
            float rawY = obj.y / CellSizePixels;

            int xx = obj.dir < 0 ? Mathf.FloorToInt(rawX) : Mathf.CeilToInt(rawX);
            int yy = 23 - Mathf.Abs(Mathf.CeilToInt(rawY));

            int sx = ((int)(rawX)) / 4 >= 0 ? -1 : 0;
            int ex = ((int)(rawX + obj.width * 2)) / 4 > 0 ? obj.width + 1 : obj.width;

            for (int yo = 0; yo < obj.height; yo++)
            {
                for (int xo = sx; xo < ex; xo++)
                {
                    int index = (yy - yo) * 32 + xx + xo;
                    if (index < 0 || index >= 768) continue;

                    inkColours[index] = obj.ink > -1 ? palette[obj.ink] : palette[6];
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

        ClearComponentList(enemies);
        ClearComponentList(conveyors);
        ClearComponentList(charBlocks);
        ClearComponentList(ladders);
        if (wallyInstance)
        {
            Destroy(wallyInstance.gameObject);
            wallyInstance = null;
        }

        if (doorInstance)
        {
            Destroy(doorInstance.gameObject);
            doorInstance = null;
        }

        foreach (var ko in killerObjects)
        {
            Destroy(ko);
        }
        killerObjects.Clear();

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
                var charBlock = Instantiate(blockPrefab, hoistRoomRoot);
                charBlock.transform.localPosition = new Vector3(x * CellSizePixels, y * -CellSizePixels);
                charBlock.Init(blockSprites[blockId]);
                charBlocks.Add(charBlock);
            }
        }

        CreateConveyors(tiledMapGroup.GetConveyors());
        CreateKillerObjects(tiledMapGroup.GetKillerObjects());
        CreateLadderObjects(tiledMapGroup.GetLadders());
        CreateEnemyObjects(tiledMapGroup.GetEnemies());
        CreateDoor(tiledMapGroup.GetDoor());

        ZXSpectrumScreen.Instance.SetStaticColours(staticInkColours, staticPaperColours);

        var wallyStart = GetWallyStart(tiledMapGroup.GetPlayerStart());
        wallyInstance = Instantiate(wallyMob, wallyStart, Quaternion.identity);

        foreach (var collectable in collectables)
        {
            collectable.gameObject.SetActive(inWarehouse);
        }

        describingRoom = false;
    }

    private void CreateCollectables()
    {
        var levelCollectables = tiledMap.GetWorkshop().GetCollectables();
        if (levelCollectables == null) return;

        foreach (var c in levelCollectables)
        {
            var col = Instantiate(collectablePrefab, new Vector2(c.x, -c.y + ScreenTopOffset), Quaternion.identity);
            col.index = c.properties[0].value;
            col.SetSprite(currentCollectables.sprites[col.index]);
            collectables.Add(col);
        }
    }

    private Vector3 GetWallyStart(TileMapObject tileMapObject)
    {
        if (tileMapObject == null) return Vector3.zero;
        return new Vector3(tileMapObject.x, -tileMapObject.y + ScreenTopOffset);
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

        doorInstance = Instantiate(doorPrefabs[inWarehouse ? 0 : 1], new Vector3(d.x, -d.y + ScreenTopOffset), Quaternion.identity);
    }

    private void CreateLadderObjects(TileMapObject[] tileMapObjects)
    {
        if (tileMapObjects == null) return;
        foreach (var o in tileMapObjects)
        {
            var numRungs = (o.height / CellSizePixels) - 4; // MAGIC NUMBERS PLEASE REPLACE!!
            var ladder = Instantiate(ladderPrefab, new Vector3(o.x, -o.y + ScreenTopOffset), Quaternion.identity);
            ladder.Init(numRungs);
            ladders.Add(ladder);
        }
    }

    private void CreateKillerObjects(TileMapObject[] tileMapObjects)
    {
        if (tileMapObjects == null) return;
        foreach (var o in tileMapObjects)
        {
            var index = (int)o.properties[0].value; // TODO: Make this safer
            killerObjects.Add(Instantiate(killObjects[index], new Vector3(o.x, -o.y + ScreenTopOffset), Quaternion.identity));
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

    private void ClearComponentList<T>(List<T> components) where T : Component
    {
        if (components == null || components.Count == 0) return;

        foreach (var c in components) Destroy(c.gameObject);
        components.Clear();
    }
}

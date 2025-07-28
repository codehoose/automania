using System;
using System.Linq;

[Serializable]
public class TiledMapGroup
{
    public string name;
    public TiledMapLayer[] layers;

    public TiledMapLayer GetInk() => layers.FirstOrDefault(layer => layer.name == "Ink");

    public TiledMapLayer GetPaper() => layers.FirstOrDefault(layer => layer.name == "Paper");

    public TiledMapLayer GetBlocks() => layers.FirstOrDefault(layer => layer.name == "Blocks");

    public TileMapObject[] GetConveyors() => layers.FirstOrDefault(layer => layer.name == "Conveyors")?.objects;

    public TileMapObject[] GetKillerObjects() => layers.FirstOrDefault(layer => layer.name == "KillerObjects")?.objects;

    public TileMapObject[] GetLadders() => layers.FirstOrDefault(layer => layer.name == "Ladders")?.objects;

    public TileMapObject[] GetEnemies() => layers.FirstOrDefault(layer => layer.name == "Enemies")?.objects;

    public TileMapObject[] GetCollectables() => layers.FirstOrDefault(layer => layer.name == "Collectables")?.objects;

    public TileMapObject GetDoor() => layers.FirstOrDefault(layer => layer.name == "Doors")?.objects[0];

    public TileMapObject GetPlayerStart() => layers.FirstOrDefault(layer => layer.name == "Player")?.objects[0];
}

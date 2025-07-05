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
}

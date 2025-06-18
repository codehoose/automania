using System;
using System.Linq;

[Serializable]
public class TiledMapFile
{
    public TiledMapGroup[] layers;

    public TiledMapGroup GetWorkshop() => layers.FirstOrDefault(group => group.name == "Workshop");

    public TiledMapGroup GetHoist() => layers.FirstOrDefault(group => group.name == "Hoist");
}
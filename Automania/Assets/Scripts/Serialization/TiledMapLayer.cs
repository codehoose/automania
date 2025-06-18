using System;

[Serializable]
public class TiledMapLayer
{
    public int[] data;
    public string name;
    public string type;
    public TileMapObject[] objects;
}

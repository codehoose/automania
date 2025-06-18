using System;

[Serializable]
public class TileMapObject
{
    public int x;
    public int y;
    public int width;
    public int height;
    public string name;
    public TileMapProperty[] properties;
}
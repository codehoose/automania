using UnityEngine;

internal static class TiledMapDeserializer
{
    public static TiledMapFile Load(TextAsset asset)
    {
        var tiledMapGroup = JsonUtility.FromJson<TiledMapFile>(asset.text);
        return tiledMapGroup;
    }
}

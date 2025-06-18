using UnityEngine;

public class ZXObject : MonoBehaviour
{
    public AttributeRun[] attrs;
    public int drawOrder;

    public void Init(Sprite sprite, Color ink, Color paper)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;

        var attrRun = ScriptableObject.CreateInstance<AttributeRun>();
        attrRun.ink = ink;
        attrRun.paper = paper;
        attrRun.size = new Vector2(1, 1);
        attrs = new[] { attrRun };
    }
}

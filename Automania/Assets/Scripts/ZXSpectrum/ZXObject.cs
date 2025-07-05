using UnityEngine;

public class ZXObject : MonoBehaviour
{
    public AttributeRun[] attrs;
    public int drawOrder;

    public void Init(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}

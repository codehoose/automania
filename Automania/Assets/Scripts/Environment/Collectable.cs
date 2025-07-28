using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Collectable : MonoBehaviour
{
    public int index;
    public bool collected;

    public void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void Collect()
    {
        collected = true;
        GetComponent<SpriteRenderer>().sprite = null;
    }
}

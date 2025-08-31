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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerPickup")
        {
            var wally = collision.transform.GetComponentInParent<WallyMob>();
            var copy = Instantiate(gameObject);
            gameObject.SetActive(false);
            wally.PickupObject(index, copy.transform);
        }
    }
}

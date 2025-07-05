using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    private const float TICK_TIME = 0.125f;
    private bool creatingBlocks = true;

    private Sprite[] sprites;
    private int blockCount;
    private int direction;
    private int currentFrame = -1;
    private float cooldown = 0f;
    private float pauseTime = 0f;

    List<Object> blocks;

    [SerializeField] private GameObject blockPrefab;

    private void Awake()
    {
        blocks = new();
    }

    public void Init(int direction, int blockCount, Sprite[] sprites)
    {
        this.direction = direction;
        this.blockCount = blockCount;
        this.sprites = sprites;
    }


    private void Update()
    {
        if (cooldown < TICK_TIME)
        {
            cooldown += Time.deltaTime;
            return;
        }

        if (pauseTime > 0)
        {
            pauseTime -= Time.deltaTime;
            if (pauseTime < 0)
                pauseTime = 0;

            return;
        }

        if (creatingBlocks)
        {
            if (currentFrame < 0)
            {
                var block = Instantiate(blockPrefab, transform.position + new Vector3(blocks.Count * 8 * direction, 0, 0), Quaternion.identity);
                blocks.Add(block);
                block.GetComponent<SpriteRenderer>().sprite = sprites[0];
                currentFrame = 1;
            }
            else
            {
                var current = blocks[blocks.Count - 1].GetComponent<SpriteRenderer>();
                current.sprite = sprites[currentFrame++];
                if (currentFrame == 5)
                {
                    currentFrame = -1;

                    if (blocks.Count == blockCount)
                    {
                        creatingBlocks = false;
                        currentFrame = 4;
                        pauseTime = 1;
                    }
                }
            }
        }
        else
        {
            if (currentFrame >= 0)
            {
                var current = blocks[blocks.Count - 1].GetComponent<SpriteRenderer>();
                current.sprite = sprites[currentFrame--];
            }
            else
            {
                var block = blocks[blocks.Count - 1];
                blocks.Remove(block);
                Destroy(block);
                currentFrame = 4;
                if (blocks.Count == 0)
                {
                    creatingBlocks = true;
                    currentFrame = -1;
                    pauseTime = 1;
                }
            }
        }

        cooldown -= TICK_TIME;
    }
}

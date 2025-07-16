using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private SpriteRenderer r;
    private int index;
    private bool started;
    private float dir;
    private float frameCount = 0;
    private float moveCount = 0;

    private Vector3 pos;

    [SerializeField] private float waitToStartSeconds;
    [SerializeField] private Sprite[] sprites;

    public int Direction => (int)Mathf.Sign(dir);

    public void Init(int direction, int waitSecs)
    {
        waitToStartSeconds = waitSecs;
        pos = transform.position;
        dir = direction;
    }

    IEnumerator Start()
    {
        r = GetComponent<SpriteRenderer>();
        r.sprite = sprites[index];

        if (waitToStartSeconds > 0)
        {
            yield return new WaitForSeconds(waitToStartSeconds);
        }

        started = true;
    }


    private void Update()
    {
        if (!started) return;

        frameCount += Time.deltaTime;
        if (frameCount > 0.25f)
        {
            frameCount -= 0.25f;
            index = (index + 1) % sprites.Length;
            r.sprite = sprites[index];
        }

        moveCount += Time.deltaTime;
        if (moveCount > 0.25f)
        {
            pos += Vector3.right * dir * Time.deltaTime * 16f;

            if (pos.x >240)
            {
                pos.x = 240;
                dir *= -1;
            }

            if (pos.x < 0)
            {
                pos.x = 0;
                dir *= -1;
            }

            transform.position = new Vector3(Mathf.Floor(pos.x), Mathf.Floor(pos.y));
        }
    }
}

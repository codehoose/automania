using UnityEngine;

public class WallyMob : ZXMob
{
    private SpriteRenderer spr;
    private bool stopProcessingCollisions;
    private float x;
    private int frame;
    private bool dir;
    private Sprite[] frames;

    [SerializeField] private Sprite[] walking;
    [SerializeField] private Sprite[] climbing;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (stopProcessingCollisions) return;

        if (collision.tag == "Door")
        {
            stopProcessingCollisions = true;
            GameController.Instance.ChangeRoom();
        }
    }

    public override void Start()
    {
        base.Start();
        frames = walking;

        NextWalkFrame.AddListener(() =>
        {
            frame = (frame + 1) % frames.Length;
            spr.sprite = frames[frame];
        });

        spr = GetComponent<SpriteRenderer>();
        x = GameController.Instance.RegisterWally(Move_Changed);
        spr.flipX = x < 0;
    }

    private void OnDestroy()
    {
        GameController.Instance?.MovePlayer.RemoveListener(Move_Changed);
    }

    private void Move_Changed(float newValue)
    {
        x = newValue;
        spr.flipX = x == 0 ? spr.flipX : x < 0;
        if (x == 0)
        {
            frame = 0;
            spr.sprite = frames[frame];
            ResetAnimation();
        }
    }

    public override void FixedUpdate()
    {
        pos.x += x * 32f * Time.deltaTime;
        base.FixedUpdate();
    }
}

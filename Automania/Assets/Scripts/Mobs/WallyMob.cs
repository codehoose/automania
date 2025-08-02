using UnityEngine;

public class WallyMob : ZXMob
{
    private static float SpeedPixelsPerSecond = 32f;
    private static float NearGroundFudgeFactorPixels = 2f;

    private SpriteRenderer spr;
    private bool stopProcessingCollisions;
    private bool isNearTop;
    private bool isNearBottom;
    private float x;
    private float y;
    private int frame;
    private bool isClimbing;
    private Sprite[] frames;

    private Ladder currentLadder;

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
        else if (collision.tag == "Rung")
        {
            var ladder = collision.GetComponentInParent<Ladder>();
            if (ladder == null) return;
            currentLadder = ladder;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Rung" && !isClimbing)
        {
            currentLadder = null;
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

    private void Move_Changed(float newX, float newY)
    {
        if (isClimbing)
        {
            y = newY;
            x = newX;
        }
        else
        {
            if (!isClimbing && newY != 0 && OverALadder())
            {
                y = newY;
                x = newX;
                isClimbing = true;
                frames = climbing;
                frame = 0;
                pos.x = currentLadder.transform.position.x + LevelBuilder.CellSizePixels;
                ResetAnimation();
            }
            else
            {
                x = newX;
                spr.flipX = x == 0 ? spr.flipX : x < 0;
                if (x == 0)
                {
                    frame = 0;
                    spr.sprite = frames[frame];
                    ResetAnimation();
                }
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"NearTop: {isNearTop}, NearBottom: {isNearBottom}");
        GUILayout.EndHorizontal();
    }

    public override void FixedUpdate()
    {
        if (isClimbing)
        {
            pos.y += y * SpeedPixelsPerSecond * Time.deltaTime;

            var nearTop = Mathf.Abs((currentLadder.transform.position.y - LevelBuilder.WallyHeight) - pos.y);
            isNearTop = nearTop <= NearGroundFudgeFactorPixels;

            var nearBottom = Mathf.Abs((currentLadder.LowestY - LevelBuilder.CellSizePixels) - pos.y);
            isNearBottom= nearBottom <= NearGroundFudgeFactorPixels;

            if (pos.y > currentLadder.transform.position.y - LevelBuilder.WallyHeight)
            {
                pos.y = currentLadder.transform.position.y - LevelBuilder.WallyHeight;
            }
            else if (pos.y < currentLadder.LowestY - LevelBuilder.CellSizePixels)
            {
                pos.y = currentLadder.LowestY - LevelBuilder.CellSizePixels;
            }

            if (x != 0 && isNearTop)
            {
                pos.y = currentLadder.transform.position.y - LevelBuilder.WallyHeight;
                isClimbing = false;
                frames = walking;
                frame = 0;
                spr.flipX = x < 0;
                spr.sprite = frames[frame];
                ResetAnimation();
            }
            else if (x != 0 && isNearBottom)
            {
                pos.y = currentLadder.LowestY - LevelBuilder.CellSizePixels;
                isClimbing = false;
                frames = walking;
                frame = 0;
                spr.flipX = x < 0;
                spr.sprite = frames[frame];
                ResetAnimation();
            }
        }

        if (!isClimbing)
        {
            pos.x += x * SpeedPixelsPerSecond * Time.deltaTime;
        }
        
        base.FixedUpdate();
    }

    private bool OverALadder()
    {
        if (!currentLadder) return false;

        return pos.x >= currentLadder.transform.position.x && 
               pos.x <= currentLadder.transform.position.x + 16;
    }
}

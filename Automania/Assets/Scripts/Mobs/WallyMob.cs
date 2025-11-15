using System.Linq;
using UnityEngine;

public class WallyMob : ZXMob
{
    enum MovementState
    {
        Walking,
        Climbing,
        Jumping,
        Falling
    }

    private const float ProbeDistance = 8f;
    private const float ProbeDistanceShort = 4f;

    private static float SpeedPixelsPerSecond = 32f;
    private static float JumpSpeedPixelsPerSecond = 34f;
    private static float NearGroundFudgeFactorPixels = 2f;
    
    private SpriteRenderer spr;
    private bool stopProcessingCollisions;
    private bool isNearTop;
    private bool isNearBottom;
    private float x;
    private float y;
    private int frame;
    private Sprite[] frames;

    private Ladder currentLadder;
    private MovementState state;
    private bool isFreeFalling;

    [SerializeField] private bool invincible;
    [SerializeField] private Transform pickupRoot;
    [SerializeField] private Sprite[] walking;
    [SerializeField] private Sprite[] climbing;

    [Header("Ground Probes")]
    [SerializeField] private Transform leftGroundProbe;
    [SerializeField] private Transform rightGroundProbe;
    private float jumpInitialY;
    private float jumpTime;
    private float jumpX;
    private int jumpY;
    private bool jumpFalling;

    private bool Carrying => pickupRoot.childCount > 0;

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
        else if (collision.tag == "HoistDropOff")
        {
            DropCurrentObject();
        }
        else if (collision.tag == "Kill" && !invincible)
        {
            GameController.Instance.WallyKilled();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Rung" && state != MovementState.Climbing)
        {
            currentLadder = null;
        }
    }

    public void PickupObject(int collectableIndex, Transform obj)
    {
        if (Carrying) return;
        obj.SetParent(pickupRoot);
        obj.localPosition = Vector2.left * 8; // move left 8 pixels
        GameController.Instance.State.CurrentCollectable = collectableIndex;
        GameController.Instance.State.PickupObject();
    }

    public void DropCurrentObject()
    {
        if (!Carrying) return;

        var child = pickupRoot.GetChild(0);
        GameController.Instance.DropObject();
        Destroy(child.gameObject);
    }

    public override void Start()
    {
        base.Start();
        frames = walking;
        state = MovementState.Walking;

        NextWalkFrame.AddListener(() =>
        {
            frame = (frame + 1) % frames.Length;
            spr.sprite = frames[frame];
        });

        spr = GetComponent<SpriteRenderer>();
        x = GameController.Instance.RegisterWally(Move_Changed, Jump_Changed);
        spr.flipX = x < 0;
    }

    private void OnDestroy()
    {
        // TODO: Is this going to be a problem? There is no unregister
    }

    private void Jump_Changed()
    {
        if (state != MovementState.Walking) return;

        jumpInitialY = pos.y;
        jumpFalling = false;
        jumpTime = 0f;
        jumpX = x;
        jumpY = 1;
        state = MovementState.Jumping;
    }

    private void Move_Changed(float newX, float newY)
    {
        if (state == MovementState.Jumping)
        {
            if (newX != jumpX) jumpX = newX;
            
            return;
        }

        if (state == MovementState.Climbing)
        {
            y = newY;
            x = newX;
        }
        else
        {
            if (newY != 0 && OverALadder())
            {
                y = newY;
                x = newX;
                state = MovementState.Climbing;
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

    //private void OnGUI()
    //{
    //    GUILayout.BeginHorizontal();
    //    GUILayout.Label($"NearTop: {isNearTop}, NearBottom: {isNearBottom}");
    //    GUILayout.EndHorizontal();
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(leftGroundProbe.position, 2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(leftGroundProbe.position + Vector3.down * ProbeDistanceShort, 2f);
        Gizmos.DrawWireSphere(leftGroundProbe.position + Vector3.down * ProbeDistance, 2f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(rightGroundProbe.position, 2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rightGroundProbe.position + Vector3.down * ProbeDistanceShort, 2f);
        Gizmos.DrawWireSphere(rightGroundProbe.position + Vector3.down * ProbeDistance, 2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(leftGroundProbe.position, leftGroundProbe.position + Vector3.down * ProbeDistanceShort);
        Gizmos.DrawLine(rightGroundProbe.position, rightGroundProbe.position + Vector3.down * ProbeDistanceShort);
    }

    public override void FixedUpdate()
    {
        if (state == MovementState.Climbing)
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
                state = MovementState.Walking;
                frames = walking;
                frame = 0;
                spr.flipX = x < 0;
                spr.sprite = frames[frame];
                ResetAnimation();
            }
            else if (x != 0 && isNearBottom)
            {
                pos.y = currentLadder.LowestY - LevelBuilder.CellSizePixels;
                state = MovementState.Walking;
                frames = walking;
                frame = 0;
                spr.flipX = x < 0;
                spr.sprite = frames[frame];
                ResetAnimation();
            }
        }
        else if (state == MovementState.Jumping || state == MovementState.Falling)
        {
            pos.y += jumpY * JumpSpeedPixelsPerSecond * Time.deltaTime;
            jumpTime += Time.deltaTime;
            if (jumpTime >= 0.5f && !jumpFalling)
            {
                jumpFalling = true;
                jumpY *= -1;
            }

            if (jumpTime >= 1f)
            {
                if (IsOnPlatform())
                {
                    if (isFreeFalling)
                    {
                        isFreeFalling = false;
                        GameController.Instance.WallyKilled();
                    }
                    else
                    {
                        state = MovementState.Walking;
                        pos.y = jumpInitialY;
                        x = jumpX;
                        spr.flipX = x == 0 ? spr.flipX : x < 0;
                    }
                }
                else
                {
                    state = MovementState.Falling;
                    x = 0;
                    if (pos.y < jumpInitialY - 8)
                    {
                        isFreeFalling = true;
                    }
                }
            }
        }

        if (state != MovementState.Climbing)
        {
            pos.x += x * SpeedPixelsPerSecond * Time.deltaTime;
        }
        
        base.FixedUpdate();
    }

    private bool IsOnPlatform(float probeDistance = ProbeDistance)
    {
        var hitTestLeft = Physics2D.OverlapCircleAll(leftGroundProbe.position + Vector3.down * ProbeDistanceShort, 1f);
        var hitTestRight = Physics2D.OverlapCircleAll(rightGroundProbe.position + Vector3.down * ProbeDistanceShort, 1f);
        return hitTestLeft.Concat(hitTestRight)
                          .Where(hit => hit.gameObject.tag == "Walkable")
                          .Any();
    }
    
    private bool OverALadder()
    {
        if (!currentLadder) return false;

        return pos.x >= currentLadder.transform.position.x && 
               pos.x <= currentLadder.transform.position.x + 16;
    }
}

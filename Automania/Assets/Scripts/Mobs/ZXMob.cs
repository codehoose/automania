using UnityEngine;
using UnityEngine.Events;

public class ZXMob : ZXObject
{
    private float lastX;
    private float lastY;

    protected Vector3 pos;
    public bool addOnStartup;

    protected UnityEvent NextWalkFrame;

    public virtual void Start()
    {
        NextWalkFrame = new UnityEvent();
        pos = transform.position;
        lastX = pos.x;
    }

    public virtual void FixedUpdate()
    {
        if (Mathf.Abs((int)pos.x - (int)lastX) > 2)
        {
            lastX = (int)pos.x;
            NextWalkFrame?.Invoke();
        }
        else if (Mathf.Abs((int)pos.y - (int)lastY) > 2)
        {
            lastY = (int)pos.y;
            NextWalkFrame?.Invoke();
        }

        transform.position = new Vector3((int)pos.x, (int)pos.y, 0);
    }

    protected void ResetAnimation()
    {
        lastX = (int)transform.position.x;
    }
}

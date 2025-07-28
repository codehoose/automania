using UnityEngine;
using UnityEngine.Events;

public class ZXMob : ZXObject
{
    private float lastX;

    protected Vector3 pos;
    public bool addOnStartup;

    protected UnityEvent NextWalkFrame;

    public virtual void Start()
    {
        NextWalkFrame = new UnityEvent();

        if (addOnStartup)
        {
            pos = transform.position;
            ZXSpectrumScreen.Instance.AddObject(this);
        }
    }

    public virtual void FixedUpdate()
    {
        if (Mathf.Abs((int)pos.x - (int)lastX) > 2)
        {
            lastX = (int)pos.x;
            NextWalkFrame?.Invoke();
        }

        transform.position = new Vector3((int)pos.x, (int)pos.y, 0);
    }

    protected void ResetAnimation()
    {
        lastX = (int)pos.x;
    }
}

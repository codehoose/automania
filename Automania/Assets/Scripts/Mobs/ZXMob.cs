using UnityEngine;

public class ZXMob : ZXObject
{
    protected Vector3 pos;
    public bool addOnStartup;

    public virtual void Start()
    {
        if (addOnStartup)
        {
            pos = transform.position;
            ZXSpectrumScreen.Instance.AddObject(this);
        }
    }

    public virtual void FixedUpdate()
    {
        transform.position = new Vector3((int)pos.x, (int)pos.y, 0);
    }
}

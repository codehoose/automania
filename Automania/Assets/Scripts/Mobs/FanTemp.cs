using UnityEngine;

public class FanTemp : MonoBehaviour
{
    Vector3 pos;

    public float sx = 32;
    public float sy = 32;


    private void Update()
    {
        pos = pos += new Vector3(sx, sy, 0) * Time.deltaTime;

        if (pos.x >= 255 - 16)
        {
            pos.x = 255 - 16;
            sx *= -1;
        }

        if (pos.x < 0)
        {
            pos.x = 0;
            sx *= -1;
        }

        if (pos.y > 0)
        {
            pos.y = 0;
            sy *= -1;
        }

        if (pos.y < -166)
        {
            pos.y = -166;
            sy *= -1;
        }

        transform.position = new Vector3((int)pos.x, (int)pos.y, 0);
    }
}

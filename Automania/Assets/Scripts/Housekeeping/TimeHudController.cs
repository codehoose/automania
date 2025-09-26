using UnityEngine;

public class TimeHudController : MonoBehaviour
{
    private float current = 1f;

    [SerializeField] private float maxValue = 10f;

    public float Current
    {
        get => current;
        set
        {
            var tmp = Mathf.Clamp(value, 0f, 1f);
            var scale = maxValue * tmp;
            current = tmp;
            transform.localScale = new Vector3(scale, 1);
        }
    }
}

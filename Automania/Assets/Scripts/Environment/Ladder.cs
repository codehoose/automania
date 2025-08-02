using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private GameObject rungPrefab;

    public float LowestY { get; private set; }

    public void Init(int numRungs)
    {
        for (int i = 0; i < numRungs; i++)
        {
            var rung = Instantiate(rungPrefab, root);
            rung.transform.localPosition = new Vector3(0, i * -8);
            LowestY = rung.transform.position.y;
        }
    }
}

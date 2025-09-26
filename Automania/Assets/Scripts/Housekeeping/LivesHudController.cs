using System.Linq;
using UnityEngine;

public class LivesHudController : MonoBehaviour
{
    [SerializeField] private GameObject[] wallys;

    public int Lives
    {
        get
        {
            return wallys.Where(w => w.activeInHierarchy).Count();
        }
        set
        {
            var lives = Mathf.Clamp(value, 0, wallys.Length);

            for (int i = 0; i < wallys.Length; i++)
            {
                wallys[i].SetActive(i < lives);
            }
        }
    }
}

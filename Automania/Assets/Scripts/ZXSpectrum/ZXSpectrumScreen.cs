using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZXSpectrumScreen : MonoBehaviour
{
    #region Singleton
    private static ZXSpectrumScreen instance;
    public static ZXSpectrumScreen Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ZXSpectrumScreen>();
                if (instance == null)
                {
                    var go = new GameObject("go", new System.Type[] { typeof(ZXSpectrumScreen) });
                    instance = go.GetComponent<ZXSpectrumScreen>();
                }
            }

            return instance;
        }
    }

    #endregion

    private Material matInstance;
    private Texture2D inkInstance;
    private Texture2D paperInstance;

    List<ZXObject> objects;

    [SerializeField] private Transform plane;
    [SerializeField] private Material material;
    [SerializeField] private Texture2D ink;
    [SerializeField] private Texture2D paper;

    private void Awake()
    {
        objects = new();
        matInstance = Instantiate(material);
        paperInstance = Instantiate(paper);
        inkInstance = Instantiate(ink);
        plane.GetComponent<Renderer>().material = matInstance;
        matInstance.SetTexture("_OverlayTex", inkInstance);
        matInstance.SetTexture("_OverlayPaperTex", paperInstance);
    }

    public void AddObject(ZXObject zxo)
    {
        objects.Add(zxo);
    }

    public void AddObjects(IEnumerable<ZXObject> newObjects)
    {
        objects.AddRange(newObjects);
    }

    private void Update()
    {
        Color[] inkColours = new Color[32 * 24];
        Color[] paperColours = new Color[32 * 24];

        var objs = objects.OrderBy(o => o.drawOrder);

        foreach (var obj in objs)
        {
            int x = Mathf.FloorToInt(obj.transform.position.x / 8);
            int y = 23 - Mathf.Abs(Mathf.FloorToInt(obj.transform.position.y / 8));

            foreach (var run in obj.attrs)
            {
                int xx = (int)(x + run.position.x);
                int yy = (int)(y - run.position.y);

                for (int yo = 0; yo < run.size.y; yo++)
                {
                    for (int xo = 0; xo < run.size.x; xo++)
                    {
                        int index = (yy - yo) * 32 + xx + xo;
                        inkColours[index] = run.ink;
                        paperColours[index] = run.paper;
                    }
                }
            }
        }

        inkInstance.SetPixels(inkColours);
        paperInstance.SetPixels(paperColours);
        inkInstance.Apply();
        paperInstance.Apply();
    }
}

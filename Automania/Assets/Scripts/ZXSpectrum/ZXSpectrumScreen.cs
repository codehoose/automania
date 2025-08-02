using System;
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

    private Color White = new Color(221, 221, 221);

    private Material matInstance;
    private Texture2D inkInstance;
    private Texture2D paperInstance;

    [SerializeField] private Transform plane;
    [SerializeField] private Material material;
    [SerializeField] private Texture2D ink;
    [SerializeField] private Texture2D paper;
    private Color[] staticInk;
    private Color[] staticPaper;

    private void Awake()
    {
        matInstance = Instantiate(material);
        paperInstance = Instantiate(paper);
        inkInstance = Instantiate(ink);
        plane.GetComponent<Renderer>().material = matInstance;
        matInstance.SetTexture("_OverlayTex", inkInstance);
        matInstance.SetTexture("_OverlayPaperTex", paperInstance);
    }

    public void SetStaticColours(Color[] inkColours, Color[] paperColours)
    {
        staticInk = inkColours;
        staticPaper = paperColours;
    }

    private void Update()
    {
        Color[] inkColours = new Color[32 * 24];
        Color[] paperColours = new Color[32 * 24];

        Array.Fill(inkColours, White);
        if (staticInk != null)
        {
            Array.Copy(staticInk, 0, inkColours, 0, staticInk.Length);
        }

        Array.Fill(paperColours, Color.black);
        if (staticPaper != null)
        {
            Array.Copy(staticPaper, 0, paperColours, 0, staticPaper.Length);
        }

        inkInstance.SetPixels(inkColours);
        paperInstance.SetPixels(paperColours);
        inkInstance.Apply();
        paperInstance.Apply();
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Attrs", menuName = "ZX Spectrum/Attribute Run")]
public class AttributeRun : ScriptableObject
{
    public Vector2 position;
    public Vector2 size;
    public Color ink;
    public Color paper;
}

using UnityEngine;

public class HoistCar : MonoBehaviour
{
    [SerializeField] private GameObject[] parts;

    public void ShowAllParts()
    {
        foreach (var part in parts)
        {
            part.SetActive(true);
        }
    }

    public void AcceptPart(int index)
    {
        parts[index%parts.Length].gameObject.SetActive(true);
    }
}

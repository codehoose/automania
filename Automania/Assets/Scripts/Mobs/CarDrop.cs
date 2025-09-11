using UnityEngine;

public class CarDrop : MonoBehaviour
{
    private HoistCar hoistCarInstance;

    [SerializeField] private Transform carSpawnPoint;

    public Transform Car => carSpawnPoint;

    public void Init(HoistCar prefab)
    {
        hoistCarInstance = Instantiate(prefab, carSpawnPoint);
        hoistCarInstance.ShowAllParts();
    }
}

using UnityEngine;

public class HudController : MonoBehaviour
{
    [SerializeField] private TimeHudController timeControlller;
    [SerializeField] private CounterHudController scoreController;
    [SerializeField] private CounterHudController carsController;
    [SerializeField] private LivesHudController livesController;

    public TimeHudController Time => timeControlller;
    public CounterHudController Score => scoreController;
    public CounterHudController Cars => carsController;
    public LivesHudController Lives => livesController;
}

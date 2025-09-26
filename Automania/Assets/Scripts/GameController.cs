using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    private static GameController instance;

    public static GameController Instance
    {
        get
        {
            return instance;
        }
    }

    private float clock = 120f;
    private float cachedX;
    private float cachedY;
    private GlobalGameState gameState;
    private InputAction moveAction;
    private InputAction jumpAction;
    private CarDrop carDropInstance;

    [SerializeField] private LevelBuilder levelBuilder;
    [SerializeField] private HudController hudController;

    [Header("Prefabs")]
    [SerializeField] private CarDrop carDropPrefab;
    
    public HoistCar HoistCar
    {
        get => FindFirstObjectByType<HoistCar>();
    }

    public UnityEvent<float, float> MovePlayer;
    public UnityEvent JumpPlayer;

    public GlobalGameState State => gameState;

    public void ChangeRoom()
    {
        levelBuilder.ChangeRoom();
    }

    public float RegisterWally(UnityAction<float, float> listener, UnityAction jumpListener)
    {
        MovePlayer.AddListener(listener);
        JumpPlayer.AddListener(jumpListener);
        return cachedX;
    }

    private void UnregisterWally()
    {
        MovePlayer.RemoveAllListeners();
        JumpPlayer.RemoveAllListeners();
    }

    private void Awake()
    {
        gameState = new GlobalGameState();
        instance = this;
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        moveAction.performed += MoveAction_performed;
        moveAction.canceled += MoveAction_canceled;

        jumpAction.performed += JumpAction_Performed;

        carDropInstance = Instantiate(carDropPrefab, new Vector3(88, -96), Quaternion.identity);
        carDropInstance.Init(levelBuilder.HoistCar);
        carDropInstance.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameState.State == GameState.NormalGameplay)
        {
            if (gameState.AllPartsCollected)
            {
                DropCar();
            }
        }

        clock -= Time.deltaTime;
        hudController.Time.Current = clock / 120f;
    }

    public void EndLevel() => DropCar();


    private void DropCar()
    {
        gameState.State = GameState.DropCar;
        UnregisterWally();
        levelBuilder.PrepareCarDrop();
        StartCoroutine(DropTheCar());
    }

    IEnumerator DropTheCar()
    {
        carDropInstance.gameObject.SetActive(true);
        var start = carDropInstance.transform.position;
        var target = new Vector3(88, -152);

        float time = 0f;
        while (time < 1f)
        {
            var pos = Vector3.Lerp(start, target, time);
            var npos = new Vector3((int)pos.x, (int)pos.y);
            carDropInstance.transform.position = npos;
            time += Time.deltaTime / 2f;
            yield return null;
        }

        carDropInstance.transform.position = target;

        var targetTransform = carDropInstance.Car;
        start = targetTransform.position;
        target = new Vector3(-64, targetTransform.position.y);

        time = 0f;
        
        while (time < 1f)
        {
            var pos = Vector3.Lerp(start, target, time);
            var npos = new Vector3((int)pos.x, (int)pos.y);
            targetTransform.position = npos;
            time += Time.deltaTime / 2f;
            yield return null;
        }
        targetTransform.position = target;
    }

    private void JumpAction_Performed(InputAction.CallbackContext obj)
    {
        JumpPlayer?.Invoke();
    }

    private void OnDestroy()
    {
        MovePlayer.RemoveAllListeners();

        moveAction.performed -= MoveAction_performed;
        moveAction.canceled -= MoveAction_canceled;
    }

    private void MoveAction_canceled(InputAction.CallbackContext obj)
    {
        cachedX = 0;
        cachedY = 0;
        MovePlayer?.Invoke(0f, 0f);
    }

    private void MoveAction_performed(InputAction.CallbackContext obj)
    {
        var x = obj.ReadValue<Vector2>().x;
        var y = obj.ReadValue<Vector2>().y;

        cachedX = x != 0 ? Mathf.Sign(x) : 0;
        cachedY = y != 0 ? Mathf.Sign(y) : 0;
        MovePlayer?.Invoke(cachedX, cachedY);
    }
}

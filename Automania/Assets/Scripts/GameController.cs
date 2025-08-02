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

    private float cachedX;
    private float cachedY;
    private InputAction moveAction;
    private InputAction jumpAction;

    [SerializeField] private LevelBuilder levelBuilder;

    public UnityEvent<float, float> MovePlayer;

    public void ChangeRoom()
    {
        levelBuilder.ChangeRoom();
    }

    public float RegisterWally(UnityAction<float, float> listener)
    {
        MovePlayer.AddListener(listener);
        return cachedX;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        moveAction.performed += MoveAction_performed;
        moveAction.canceled += MoveAction_canceled;
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

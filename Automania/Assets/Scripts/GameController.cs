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
    private InputAction moveAction;
    private InputAction jumpAction;

    [SerializeField] private LevelBuilder levelBuilder;

    public UnityEvent<float> MovePlayer;

    public void ChangeRoom()
    {
        levelBuilder.ChangeRoom();
    }

    public float RegisterWally(UnityAction<float> listener)
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
        MovePlayer?.Invoke(0f);
    }

    private void MoveAction_performed(InputAction.CallbackContext obj)
    {
        cachedX = Mathf.Sign(obj.ReadValue<Vector2>().x);
        MovePlayer?.Invoke(cachedX);
    }
}

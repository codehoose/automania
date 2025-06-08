using UnityEngine;
using UnityEngine.InputSystem;

public class WallyMob : ZXMob
{
    private bool stopProcessingCollisions;

    private float x;
    private InputAction moveAction;
    private InputAction jumpAction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (stopProcessingCollisions) return;

        if (collision.tag == "Door")
        {
            stopProcessingCollisions = true;
            GameController.Instance.ChangeRoom();
        }
    }


    public override void Start()
    {
        base.Start();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        moveAction.performed += MoveAction_performed;
        moveAction.canceled += MoveAction_canceled;
    }

    private void OnDestroy()
    {
        moveAction.performed -= MoveAction_performed;
        moveAction.canceled -= MoveAction_canceled;
    }

    private void MoveAction_canceled(InputAction.CallbackContext obj)
    {
        x = 0;
    }

    private void MoveAction_performed(InputAction.CallbackContext obj)
    {
        x = Mathf.Sign(obj.ReadValue<Vector2>().x);
    }

    public override void FixedUpdate()
    {
        pos.x += x * 32f * Time.deltaTime;
        base.FixedUpdate();
    }
}

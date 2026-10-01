using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    
    [SerializeField] float speed;

    Vector3 moveXYZ;

    InputActions inputActions;

    void Start()
    {
        
    }

    
    void Update()
    {
        MovePlayer();
    }

    void Awake()
    {
        inputActions = new InputActions();

        inputActions.Player.Move.performed += ctx => moveXYZ = ctx.ReadValue<Vector3>();
        inputActions.Player.Move.canceled += _ => moveXYZ = Vector3.zero;
    }

    void MovePlayer()
    {
        Vector3 displacement = new Vector3(-moveXYZ.x, 0f, moveXYZ.z) * speed * Time.deltaTime;
        //Vector3 newPosition = transform.position += displacement;
        transform.Translate(displacement);
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
}

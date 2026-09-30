using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    [SerializeField] float speed;
    Vector3 moveXYZ;

    


    void Start()
    {
        
    }

    
    void Update()
    {
        
    }


    private InputAction inputActions; // Fíjate que es "InputAction" sin la "s" al final y sin espacios

    private void Awake()
    {
        // Instancia la clase con el nombre exacto de tu archivo
        inputActions = new InputAction();

        // MOVIMIENTO
        inputActions.Player.Move.performed += ctx => moveXYZ = ctx.ReadValue<Vector3>();
        inputActions.Player.Move.canceled += ctx => moveXYZ = Vector3.zero;
    }


    void MovePlayer()
    { 
    
       Vector3 displacement = new Vector3 (moveXYZ.x, moveXYZ.y, moveXYZ.z) * speed * Time.deltaTime;
       Vector3 newPosition = transform.position + displacement;

    }





    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

}

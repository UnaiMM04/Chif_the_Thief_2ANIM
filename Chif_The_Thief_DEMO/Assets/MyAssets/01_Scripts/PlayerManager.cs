using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    
    [SerializeField] float speed;
    [SerializeField] public float rotationSpeed = 100f; //SENSIVILIDAD DEL MANDO PARA ROTAR EL JUGADOR
    [SerializeField] Transform playerCamera;   //PARA CONTROLAR LA CAMARA

    Vector3 moveXYZ;
    Vector2 rotateXY;

    float xRotation = 0f; // Para acumular y limitar la rotación vertical de la cámara

    InputActions inputActions;

    void Start()
    {
        //VISIVILIDAD Y BLOQUEO DEL CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
        MovePlayer();
        RotatePlayer();
    }

    void Awake()
    {
        inputActions = new InputActions();

        

        //MOVIMIENTO DEL JUGADOR
        inputActions.Player.PlayerMove.performed += ctx => moveXYZ = ctx.ReadValue<Vector3>();
        inputActions.Player.PlayerMove.canceled += _ => moveXYZ = Vector3.zero;

        //ROTACIÓN DEL JUGADOR
        inputActions.Player.PlayerRotate.performed += ctx => rotateXY = ctx.ReadValue<Vector2>();
        inputActions.Player.PlayerRotate.canceled += _ => rotateXY = Vector2.zero;
    }

    void MovePlayer()
    {
        Vector3 displacement = new Vector3(moveXYZ.x, 0f, moveXYZ.z) * speed * Time.deltaTime;
        
        transform.Translate(displacement);
 
    }

    void RotatePlayer()
    {
        

        // 1. ROTACIÓN HORIZONTAL 
        float horizontalRotation = rotateXY.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, horizontalRotation, Space.World);

        // 2. ROTACIÓN VERTICAL SOLO DE LA CÁMARA 
        if (playerCamera != null)
        {
            float verticalRotation = rotateXY.y * rotationSpeed * Time.deltaTime;

            xRotation -= verticalRotation; 
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // LIMITES CAMARA ARRIBA Y ABAJO

            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }


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

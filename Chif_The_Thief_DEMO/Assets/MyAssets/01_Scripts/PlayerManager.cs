using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    
    [SerializeField] float speed;
    [SerializeField] public float rotationSpeed = 100f; // Sube un poco este valor si va muy lento con el mando
    [SerializeField] Transform playerCamera;   // Arrastra aquí la cámara hija desde el inspector

    Vector3 moveXYZ;
    Vector2 rotateXY;

    float xRotation = 0f; // Para acumular y limitar la rotación vertical de la cámara

    InputActions inputActions;

    void Start()
    {
        
    }

    
    void Update()
    {
        MovePlayer();
        RotatePlayer();
    }

    void Awake()
    {
        inputActions = new InputActions();

        inputActions.Player.PlayerMove.performed += ctx => moveXYZ = ctx.ReadValue<Vector3>();
        inputActions.Player.PlayerMove.canceled += _ => moveXYZ = Vector3.zero;

        //inputActions.Player.PlayerRotate.performed += ctx => moveXYZ = ctx.ReadValue<Vector2>();
        //inputActions.Player.PlayerRotate.canceled += _ => moveXYZ = Vector2.zero;

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
        //Vector2 rotation = new Vector2(rotateXY.x, rotateXY.y) * rotationSpeed * Time.deltaTime;
        //transform.Rotate(rotation);

        // 1. ROTACIÓN HORIZONTAL (Izquierda / Derecha) -> Rota la cápsula entera
        float horizontalRotation = rotateXY.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, horizontalRotation, Space.World);


        // 2. ROTACIÓN VERTICAL (Arriba / Abajo) -> Rota solo la cámara y limitamos el ángulo
        if (playerCamera != null)
        {
            float verticalRotation = rotateXY.y * rotationSpeed * Time.deltaTime;

            xRotation -= verticalRotation; // Restamos para que el eje Y del mando sea natural
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Evita que la cámara dé la vuelta completa

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

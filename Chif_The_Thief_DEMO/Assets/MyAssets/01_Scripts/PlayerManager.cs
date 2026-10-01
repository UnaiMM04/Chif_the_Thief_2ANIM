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

    [SerializeField] float jumpForce = 5f;       // Fuerza del salto
    [SerializeField] float gravity = -9.81f;     // Gravedad personalizada
    private float verticalVelocity;              // Velocidad vertical actual

    private CharacterController characterController; // Recomendado para manejar físicas y suelo

    InputActions inputActions;

    void Start()
    {
        //VISIVILIDAD Y BLOQUEO DEL CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Obtenemos o añadimos automáticamente el CharacterController
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogWarning("Se recomienda añadir un componente CharacterController al Player para que funcionen las colisiones y el suelo.");
        }


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

        inputActions.Player.Salto.performed += _ => Jump();
    }

    void MovePlayer()
    {
        // Movimiento horizontal relativo a la orientación del jugador
        Vector3 move = transform.right * moveXYZ.x + transform.forward * moveXYZ.z;
        Vector3 displacement = move * speed;

        // 2. Gestión de la gravedad
        if (characterController != null && characterController.isGrounded)
        {
            // Si está en el suelo, mantenemos una pequeña fuerza negativa para que se pegue bien
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            // Si está en el aire, aplicamos gravedad con el tiempo
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Añadimos la velocidad vertical al desplazamiento
        displacement.y = verticalVelocity;

        // 3. Aplicar movimiento (usamos CharacterController si existe, o Translate como tenías)
        if (characterController != null)
        {
            characterController.Move(displacement * Time.deltaTime);
        }
        else
        {
            transform.Translate(displacement * Time.deltaTime, Space.World);
        }

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

    void Jump()
    {
        // Solo saltamos si estamos en el suelo (requiere CharacterController)
        if (characterController != null && characterController.isGrounded)
        {
            // Fórmula física para calcular la velocidad de salto instantánea
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
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

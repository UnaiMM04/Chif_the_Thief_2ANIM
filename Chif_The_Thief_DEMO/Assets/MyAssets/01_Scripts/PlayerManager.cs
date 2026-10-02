using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    //VELOCIDADES
    [SerializeField] float speed;

    //SENSIBILIDAD DEL MANDO PARA ROTAR EL JUGADOR
    [Range(50f, 100f)] [SerializeField] public float rotationSpeed = 75f; //RANGO DE SENSIBILIDAD DEL MANDO PARA ROTAR EL JUGADOR Y SENS INICIAL DE 75
    
    //CAMARA
    [SerializeField] Transform playerCamera; //PARA CONTROLAR LA CAMARA

    //MOVIMIENTO
    Vector3 moveXYZ;
   
    //ROTACION
    Vector2 rotateXY;
    float xRotation = 0f; // Para acumular y limitar la rotación vertical de la cámara

    //SALTO Y GRAVEDAD
    [SerializeField] float jumpForce = 5f;       // Fuerza del salto
    [SerializeField] float gravity = -9.81f;     // Gravedad personalizada
    private float verticalVelocity;              // Velocidad vertical actual
        //CHARACTER CONTROLLER
        private CharacterController characterController; // Recomendado para manejar físicas y suelo

    //INPUTS
    InputActions inputActions;

    void Start()
    {
        //VISIBILIDAD Y BLOQUEO DEL CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //ESTO ES PARA QUE EL FUNCIONEN BIEN LAS FISICAS DEL JUGADOR, COMO EL SALTO Y LA GRAVEDAD
        characterController = GetComponent<CharacterController>();
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
        // 1. Movimiento horizontal basado en la velocidad base
        Vector3 move = transform.right * moveXYZ.x + transform.forward * moveXYZ.z;
        Vector3 displacement = move * speed;

        // 2. Gestión de la gravedad
        if (characterController != null && characterController.isGrounded)
        {
            // Si está en el suelo, mantenemos una pequeña fuerza negativa constante para que no flote en pendientes
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            // Si está en el aire, acumulamos la gravedad correctamente con el tiempo
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 3. Unimos el desplazamiento horizontal con la velocidad vertical actual
        displacement.y = verticalVelocity;

        // 4. Aplicamos el movimiento multiplicando por Time.deltaTime UNA SOLA VEZ
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






using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCharacterController : MonoBehaviour
{
    private const float DefaultMoveSpeed = 5f;
    private const float DefaultJumpForce = 5f;
    private const float DefaultMouseSensitivity = 2f;
    private const float DefaultMinVerticalAngle = -80f;
    private const float DefaultMaxVerticalAngle = 80f;
    private const float DefaultGroundCheckDistance = 0.1f;
    private const float DefaultStepInterval = 0.5f;
    private const float DefaultSoundStopDelay = 0.2f;
    private const float DefaultMinPitch = 0.9f;
    private const float DefaultMaxPitch = 1.1f;
    private const float DefaultMovementThreshold = 0.1f;
    private const string DefaultSurfaceTypeName = "Default";

    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = DefaultMoveSpeed;
    [SerializeField] private float jumpForce = DefaultJumpForce;

    [Header("Настройки поворота мыши")]
    [SerializeField] private float mouseSensitivity = DefaultMouseSensitivity;
    [SerializeField] private float minVerticalAngle = DefaultMinVerticalAngle;
    [SerializeField] private float maxVerticalAngle = DefaultMaxVerticalAngle;

    [Header("Ссылки")]
    [SerializeField] private Transform cameraTransform;

    [Header("Настройки земли")]
    [SerializeField] private float groundCheckDistance = DefaultGroundCheckDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("Настройки звуков ходьбы")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private FootstepSoundManager footstepSoundManager;
    [SerializeField] private float stepInterval = DefaultStepInterval;
    [SerializeField] private float soundStopDelay = DefaultSoundStopDelay;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float yRotation = 0f;
    private float xRotation = 0f;

    private InputSystem_Actions inputActions;

    private float stepTimer = 0f;
    private float stopTimer = 0f;
    private bool isWalking = false;
    private string currentSurfaceType = "";
    private RaycastHit groundHit;
    
    void Awake()
    {
        inputActions = new InputSystem_Actions();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += OnJump;
    }
    
    void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    
    void Update()
    {
        CheckGround();
        HandleLook();
        HandleMovement();
    }
    
    void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out groundHit,
                                     transform.localScale.y + groundCheckDistance,
                                     groundLayer);
    }
    
    void HandleLook()
    {
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();
        
        // Поворот персонажа по горизонтали
        yRotation += lookInput.x * mouseSensitivity;
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        
        // Поворот камеры по вертикали
        if (cameraTransform != null)
        {
            xRotation -= lookInput.y * mouseSensitivity;
            xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
    
    void HandleMovement()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        // Движение относительно направления взгляда персонажа
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        moveDirection.y = 0f;
        moveDirection = moveDirection.normalized * moveSpeed * Time.deltaTime;

        rb.MovePosition(transform.position + moveDirection);

        // Обработка звуков шагов
        if (isGrounded && moveInput.magnitude > DefaultMovementThreshold)
        {
            isWalking = true;
            stopTimer = soundStopDelay;

            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstepSound();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;

            if (isWalking)
            {
                stopTimer -= Time.deltaTime;
                if (stopTimer <= 0f)
                {
                    StopFootstepSound();
                    isWalking = false;
                }
            }
        }
    }
    
    void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void PlayFootstepSound()
    {
        if (audioSource == null || footstepSoundManager == null || !isGrounded) return;

        // Получаем информацию о поверхности
        string surfaceType = GetSurfaceType();

        // Получаем звук для текущей поверхности
        AudioClip footstepClip = footstepSoundManager.GetFootstepSound(surfaceType);

        if (footstepClip != null)
        {
            audioSource.pitch = Random.Range(DefaultMinPitch, DefaultMaxPitch);
            audioSource.PlayOneShot(footstepClip);
        }
    }

    void StopFootstepSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    string GetSurfaceType()
    {
        if (groundHit.collider == null) return DefaultSurfaceTypeName;

        // Пытаемся получить тип поверхности из компонента
        SurfaceType surfaceComponent = groundHit.collider.GetComponent<SurfaceType>();
        if (surfaceComponent != null)
        {
            return surfaceComponent.GetSurfaceTypeName();
        }

        // Пытаемся определить по материалу
        Renderer renderer = groundHit.collider.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            return renderer.material.name.Replace(" (Instance)", "");
        }

        // Пытаемся определить по тегу
        if (!string.IsNullOrEmpty(groundHit.collider.tag))
        {
            return groundHit.collider.tag;
        }

        return DefaultSurfaceTypeName;
    }
}
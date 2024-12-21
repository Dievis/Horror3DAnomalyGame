using UnityEngine;
using UnityEngine.UI; // Import UI namespace

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraHeightOffset = 0.665f; // Thêm độ dời cho camera

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 15f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float staminaRegenDelay = 2f;

    [Header("UI Settings")]
    [SerializeField] private Image staminaBar; // Reference to the UI Image for stamina bar
    [SerializeField] private GameObject playerHUD; // Reference to the player's HUD


    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

    private float currentStamina;
    private float staminaRegenTimer;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentStamina = maxStamina;

        // Hiển thị HUD cho người chơi
        playerHUD.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;

        UpdateStaminaUI(); // Initialize UI

        // Điều chỉnh Near Clipping Plane của camera
        cameraTransform.GetComponent<Camera>().nearClipPlane = 0.1f;
        // Kích hoạt Occlusion Culling
        cameraTransform.GetComponent<Camera>().useOcclusionCulling = true;

        // Thiết lập độ dời camera cho phù hợp với chiều cao mắt của player
        cameraHeightOffset = 0.665f;
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleCameraCollision(); // Kiểm tra va chạm của camera

        // Cập nhật vị trí của camera với độ dời chiều cao
        Vector3 cameraPosition = cameraTransform.localPosition;
        cameraPosition.y = cameraHeightOffset;
        cameraTransform.localPosition = cameraPosition;
    }

    private void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset falling velocity
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Update stamina
        if (isRunning)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0);
            staminaRegenTimer = 0f; // Reset stamina regen timer
        }
        else if (currentStamina < maxStamina)
        {
            staminaRegenTimer += Time.deltaTime;
            if (staminaRegenTimer >= staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }
        }

        UpdateStaminaUI(); // Cập nhật UI

        characterController.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, -50f); // Limit falling speed
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleCameraCollision()
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position, cameraTransform.position, out hit))
        {
            cameraTransform.position = hit.point;
        }
    }

    private void UpdateStaminaUI()
    {
        if (staminaBar != null)
        {
            staminaBar.fillAmount = currentStamina / maxStamina;
        }
    }
}

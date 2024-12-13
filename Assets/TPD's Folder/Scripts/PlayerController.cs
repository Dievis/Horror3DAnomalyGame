using Photon.Pun;
using UnityEngine;
using UnityEngine.UI; // Import UI namespace

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform cameraTransform;

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

        if (photonView.IsMine)
        {
            // Chỉ hiển thị HUD cho player này
            playerHUD.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // Ẩn HUD của các player khác
            playerHUD.SetActive(false);
            cameraTransform.gameObject.SetActive(false);
        }

        UpdateStaminaUI(); // Initialize UI
    }

    private void Update()
    {
        // Kiểm tra xem trò chơi có đang ở chế độ Multiplayer không
        if (PhotonNetwork.IsConnected)
        {
            if (photonView.IsMine) // Nếu là của người chơi hiện tại
            {
                HandleMovement();
                HandleMouseLook();
            }
        }
        else
        {
            // Xử lý chuyển động và UI cho chế độ Singleplayer (không có Photon)
            HandleMovement();
            HandleMouseLook();
        }
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

            // Đồng bộ trạng thái stamina cho các máy khách trong multiplayer
            if (PhotonNetwork.IsConnected && photonView.IsMine)
            {
                photonView.RPC("UpdateStamina", RpcTarget.Others, currentStamina);  // Cập nhật stamina cho các máy khách khác
            }
        }
        else if (currentStamina < maxStamina)
        {
            staminaRegenTimer += Time.deltaTime;
            if (staminaRegenTimer >= staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);

                // Đồng bộ trạng thái stamina cho các máy khách trong multiplayer
                if (PhotonNetwork.IsConnected && photonView.IsMine)
                {
                    photonView.RPC("UpdateStamina", RpcTarget.Others, currentStamina);  // Cập nhật stamina cho các máy khách khác
                }
            }
        }

        UpdateStaminaUI(); // Cập nhật UI trên máy khách hiện tại

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
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void UpdateStaminaUI()
    {
        if (staminaBar != null)
        {
            staminaBar.fillAmount = currentStamina / maxStamina;
        }
    }

    // RPC để đồng bộ stamina với các máy khách khác trong Multiplayer
    [PunRPC]
    private void UpdateStamina(float newStamina)
    {
        currentStamina = newStamina;
        UpdateStaminaUI();
    }
}

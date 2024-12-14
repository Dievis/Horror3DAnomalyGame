//SingleplayerController

using DACNNEWMAP.Manager;
using UnityEngine;
using UnityEngine.UI; // Thêm UI namespace

namespace DACNNEWMAP.PlayerControl
{
    public class SingleplayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        [SerializeField, Range(10, 500)] private float JumpFactor = 260f;
        [SerializeField] private float Dis2Ground = 0.8f;
        [SerializeField] private LayerMask GroundCheck;
        [SerializeField] private float AirResistance = 0.8f;

        [Header("Stamina Settings")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaDrainRate = 15f;
        [SerializeField] private float staminaRegenRate = 10f;
        [SerializeField] private float staminaRegenDelay = 2f;
        [SerializeField] private Image staminaBar; // UI Image cho stamina
        [SerializeField] private GameObject playerHUD; // UI HUD của player

        private Rigidbody _playerRigidbody;
        private InputManager _inputManager;
        private Animator _animator;
        private bool _grounded = false;
        private bool _hasAnimator;
        private int _xVelHash;
        private int _yVelHash;
        private int _jumpHash;
        private int _groundHash;
        private int _fallingHash;
        private int _zVelHash;
        private int _crouchHash;
        private float _xRotation;

        private const float _walkSpeed = 2f;
        private const float _runSpeed = 6f;
        private Vector2 _currentVelocity;

        private float currentStamina;
        private float staminaRegenTimer;

        private void Start()
        {
            _hasAnimator = TryGetComponent<Animator>(out _animator);
            _playerRigidbody = GetComponent<Rigidbody>();
            _inputManager = GetComponent<InputManager>();

            _xVelHash = Animator.StringToHash("X_Velocity");
            _yVelHash = Animator.StringToHash("Y_Velocity");
            _zVelHash = Animator.StringToHash("Z_Velocity");
            _jumpHash = Animator.StringToHash("Jump");
            _groundHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");
            _crouchHash = Animator.StringToHash("Crouch");

            currentStamina = maxStamina;

            // Chỉ hiển thị HUD cho player này
            playerHUD.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;

            UpdateStaminaUI();
        }

        private void FixedUpdate()
        {
            SampleGround();
            Move();
            HandleJump();
            HandleCrouch();
        }

        private void LateUpdate()
        {
            if (Camera == null)
            {
                Debug.LogWarning("Camera is missing or destroyed!");
                return;
            }

            CamMovements();
        }

        private void Move()
        {
            if (!_hasAnimator) return;

            float targetSpeed = _walkSpeed;  // Mặc định là tốc độ đi bộ

            if (_inputManager.Run && currentStamina > 0) // Nếu còn stamina và đang chạy
            {
                targetSpeed = _runSpeed; // Chạy với tốc độ run
                currentStamina -= staminaDrainRate * Time.deltaTime;  // Giảm stamina khi chạy
                currentStamina = Mathf.Max(currentStamina, 0);
                staminaRegenTimer = 0f; // Reset timer regen stamina
            }
            else if (!_inputManager.Run && currentStamina < maxStamina) // Nếu không chạy và stamina chưa đầy
            {
                // Hồi phục stamina khi không chạy
                staminaRegenTimer += Time.deltaTime;
                if (staminaRegenTimer >= staminaRegenDelay)
                {
                    currentStamina += staminaRegenRate * Time.deltaTime;
                    currentStamina = Mathf.Min(currentStamina, maxStamina);
                }
            }

            // Khi hết stamina, không thể chạy nữa
            if (currentStamina <= 0)
            {
                targetSpeed = _walkSpeed;  // Giới hạn tốc độ di chuyển
            }

            UpdateStaminaUI();  // Cập nhật UI stamina

            _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, _inputManager.Move.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
            _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, _inputManager.Move.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

            var xVelDifference = _currentVelocity.x - _playerRigidbody.velocity.x;
            var zVelDifference = _currentVelocity.y - _playerRigidbody.velocity.z;

            _playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);

            // Cập nhật animation "run" và "walk" dựa vào tốc độ
            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
            _animator.SetBool("IsRunning", _inputManager.Run && currentStamina > 0);  // Điều khiển animation chạy
        }


        private void CamMovements()
        {
            if (!_hasAnimator) return;

            var Mouse_X = _inputManager.Look.x;
            var Mouse_Y = _inputManager.Look.y;
            Camera.position = CameraRoot.position;

            _xRotation -= Mouse_Y * MouseSensitivity * Time.smoothDeltaTime;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);

            Camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(0, Mouse_X * MouseSensitivity * Time.smoothDeltaTime, 0));
        }

        private void HandleCrouch() => _animator.SetBool(_crouchHash, _inputManager.Crouch);

        private void HandleJump()
        {
            if (!_hasAnimator || !_inputManager.Jump || !_grounded) return;

            _animator.SetTrigger(_jumpHash);
        }

        public void JumpAddForce()
        {
            _playerRigidbody.AddForce(-_playerRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
            _playerRigidbody.AddForce(Vector3.up * JumpFactor, ForceMode.Impulse);
            _animator.ResetTrigger(_jumpHash);
        }

        private void SampleGround()
        {
            if (!_hasAnimator) return;

            RaycastHit hitInfo;
            if (Physics.Raycast(_playerRigidbody.worldCenterOfMass, Vector3.down, out hitInfo, Dis2Ground + 0.1f, GroundCheck))
            {
                _grounded = true;
                SetAnimationGrounding();
                return;
            }

            _grounded = false;
            _animator.SetFloat(_zVelHash, _playerRigidbody.velocity.y);
            SetAnimationGrounding();
        }

        private void SetAnimationGrounding()
        {
            _animator.SetBool(_fallingHash, !_grounded);
            _animator.SetBool(_groundHash, _grounded);
        }

        private void UpdateStaminaUI()
        {
            if (staminaBar != null)
            {
                staminaBar.fillAmount = currentStamina / maxStamina;
            }
        }

    }
}

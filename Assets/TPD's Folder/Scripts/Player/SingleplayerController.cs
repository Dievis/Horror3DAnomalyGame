using DACNNEWMAP.Manager;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private float MouseSensitivity = 15f;
        [SerializeField] private float _walkSpeed = 4f;
        [SerializeField] private float _runSpeed = 12f;

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
        private float _xRotation;

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

            currentStamina = maxStamina;

            // Chỉ hiển thị HUD cho player này
            playerHUD.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;

            _playerRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            UpdateStaminaUI();
        }

        private void FixedUpdate()
        {
            LimitRigidbodySpeed();
            SampleGround();
            Move();
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

            float targetSpeed = _walkSpeed;

            if (_inputManager.Run && currentStamina > 0)
            {
                targetSpeed = _runSpeed;
                currentStamina -= staminaDrainRate * Time.deltaTime;
                currentStamina = Mathf.Max(currentStamina, 0);
                staminaRegenTimer = 0f;
            }
            else if (!_inputManager.Run && currentStamina < maxStamina)
            {
                staminaRegenTimer += Time.deltaTime;
                if (staminaRegenTimer >= staminaRegenDelay)
                {
                    currentStamina += staminaRegenRate * Time.deltaTime;
                    currentStamina = Mathf.Min(currentStamina, maxStamina);
                }
            }

            if (currentStamina <= 0)
            {
                targetSpeed = _walkSpeed;
            }

            UpdateStaminaUI();

            Vector3 moveDirection = transform.TransformDirection(new Vector3(_inputManager.Move.x, 0, _inputManager.Move.y));
            _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, _inputManager.Move.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
            _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, _inputManager.Move.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

            var xVelDifference = _currentVelocity.x - _playerRigidbody.velocity.x;
            var zVelDifference = _currentVelocity.y - _playerRigidbody.velocity.z;

            Vector3 force = transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference));
            force = Vector3.ClampMagnitude(force, targetSpeed);

            if (_grounded)
            {
                _playerRigidbody.AddForce(force, ForceMode.VelocityChange);
            }
            else
            {
                _playerRigidbody.AddForce(force * 0.3f, ForceMode.VelocityChange);
            }

            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
            _animator.SetBool("IsRunning", _inputManager.Run && currentStamina > 0);
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

        private void SampleGround()
        {
            if (!_hasAnimator) return;

            float raycastLength = 1.2f;
            float sphereRadius = 0.3f;
            Vector3 rayOrigin = _playerRigidbody.position + Vector3.up * 0.1f;

            // Sử dụng Raycast để kiểm tra mặt đất
            if (Physics.SphereCast(rayOrigin, sphereRadius, Vector3.down, out RaycastHit hitInfo, raycastLength))
            {
                _grounded = true;

                // Đảm bảo không áp lực bay theo đường chéo trên cầu thang
                if (Vector3.Angle(hitInfo.normal, Vector3.up) <= 45f) // Kiểm tra góc nghiêng <= 45 độ
                {
                    // Tính toán lực di chuyển trên mặt phẳng
                    Vector3 adjustedForce = Vector3.ProjectOnPlane(_playerRigidbody.velocity, hitInfo.normal);
                    _playerRigidbody.velocity = adjustedForce;
                }

                SetAnimationGrounding();
                return;
            }

            _grounded = false;
            _animator.SetFloat(_yVelHash, _playerRigidbody.velocity.y);
            SetAnimationGrounding();
        }


        private void SetAnimationGrounding()
        {
            _animator.SetBool("Falling", !_grounded);
            _animator.SetBool("Grounded", _grounded);
        }

        private void UpdateStaminaUI()
        {
            if (staminaBar != null)
            {
                staminaBar.fillAmount = currentStamina / maxStamina;
            }
        }

        private void LimitRigidbodySpeed()
        {
            Vector3 velocity = _playerRigidbody.velocity;
            float maxSpeed = _runSpeed * 1.5f;

            if (velocity.magnitude > maxSpeed)
            {
                _playerRigidbody.velocity = velocity.normalized * maxSpeed;
            }
        }
    }
}

using UnityEngine;

namespace Assets.Scripts.Dev
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        // --- Camera References ---
        [Header("Camera References")]
        public Camera playerCamera;
        public Transform cameraPivot; // The child object that handles FP vertical look

        // --- Movement Settings ---
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        // Add a field for Gravity (required when using CharacterController)
        public float gravity = -9.81f;

        // --- First Person Look Settings ---
        [Header("Look Settings")]
        public float mouseSensitivity = 100f;
        public float yRotationLimit = 85f; // Clamps the vertical look

        // --- State Management ---        
        private float _verticalLookRotation = 0f;
        private Vector3 _velocity; // Stores vertical velocity (for jumping/gravity)
        private CharacterController _controller;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (_controller == null)
            {
                Debug.LogError($"PlayerController requires CharacterController component on {gameObject.name}");
                enabled = false;
                return;
            }
        }

        void Start()
        {
            // Lock and hide the cursor for first-person control
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ResetPlayerPosition();
                return;
            }

            if (CanHandleInput())
            {
                HandleMouseLook();
                HandleMovement();
            }
        }

        private bool CanHandleInput()
        {
            return playerCamera != null && playerCamera.enabled && cameraPivot != null;
        }

        private void ResetPlayerPosition()
        {
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            _velocity = Vector3.zero;
            _verticalLookRotation = 0f;
        }

        void HandleMovement()
        {
            float currentSpeed =
            Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)
            ? moveSpeed * 2f : moveSpeed;

            // Check if player is on the ground. This resets vertical velocity.
            if (_controller.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f; // Small downward force to ensure isGrounded stays true
            }

            // Get standard input axes
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // Calculate horizontal movement direction relative to the player's current forward vector
            Vector3 horizontalMove = transform.right * x + transform.forward * z;

            // Apply horizontal movement using the CharacterController's safe Move method
            _controller.Move(currentSpeed * Time.deltaTime * horizontalMove);

            // Apply Gravity
            _velocity.y += gravity * Time.deltaTime;

            // Apply vertical movement (gravity/jumping)
            _controller.Move(_velocity * Time.deltaTime);
        }

        // --- Camera Look Logic (First Person Only) ---
        void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // 1. Vertical Look (Pitch) on the Camera Pivot
            _verticalLookRotation -= mouseY;
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -yRotationLimit, yRotationLimit);
            cameraPivot.localRotation = Quaternion.Euler(_verticalLookRotation, 0f, 0f);

            // 2. Horizontal Look (Yaw) on the Player Root
            transform.Rotate(Vector3.up * mouseX);
        }

    }
}
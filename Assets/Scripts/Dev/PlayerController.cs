using UnityEngine;

namespace Assets.Scripts.Dev
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        // Add a private reference for the CharacterController
        private CharacterController _controller;

        // --- Camera References ---
        [Header("Camera References")]
        public Camera playerCamera;
        public Transform cameraPivot; // The child object that handles FP vertical look

        // --- Movement Settings ---
        [Header("Movement Settings")]
        public float moveSpeed = 5f;

        // Add a field for Gravity (required when using CharacterController)
        public float gravity = -9.81f;
        private Vector3 velocity; // Stores vertical velocity (for jumping/gravity)

        // --- First Person Look Settings ---
        [Header("Look Settings")]
        public float mouseSensitivity = 100f;
        public float yRotationLimit = 85f; // Clamps the vertical look

        // --- State Management ---        
        private float verticalLookRotation = 0f;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
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
                transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                return;
            }

            if (playerCamera.enabled && cameraPivot != null)
            {
                HandleMouseLook();
                HandleMovement();
            }
        }

        void HandleMovement()
        {
            float currentSpeed =
            Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)
            ? moveSpeed * 2f : moveSpeed;

            // Check if player is on the ground. This resets vertical velocity.
            if (_controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Small downward force to ensure isGrounded stays true
            }

            // Get standard input axes
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // Calculate horizontal movement direction relative to the player's current forward vector
            Vector3 horizontalMove = transform.right * x + transform.forward * z;

            // Apply horizontal movement using the CharacterController's safe Move method
            _controller.Move(currentSpeed * Time.deltaTime * horizontalMove);

            // Apply Gravity
            velocity.y += gravity * Time.deltaTime;

            // Apply vertical movement (gravity/jumping)
            _controller.Move(velocity * Time.deltaTime);
        }

        // --- Camera Look Logic (First Person Only) ---
        void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // 1. Vertical Look (Pitch) on the Camera Pivot
            verticalLookRotation -= mouseY;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -yRotationLimit, yRotationLimit);
            cameraPivot.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);

            // 2. Horizontal Look (Yaw) on the Player Root
            transform.Rotate(Vector3.up * mouseX);
        }

    }
}
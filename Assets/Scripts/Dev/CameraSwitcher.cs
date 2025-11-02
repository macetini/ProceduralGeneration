using Assets.Scripts.Dev.Meta;
using UnityEngine;

namespace Assets.Scripts.Dev
{
    public class CameraSwitcher : MonoBehaviour
    {
        [Header("Camera References")]
        public Camera TopDownCamera;
        public Camera FpsCamera;

        private CameraMode currentMode = CameraMode.TopDown;

        void Start()
        {
            SwitchCamera(currentMode);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                int nextModeIndex = ((int)currentMode + 1) % 2;
                SwitchCamera((CameraMode)nextModeIndex);
            }
        }

        void SwitchCamera(CameraMode newMode)
        {
            if (FpsCamera == null || TopDownCamera == null)
            {
                Debug.LogError("CameraSwitcher:: Camera references are missing in the Inspector!");
                return;
            }

            // Disable all cameras
            FpsCamera.enabled = false;
            TopDownCamera.enabled = false;

            // Enable the selected camera and manage cursor state
            switch (newMode)
            {
                case CameraMode.FirstPerson:
                    FpsCamera.enabled = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    break;

                case CameraMode.TopDown:
                    TopDownCamera.enabled = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
            }

            currentMode = newMode;
            Debug.Log("Camera Switched to: " + newMode);
        }
    }
}

using UnityEngine;

namespace Assets.Scripts.Dev
{

    public class CameraSwitcher : MonoBehaviour
    {
        [Header("Camera References")]
        public Camera TopDownCamera;
        public Camera FpsCamera;

        public enum CameraMode { TopDown, FirstPerson }
        private CameraMode currentMode = CameraMode.TopDown;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SwitchCamera(currentMode);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C)) // Use 'C' to cycle cameras
            {
                // Calculate the next camera mode
                int nextModeIndex = ((int)currentMode + 1) % 2;
                SwitchCamera((CameraMode)nextModeIndex);
            }
        }

        void SwitchCamera(CameraMode newMode)
        {
            // Disable all cameras
            FpsCamera.enabled = false;
            TopDownCamera.enabled = false;
            // Enable the selected camera
            switch (newMode)
            {
                case CameraMode.FirstPerson:
                    FpsCamera.enabled = true;
                    break;
                case CameraMode.TopDown:
                    TopDownCamera.enabled = true;
                    break;
            }

            currentMode = newMode;
            Debug.Log("Camera Switched to: " + newMode);
        }
    }
}

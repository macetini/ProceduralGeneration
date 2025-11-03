using Assets.Scripts.Dev.Meta;
using Assets.Scripts.Generators.Zone;
using UnityEngine;

namespace Assets.Scripts.Dev
{
    public class CameraSwitcher : MonoBehaviour
    {
        [Header("Camera References")]
        public Camera TopDownCamera;
        public Camera FpsCamera;
        public GameObject spawnZone;

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
                    if (spawnZone != null)
                    {
                        spawnZone.SetActive(false);
                    }
                    break;

                case CameraMode.TopDown:
                    TopDownCamera.enabled = true;
                    Cursor.lockState = CursorLockMode.None;
                    if (spawnZone != null)
                    {
                        spawnZone.SetActive(true);
                    }
                    break;
            }

            currentMode = newMode;
            Debug.Log("Camera Switched to: " + newMode);
        }
    }
}

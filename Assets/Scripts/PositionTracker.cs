using UnityEngine;

public class GyroCameraZRotation : MonoBehaviour
{
    public Camera mainCamera; // Assign the main camera in the inspector or use Camera.main in Start
    private bool rotate = false; // Control rotation
    private bool isRotationConfigured = false; // Track if rotation has been configured

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Enable the gyroscope on supported devices
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else
        {
            Debug.LogWarning("Gyroscope not supported on this device.");
        }
    }

    void Update()
    {
        // Rotate only if rotation is configured and the rotate flag is set to true
        if (isRotationConfigured && rotate && Input.gyro.enabled)
        {
            Quaternion deviceRotation = Input.gyro.attitude;
            Quaternion correctedRotation = new Quaternion(deviceRotation.x, deviceRotation.y, -deviceRotation.z, -deviceRotation.w);
            float zRotation = correctedRotation.eulerAngles.z;
            mainCamera.transform.rotation = Quaternion.Euler(mainCamera.transform.rotation.eulerAngles.x, mainCamera.transform.rotation.eulerAngles.y, zRotation);
        }
    }

    // Method to toggle the rotate flag
    public void ToggleRotation()
    {
        rotate = !rotate;
        isRotationConfigured = true; // Mark as configured after first call
        Debug.Log("Rotation toggled. New state: " + rotate);
    }
}

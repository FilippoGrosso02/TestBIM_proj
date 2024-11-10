using UnityEngine;
using UnityEngine.UI; // If using UI for display or buttons

public class CameraSwitcher : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    private WebCamDevice[] devices;
    private int currentCameraIndex = 0;

    public RawImage display; // Assign this to a RawImage to show the camera feed

    void Start()
    {
        // Get available devices
        devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            // Start with the first camera
            StartCamera(currentCameraIndex);
        }
        else
        {
            Debug.LogError("No cameras found!");
        }
    }

    public void SwitchCamera()
    {
        if (devices.Length > 1)
        {
            // Stop the current camera
            if (webCamTexture != null)
            {
                webCamTexture.Stop();
            }

            // Increment the index and wrap around
            currentCameraIndex = (currentCameraIndex + 1) % devices.Length;

            // Start the next camera
            StartCamera(currentCameraIndex);
        }
        else
        {
            Debug.LogWarning("No other cameras to switch to.");
        }
    }

    private void StartCamera(int index)
    {
        // Initialize the WebCamTexture with the new camera
        webCamTexture = new WebCamTexture(devices[index].name);
        display.texture = webCamTexture; // Display it on a RawImage
        webCamTexture.Play();
    }

    private void OnDestroy()
    {
        // Stop the camera when the object is destroyed
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
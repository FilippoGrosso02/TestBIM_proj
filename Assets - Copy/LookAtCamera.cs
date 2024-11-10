using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera targetCamera;
    public float initialDistance = 50.0f; // Set this to the initial Y distance from the camera when the UI element was created
    public float scalingFactor = 0.5f;    // Set this to control the extent of scaling, 1 means fully constant size, 0 means no scaling adjustment

    void Start()
    {
        // Find the camera tagged as MainCamera
        targetCamera = Camera.main;

        // Optionally, you can find a camera by its name if MainCamera tag is not set
        if (targetCamera == null)
        {
            targetCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        }

        if (targetCamera == null)
        {
            Debug.LogError("No camera found. Please ensure there is a camera in the scene tagged as MainCamera.");
        }
    }

    void Update()
    {
        if (targetCamera != null)
        {
            // Make the canvas face the camera
            transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                             targetCamera.transform.rotation * Vector3.up);

            // Calculate the vertical (Y) distance from the camera to the UI element
            float currentYDistance = Mathf.Abs(transform.position.y - targetCamera.transform.position.y);

            // Adjust the local scale based on the Y distance to keep the size relatively constant on screen
            float scaleFactor = Mathf.Lerp(1.0f, currentYDistance / initialDistance, scalingFactor);
            transform.localScale = Vector3.one * scaleFactor;
        }
    }
}

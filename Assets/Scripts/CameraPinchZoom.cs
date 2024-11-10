using UnityEngine;

public class CameraPinchZoomAndDrag : MonoBehaviour
{
    public Camera mainCamera;          // Assign the Main Camera here or use Camera.main in Start
    public Camera overlayCamera;       // Assign the overlay camera here
    public float zoomSpeed = 0.1f;     // Speed at which the camera zooms
    public float dragSpeed = 0.001f;   // Base speed at which the camera drags
    public float minSize = 2f;         // Minimum zoom level
    public float maxSize = 10f;        // Maximum zoom level

    private Vector2 lastTouchPosition; // Store the last position of the touch or mouse for dragging

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (overlayCamera == null)
        {
            Debug.LogWarning("Overlay Camera is not assigned.");
        }
    }

    void Update()
    {
        if (Application.isMobilePlatform)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            // Single finger drag
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.position - lastTouchPosition;
                float adjustedDragSpeed = dragSpeed * mainCamera.orthographicSize;

                Vector3 move = new Vector3(-delta.x * adjustedDragSpeed, 0, -delta.y * adjustedDragSpeed);
                move = Quaternion.Euler(0, mainCamera.transform.eulerAngles.z, 0) * move;

                mainCamera.transform.Translate(move, Space.World);
                lastTouchPosition = touch.position;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Pan
            if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
            {
                Vector2 averageTouchDelta = (touch0.deltaPosition + touch1.deltaPosition) / 2;
                float adjustedDragSpeed = dragSpeed * mainCamera.orthographicSize;

                Vector3 move = new Vector3(-averageTouchDelta.x * adjustedDragSpeed, 0, -averageTouchDelta.y * adjustedDragSpeed);
                move = Quaternion.Euler(0, mainCamera.transform.eulerAngles.z, 0) * move;

                mainCamera.transform.Translate(move, Space.World);
            }

            // Pinch-to-zoom
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            mainCamera.orthographicSize += deltaMagnitudeDiff * zoomSpeed;
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minSize, maxSize);

            if (overlayCamera != null)
            {
                overlayCamera.orthographicSize = mainCamera.orthographicSize;
            }
        }
    }

    private void HandleMouseInput()
    {
        // Mouse zoom (scroll wheel)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        mainCamera.orthographicSize -= scroll * zoomSpeed * 10; // Scale zoom for mouse wheel
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minSize, maxSize);

        if (overlayCamera != null)
        {
            overlayCamera.orthographicSize = mainCamera.orthographicSize;
        }

        // Mouse drag (left mouse button)
        if (Input.GetMouseButtonDown(0))
        {
            lastTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastTouchPosition;
            float adjustedDragSpeed = dragSpeed * mainCamera.orthographicSize;

            Vector3 move = new Vector3(-delta.x * adjustedDragSpeed, 0, -delta.y * adjustedDragSpeed);
            move = Quaternion.Euler(0, mainCamera.transform.eulerAngles.z, 0) * move;

            mainCamera.transform.Translate(move, Space.World);
            lastTouchPosition = Input.mousePosition;
        }
    }
}

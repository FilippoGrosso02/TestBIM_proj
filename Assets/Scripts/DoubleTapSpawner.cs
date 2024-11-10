using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DoubleTapSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;    // Assign the prefab or object to spawn in the inspector
    public Camera mainCamera;           // Assign the main camera here or use Camera.main in Start
    public float doubleTapTime = 0.3f;  // Maximum time interval between taps to register a double tap
    public float planeYPosition = 0f;   // Y-position of the plane where objects should be spawned
    public UnityEvent onDoubleTap;      // Event triggered on double-tap

    private float lastTapTime;          // Time of the last tap

    private Vector2 lastTapPosition;    // Position of the last tap

    public TagManager tagManager;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseDoubleClick();  // Simulate double tap with mouse clicks in the editor or on PC
        #else
        HandleTouchDoubleTap();    // Use touch for mobile devices
        #endif
    }

    void HandleTouchDoubleTap()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                if (Time.time - lastTapTime < doubleTapTime && Vector2.Distance(touch.position, lastTapPosition) < 20f)
                {
                    onDoubleTap.Invoke();  // Trigger the event
                    SpawnObjectAtTouchPosition(touch.position);
                }

                lastTapTime = Time.time;
                lastTapPosition = touch.position;
            }
        }
    }

    void HandleMouseDoubleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;

            if (Time.time - lastTapTime < doubleTapTime && Vector2.Distance(mousePosition, lastTapPosition) < 20f)
            {
                onDoubleTap.Invoke();  // Trigger the event
                SpawnObjectAtTouchPosition(mousePosition);
            }

            lastTapTime = Time.time;
            lastTapPosition = mousePosition;
        }
    }

    void SpawnObjectAtTouchPosition(Vector2 screenPosition)
    {
        // Define the plane at the specified Y position (e.g., y = 0)
        Plane plane = new Plane(Vector3.up, new Vector3(0, planeYPosition, 0));

        // Create a ray from the camera through the touch or mouse position
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        // If the ray intersects with the plane, position the object there
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            GameObject obj = Instantiate(objectToSpawn, hitPoint, Quaternion.identity);
            tagManager.SetLastPlacedPrefab(obj);

        }
    }
}

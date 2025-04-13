using UnityEngine;

public class SimpleZoomCamera : MonoBehaviour
{
    public float zoomSpeed = 0.5f;
    public float minZoom = 5f;
    public float maxZoom = 30f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // Mobile touch input
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Get previous touch positions
            Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0Prev - touch1Prev).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = prevMagnitude - currentMagnitude;

            Zoom(difference * zoomSpeed * Time.deltaTime);
        }
#if UNITY_EDITOR
        // Scroll wheel for testing in Editor
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Zoom(scroll * -100f * zoomSpeed * Time.deltaTime);
#endif
    }

    void Zoom(float increment)
    {
        float newSize = Mathf.Clamp(cam.orthographic ? cam.orthographicSize + increment : cam.fieldOfView + increment, minZoom, maxZoom);
        
        if (cam.orthographic)
            cam.orthographicSize = newSize;
        else
            cam.fieldOfView = newSize;
    }
}

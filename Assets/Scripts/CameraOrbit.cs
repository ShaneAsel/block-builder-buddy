using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform cameraTransform;
    public float rotationSpeed = 0.2f;
    public float zoomSpeed = 0.1f;
    public float minZoom = 5f;
    public float maxZoom = 25f;
    public float minPitch = 10f;
    public float maxPitch = 80f;

    private float yaw = 0f;
    private float pitch = 30f;
    private float currentZoom = 10f;

    void Start()
    {
        currentZoom = cameraTransform.localPosition.magnitude;
        UpdateCameraPosition();
    }

    void Update()
    {
        HandleTouch();
#if UNITY_EDITOR
        HandleEditorMouse();
#endif
        UpdateCameraPosition();
    }

    void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                yaw += delta.x * rotationSpeed;
                pitch -= delta.y * rotationSpeed;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }
        }

        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            float prevDist = (t0.position - t0.deltaPosition - (t1.position - t1.deltaPosition)).magnitude;
            float currDist = (t0.position - t1.position).magnitude;
            float delta = currDist - prevDist;

            currentZoom -= delta * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
    }

    void HandleEditorMouse()
    {
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed * 10f;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * 10f;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentZoom -= scroll * zoomSpeed * 100f;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -currentZoom);
        cameraTransform.localPosition = offset;
        cameraTransform.LookAt(transform.position);
    }
}

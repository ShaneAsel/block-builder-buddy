using UnityEngine;
using UnityEngine.EventSystems;

public class CameraOrbit : MonoBehaviour
{
    public Transform cameraTransform;
    public float rotationSpeed = 0.2f;
    public float zoomSpeed = 0.1f;
    public float minZoom = 5f;
    public float maxZoom = 25f;
    public float minPitch = 10f;
    public float maxPitch = 80f;

    private float targetYaw = 0f;
    private float targetPitch = 30f;
    private float currentYaw = 0f;
    private float currentPitch = 30f;
    private float yawVelocity = 0f;
    private float pitchVelocity = 0f;
    private float currentZoom = 10f;

    public float smoothTime = 0.1f;

    void Start()
    {
        currentZoom = cameraTransform.localPosition.magnitude;
        targetYaw = currentYaw = transform.eulerAngles.y;
        targetPitch = currentPitch = 30f;
        UpdateCameraPosition();
    }

    void Update()
    {
        HandleTouch();
        #if UNITY_EDITOR
        HandleEditorMouse();
        #endif
        SmoothRotate();
        UpdateCameraPosition();
    }

    void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                targetYaw += delta.x * rotationSpeed;
                targetPitch -= delta.y * rotationSpeed;
                targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
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
            targetYaw += Input.GetAxis("Mouse X") * rotationSpeed * 10f;
            targetPitch -= Input.GetAxis("Mouse Y") * rotationSpeed * 10f;
            targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentZoom -= scroll * zoomSpeed * 100f;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
    }

    void SmoothRotate()
    {
        currentYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref yawVelocity, smoothTime);
        currentPitch = Mathf.SmoothDampAngle(currentPitch, targetPitch, ref pitchVelocity, smoothTime);
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -currentZoom);
        cameraTransform.localPosition = offset;
        cameraTransform.LookAt(transform.position);
    }
}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed; 
    public float minX;
    public float maxX;
    private float yaw;   //Horizontal
    private float pitch; //Vertical 

    public Camera cam;
    public float minZoom = 10f;
    public float maxZoom = 60f;
    public float zoomSpeed = 10f;
    private float targetZoom;

    private void Start()
    {
        targetZoom = cam.fieldOfView;
        //cam = Camera.main;

        cam = GetComponent<Camera>();
        cam = FindAnyObjectByType<Camera>();


    }

    void LateUpdate()
    {
        Move();
        Zoom();
    }

    void Move()
    {
        // Get mouse input for rotation
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minX, maxX); // Limit vertical rotation angle

        // Calculate new camera rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        transform.rotation = rotation;
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scroll * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
    }
  }

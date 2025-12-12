using UnityEngine;
using UnityEngine.UIElements;

public class BasicCameraController : MonoBehaviour
{
    Camera cam;
    float speed = 5;
    float sensitivity = 100;
    [SerializeField] Transform xRotation;
    Vector3 startPos;
    Quaternion startRot;
    Quaternion startRotxRot;
    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        startPos = transform.position;
        startRot = transform.rotation;
        startRotxRot = xRotation.localRotation;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && cam != null)
        {
            cam.orthographic = !cam.orthographic;
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (graphMode)
            {
                transform.position += new Vector3(0, 1, 0) * Time.deltaTime * speed;
            }
            else
            {
                transform.Translate(xRotation.forward * speed * Time.deltaTime, Space.World);
                if (cam != null && cam.orthographic)
                {
                    cam.orthographicSize -= speed * Time.deltaTime;
                }
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (graphMode)
            {
                transform.position += new Vector3(0, -1, 0) * Time.deltaTime * speed;
            }
            else
            {
                transform.Translate(-1 * xRotation.forward * speed * Time.deltaTime, Space.World);
                if (cam != null && cam.orthographic)
                {
                    cam.orthographicSize += speed * Time.deltaTime;
                }
            }

        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(xRotation.right * speed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-1 * xRotation.right * speed * Time.deltaTime, Space.World);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= 5;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed /= 5;
        }

        if (Input.GetMouseButton(1))
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity, 0));
            xRotation.Rotate(-Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity, 0, 0);
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }
    private bool graphMode = false;
    public void SetCamForGraph()
    {
        graphMode = !graphMode;
        if (graphMode)
        {
            transform.position = new Vector3(7, 10, 17);
            transform.rotation = Quaternion.Euler(0, -90, 0);
            xRotation.localRotation = Quaternion.Euler(0, 0, 0);
            if (cam == null)
                return;

            cam.orthographicSize = 13;
            cam.orthographic = true;
        }
        else
        {
            transform.position = startPos;
            transform.rotation = startRot;
            xRotation.localRotation = startRotxRot;
            if (cam == null) return;
            cam.orthographic = false;
        }

    }
}

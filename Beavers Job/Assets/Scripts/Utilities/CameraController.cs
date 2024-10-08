using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float zoomSpeed = 2.0f;
    public float minOrthographicSize = 1.0f;
    public float maxOrthographicSize = 20.0f;

    public float minX = -10.0f;
    public float maxX = 10.0f;
    public float minY = -10.0f;
    public float maxY = 10.0f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0);

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize -= scroll * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minOrthographicSize, maxOrthographicSize);

        if (Input.GetKey(KeyCode.E))
        {
            Camera.main.orthographicSize -= zoomSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            Camera.main.orthographicSize += zoomSpeed * Time.deltaTime;
        }

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minOrthographicSize, maxOrthographicSize);
    }
}

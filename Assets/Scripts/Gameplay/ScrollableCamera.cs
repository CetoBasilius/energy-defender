using UnityEngine;

public class ScrollableCamera : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float dragSpeed = 2f;
    private float minX = -20f;
    private float maxX = 20f;
    private float minY = 0f;
    private float maxY = 0f;

    private Vector3 dragOrigin;

    void Update()
    {
        if (Input.touchSupported)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                MoveCamera(-delta.x, -delta.y, scrollSpeed);
            }
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 dragDelta = Input.mousePosition - dragOrigin;
            MoveCamera(-dragDelta.x, -dragDelta.y, dragSpeed);
            dragOrigin = Input.mousePosition;
        }
    }

    void MoveCamera(float deltaX, float deltaY, float speed)
    {
        Vector3 movement = new Vector3(deltaX, deltaY, 0f) * speed * Time.deltaTime;
        transform.Translate(movement);
        ClampCameraPosition();
    }

    void ClampCameraPosition()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        transform.position = clampedPosition;
    }
}

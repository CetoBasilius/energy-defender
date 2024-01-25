using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScrollableCamera : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float dragSpeed = 5f;
    private Vector3 dragOrigin;
    private bool enableScroll = true;
    public Tilemap tilemap;

    private void Update()
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

    private void HandleTouchInput()
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

    private void HandleMouseInput()
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

    private void MoveCamera(float deltaX, float deltaY, float speed)
    {
        if (!enableScroll)
        {
            return;
        }

        Vector3 movement = new Vector3(deltaX, deltaY, 0f) * speed * Time.deltaTime;
        transform.Translate(movement);
        ClampCameraPosition();
    }

    private void ClampCameraPosition()
    {
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float halfCamCellsX = camWidth / 2;
        float minCellsX = tilemap.cellBounds.min.x;
        float maxCellsX = tilemap.cellBounds.max.x;

        float minX = minCellsX + halfCamCellsX;
        float maxX = maxCellsX - halfCamCellsX;

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, 0f, 0f);
        transform.position = clampedPosition;
    }

    public void SetEnabled(bool enabled)
    {
        this.enableScroll = enabled;
    }
}

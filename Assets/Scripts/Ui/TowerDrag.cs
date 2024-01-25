using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TowerDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject towerPrefab;
    public GameObject battlefield;
    public Tilemap tilemap;
    public Color placeableColor = Color.green;
    public Color nonPlaceableColor = Color.red;
    public ScrollableCamera scrollableCamera;
    private GameObject draggedTower;
    private Vector3Int lastCell;

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedTower = Instantiate(towerPrefab);
        draggedTower.transform.SetParent(battlefield.transform, false);
        scrollableCamera.SetEnabled(false); // TODO: This should be handled by a UI manager, not the scrollable camera to prevent coupling, but life is short
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedTower != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = draggedTower.transform.position.z;
            draggedTower.transform.position = mousePosition;

            UpdateTileColor();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        TileBase tilebase = GetGridTileUnderMouse();
        if (tilebase != null)
        {
            bool isPlaceable = tilebase.name == "grass";
            if (!isPlaceable)
            {
                Destroy(draggedTower);
            }
            else
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
                Vector3 snapPosition = tilemap.GetCellCenterWorld(cellPosition);
                snapPosition.z = draggedTower.transform.position.z;
                draggedTower.transform.position = snapPosition;

                Debug.Log("Dropped tower on " + tilebase.name + " at " + cellPosition);

                // TODO: set tile to tower so no other tower can be placed there or improve placement logic
            }
        }
        else
        {
            Destroy(draggedTower);
        }

        ClearLastCell();
        scrollableCamera.SetEnabled(true); // TODO: This should be handled by a UI manager, not the scrollable camera to prevent coupling, but life is short
    }

    private TileBase GetGridTileUnderMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
        TileBase tilebase = tilemap.GetTile(cellPosition);

        return tilebase;
    }

    private void ClearLastCell()
    {
        tilemap.SetTileFlags(lastCell, TileFlags.None);
        tilemap.SetColor(lastCell, Color.white);
    }

    private void UpdateTileColor()
    {
        ClearLastCell();
        TileBase tilebase = GetGridTileUnderMouse();
        if (tilebase != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);

            bool isPlaceable = tilebase.name == "grass";

            tilemap.SetTileFlags(cellPosition, TileFlags.None);
            tilemap.SetColor(cellPosition, isPlaceable ? placeableColor : nonPlaceableColor);
            lastCell = cellPosition;
        }
    }
}

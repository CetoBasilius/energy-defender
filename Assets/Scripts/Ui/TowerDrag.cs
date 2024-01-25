using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TowerDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject testTowerPrefab;
    public GameObject towers;
    public Tilemap tilemap;
    public Color placeableColor = Color.green;
    public Color nonPlaceableColor = Color.red;
    private GameObject draggedTower;
    private Vector3Int lastCell;

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedTower = Instantiate(testTowerPrefab);
        draggedTower.transform.SetParent(towers.transform, false);
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

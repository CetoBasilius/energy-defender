using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TowerDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject testTowerPrefab;
    public GameObject towers;
    public Tilemap tilemap;
    private GameObject draggedTower;

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedTower = Instantiate(testTowerPrefab);
        draggedTower.transform.SetParent(towers.transform, false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the dragged tower with the mouse
        if (draggedTower != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = draggedTower.transform.position.z;
            draggedTower.transform.position = mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        TileBase tile = GetGridTileUnderMouse();
        if (tile != null)
        {
            Debug.Log("Dropped tower on " + tile.name + " at " + tilemap.WorldToCell(draggedTower.transform.position));
        }
        else
        {
            Destroy(draggedTower);
        }
    }

    private TileBase GetGridTileUnderMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = tilemap.WorldToCell(mouseWorldPos);
        TileBase tile = tilemap.GetTile(coordinate);

        return tile;
    }
}

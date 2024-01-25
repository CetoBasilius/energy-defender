using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Data;

public class TowerDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string dataType = "cannon";
    public GameManager gameManager;
    // public GridManager gridManager;
    public GameObject towerPrefab;
    public GameObject battlefield;
    public Tilemap tilemap;
    public TextMeshProUGUI energyCostText;
    public Color placeableColor = Color.green;
    public Color nonPlaceableColor = Color.red;
    public ScrollableCamera scrollableCamera;

    private GameObject draggedTower;
    private Vector3Int lastCell;
    private TowerData towerData;

    void Start()
    {
        towerData = gameManager.GetTowerData(dataType);
        int energyCost = towerData != null ? towerData.energyCost : 0;
        energyCostText.text = energyCost.ToString();
    }

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
                // TODO: this should be handled by a tower manager or game manager, not the TowerDrag script

                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
                Vector3 snapPosition = tilemap.GetCellCenterWorld(cellPosition);
                snapPosition.z = draggedTower.transform.position.z;
                draggedTower.transform.position = snapPosition;

                // TODO: Set tile to tower so no other tower can be placed there or improve placement logic
                // tilemap.SetTile(cellPosition, tilebase);

                // TODO: Register tower with gridManager

                draggedTower = null;
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

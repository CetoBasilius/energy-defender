using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Data;

public class TowerDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string id = "cannon";
    public GameManager gameManager;
    public GridManager gridManager;
    public TextMeshProUGUI energyCostText;
    public Color placeableColor = Color.green;
    public Color nonPlaceableColor = Color.red;
    // Light orange
    public Color expensiveColor = new Color(1, 0.7f, 0.2f);
    public UIManager uiManager;
    private GameObject draggedTower;
    private Vector3Int lastCell;
    private int energyCost;
    private bool isExpensive = false;

    void Start()
    {
        energyCost = gridManager.GetTowerEnergyCost(id);
        energyCostText.text = energyCost.ToString();
    }

    void Update()
    {
        bool wasExpensive = isExpensive;
        if (energyCost > gameManager.GetCurrentEnergy())
        {
            isExpensive = true;
            energyCostText.color = Color.red;
        }
        else
        {
            isExpensive = false;
            energyCostText.color = Color.white;
        }
        if (wasExpensive != isExpensive && lastCell != null && draggedTower != null)
        {
            UpdateTileColor();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedTower = gridManager.AddTower(id);
        uiManager.LockCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedTower != null)
        {
            // TODO: check eventData instead of Input.mousePosition
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = draggedTower.transform.position.z;
            draggedTower.transform.position = mousePosition;

            UpdateTileColor();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // TODO: check eventData instead of Input.mousePosition
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (gridManager.IsTileAvailable(mousePosition))
        {
            if (gameManager.SpendEnergy(energyCost))
            {
                gridManager.PlaceTower(mousePosition, draggedTower.GetComponent<Tower>());
            }
            else
            {
                Destroy(draggedTower);
            }
        }
        else
        {
            Destroy(draggedTower);
        }

        draggedTower = null;
        gridManager.ColorCell(lastCell, Color.white);
        uiManager.UnlockCamera();
    }

    private void UpdateTileColor()
    {
        gridManager.ColorCell(lastCell, Color.white);

        // TODO: check eventData instead of Input.mousePosition
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool isTileAvailable = gridManager.IsTileAvailable(mousePosition);
        Vector3Int cellPosition = gridManager.GetTileCellPosition(mousePosition);

        gridManager.ColorCell(cellPosition, isTileAvailable ? (isExpensive ? expensiveColor : placeableColor) : nonPlaceableColor);
        lastCell = cellPosition;
    }
}

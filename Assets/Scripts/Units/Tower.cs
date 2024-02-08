using System.Collections;
using System.Collections.Generic;
using Data;
using Gameplay;
using UnityEngine;

public class Tower : GameUnit
{
    private TowerData data;
    public GameObject turret;
    public GameObject gun;
    public float rotationSpeed = 0f;
    public bool isDefense = false;

    private GridCell targetCell;
    private List<GridCell> cellsInRange = new List<GridCell>();
    private bool isActive = false;
    private GridCell currentCell;

    void Start()
    {

    }

    void Update()
    {
        // TODO: extract to own methods
        if (isDefense)
        {
            turret.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        if (!isActive)
        {
            return;
        }
        if (targetCell != null && gun != null)
        {
            Vector3 direction = targetCell.GetPosition() - turret.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void SetTarget(GridCell cell)
    {
        this.targetCell = cell;
    }

    public void Activate(List<GridCell> cellsInRange)
    {
        this.isActive = true;
        this.cellsInRange = cellsInRange;
    }

    public void Setup(TowerData towerData)
    {
        this.data = towerData;
    }

    public void SetCurrentCell(GridCell cell)
    {
        this.currentCell = cell;
    }

    public int GetRange()
    {
        return isDefense ? data.specialRange : data.attackRange;
    }
}

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
    private List<GridCell> targetCells = new List<GridCell>();
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

        if (targetCell == null)
        {
            if (targetCells.Count > 0)
            {
                targetCell = targetCells[0];
            }
        }

        if (targetCell != null && gun != null)
        {
            Enemy targetEnemy = targetCell.enemies[0];

            Vector3 turretDirection = targetEnemy.transform.position - turret.transform.position;
            float desiredTurretAngle = Mathf.Atan2(turretDirection.y, turretDirection.x) * Mathf.Rad2Deg - 90;
            float currentTurretAngle = turret.transform.rotation.eulerAngles.z;
            float turretAngleDifference = Mathf.DeltaAngle(currentTurretAngle, desiredTurretAngle);
            float turretRotationAmount = Mathf.Sign(turretAngleDifference) * Mathf.Min(Mathf.Abs(turretAngleDifference), 0.3f);
            turret.transform.Rotate(0, 0, turretRotationAmount);
        }
    }

    public bool AddTarget(GridCell cell)
    {
        if (targetCells.Contains(cell))
        {
            return false;
        }

        targetCells.Add(cell);
        return true;
    }

    public bool RemoveTarget(GridCell cell)
    {
        if (cell == targetCell)
        {
            targetCell = null;
        }

        if (targetCells.Contains(cell))
        {
            targetCells.Remove(cell);
            return true;
        }
        return false;
    }

    public void Activate()
    {
        this.isActive = true;
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

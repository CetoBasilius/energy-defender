using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Gameplay;
using UnityEngine;

public class Enemy : GameUnit
{
    GridCell currentPathCell;
    GridCell nextPathCell;

    GridCell targetCell;

    public GameObject turret;
    public GameObject body;
    public Tread leftTread;
    public Tread rightTread;

    private float speed = 0.5f;

    private Vector2 treadSpeed = new Vector2(0, 0);
    private EnemyData enemyData;

    public delegate List<GridCell> CellChangedHandler(Enemy self, GridCell oldCell, GridCell newCell);
    public event CellChangedHandler OnCellChanged;

    void Start()
    {
        leftTread.speed = treadSpeed;
        rightTread.speed = treadSpeed;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        leftTread.speed = treadSpeed;
        rightTread.speed = treadSpeed;

        if (nextPathCell == null)
        {
            treadSpeed.y = 0f;
            return;
        }
        treadSpeed.y = 16f; // TODO: relate this to speed (or enemyData)

        Vector3 direction = nextPathCell.GetPosition() - transform.position;
        transform.Translate(direction.normalized * Time.deltaTime * speed);

        // Rotate body smoothly from previous angle to face the next cell
        Vector3 bodyDirection = nextPathCell.GetPosition() - body.transform.position;
        float desiredAngle = Mathf.Atan2(bodyDirection.y, bodyDirection.x) * Mathf.Rad2Deg - 90;
        float currentAngle = body.transform.rotation.eulerAngles.z;
        float angleDifference = Mathf.DeltaAngle(currentAngle, desiredAngle);
        float rotationAmount = Mathf.Sign(angleDifference) * Mathf.Min(Mathf.Abs(angleDifference), 0.5f);
        body.transform.Rotate(0, 0, rotationAmount);

        // Do the same for the turret
        if (targetCell != null)
        {
            Vector3 turretDirection = targetCell.GetPosition() - turret.transform.position;
            float desiredTurretAngle = Mathf.Atan2(turretDirection.y, turretDirection.x) * Mathf.Rad2Deg - 90;
            float currentTurretAngle = turret.transform.rotation.eulerAngles.z;
            float turretAngleDifference = Mathf.DeltaAngle(currentTurretAngle, desiredTurretAngle);
            float turretRotationAmount = Mathf.Sign(turretAngleDifference) * Mathf.Min(Mathf.Abs(turretAngleDifference), 0.3f);
            turret.transform.Rotate(0, 0, turretRotationAmount);
        }

        if (Vector3.Distance(transform.position, nextPathCell.GetPosition()) < 0.1f)
        {
            SetCurrentCell(nextPathCell);
        }
    }

    public void Setup(EnemyData enemyData)
    {
        this.enemyData = enemyData;
        this.speed = enemyData.speed / 100;
    }

    public void SetCurrentCell(GridCell cell)
    {
        List<GridCell> targetCells = OnCellChanged?.Invoke(this, cell, this.currentPathCell);

        if (targetCells != null && targetCells.Count > 0)
        {
            if (!targetCells.Contains(targetCell))
            {
                targetCell = targetCells[0];
            }
        } else {
            targetCell = cell.nextPathCell;
        }

        this.currentPathCell = cell;
        this.nextPathCell = cell.nextPathCell;
    }

    internal int GetRange()
    {
        return enemyData.attackRange;
    }
}

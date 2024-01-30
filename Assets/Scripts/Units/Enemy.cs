using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class Enemy : GameUnit
{
    GridCell currentCell;
    GridCell nextCell;

    GridCell targetCell;

    public GameObject turret;
    public GameObject body;
    public Tread leftTread;
    public Tread rightTread;

    private float speed = 0.5f;

    private Vector2 treadSpeed = new Vector2(0, 0);
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

        if (nextCell == null)
        {
            treadSpeed.y = 0f;
            return;
        }
        treadSpeed.y = 16f;

        Vector3 direction = nextCell.GetPosition() - transform.position;
        transform.Translate(direction.normalized * Time.deltaTime * speed);

        // Rotate body smoothly from previous angle to face the next cell
        Vector3 bodyDirection = nextCell.GetPosition() - body.transform.position;
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
        
        if (Vector3.Distance(transform.position, nextCell.GetPosition()) < 0.1f)
        {
            SetCurrentCell(nextCell);
            targetCell = nextCell.nextPathCell;
        }
    }

    public void SetCurrentCell(GridCell cell)
    {
        this.currentCell = cell;
        this.nextCell = cell.nextPathCell;
    }
}

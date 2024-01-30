using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class Enemy : GameUnit
{
    GridCell currentCell;
    GridCell nextCell;

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
        // Rotate turret to face the next cell
        Vector3 turretDirection = nextCell.GetPosition() - turret.transform.position;
        float turretAngle = Mathf.Atan2(turretDirection.y, turretDirection.x) * Mathf.Rad2Deg;
        turret.transform.rotation = Quaternion.AngleAxis(turretAngle - 90, Vector3.forward);

        if (Vector3.Distance(transform.position, nextCell.GetPosition()) < 0.1f)
        {
            SetCurrentCell(nextCell);
        }
    }

    public void SetCurrentCell(GridCell cell)
    {
        this.currentCell = cell;
        this.nextCell = cell.nextPathCell;
    }
}

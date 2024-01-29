using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : GameUnit
{
    public GameObject turret;
    public GameObject gun;
    public float rotationSpeed = 0f;
    public bool isDefense = false;

    private Enemy enemyTarget;


    void Start()
    {

    }

    void Update()
    {
        if (isDefense)
        {
            turret.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        else if (enemyTarget != null && gun != null)
        {
            Vector3 direction = enemyTarget.transform.position - turret.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void SetTarget(Enemy enemy)
    {
        this.enemyTarget = enemy;
    }
}

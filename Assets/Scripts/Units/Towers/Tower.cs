using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject turret;
    public GameObject gun;
    public float rotationSpeed = 0f;
    public bool isDefense = false;
    public GameManager gameManager;
    void Start()
    {

    }

    void Update()
    {
        if (isDefense)
        {
            turret.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        else if (gameManager != null)
        {
            // TODO: scan for enemies using gameManager or enemyManager
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap backgroundTilemap;
    public Tilemap towerTilemap;
    void Start()
    {
        LevelManager.Setup(backgroundTilemap, towerTilemap);
        LevelManager.SetLevel("World01/01"); // TODO: This will be able to be set from the main menu
    }

    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{    
    
    private void Awake()
    {
        var nc = NetworkController.Instance;
        nc.spawnManager.SpawnPlayer();
    }
}
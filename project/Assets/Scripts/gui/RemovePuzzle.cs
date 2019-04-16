using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RemovePuzzle : MonoBehaviour
{

   // bool gameStarted = false;
    // Start is called before the first frame update
    void Awake()
    {
        //playerController.Instance.enabled = false;
        //Spawner.Instance.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.touchCount > 0)   //!gameStarted &&
        {
            Debug.Log("Start the game");
            Debug.Log("nr of touchcounts: " + Input.touchCount);
            //gameStarted = true;

            //unspawn
            NetworkServer.UnSpawn(gameObject);
            //ta bort puzzle
            Destroy(gameObject);

            

        }

    }
}

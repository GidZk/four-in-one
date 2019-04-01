using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //List<GameObject> activeNPC = new List<GameObject>;

    public GameObject enemy;

    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    void SpawnEnemy()
    {
        Vector3 clickPosition =
       Camera.main.ScreenToWorldPoint(Input.mousePosition);

        clickPosition.z = 0;

        // Now we can actually spawn a bob object
        var go = Instantiate(enemy, new Vector3(30,Random.RandomRange(-7.0f,13.0f),0), Quaternion.identity);


        //Vector2 spawnPosition = new Vector2();
        //Instantiate(enemy, spawnPosition);

    }
    // Update is called once per frame
    void Update()
    {
     

            if (Random.value < 0.01)
                SpawnEnemy();

    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //List<GameObject> activeNPC = new List<GameObject>;

    public GameObject enemy;
    public GameObject stone;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void SpawnEnemy()
    {
        //Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //clickPosition.z = 0;

        // Now we can actually spawn a bob object
        Debug.Log("Spawning shark");
        var go = Instantiate(enemy, new Vector3(40, Random.Range(-13f,4f),0), Quaternion.identity);

  

    }

    void SpawnStone()
    {

        //spawning stones
        var stillgo = Instantiate(stone, new Vector3(40, Random.Range(-13f, 4f), 0), Quaternion.identity);

    }
    // Update is called once per frame
    void Update()
    {
    
            if (Random.value < 0.006)
                SpawnEnemy();
            if (Random.value < 0.006)
                SpawnStone();

    }
    
}

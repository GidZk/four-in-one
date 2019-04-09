using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //List<GameObject> activeNPC = new List<GameObject>;

    public GameObject shark;
    public GameObject stone;
    public GameObject crab;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void SpawnShark()
    {
        //Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //clickPosition.z = 0;

        // Now we can actually spawn a shark object
        Debug.Log("Spawning shark");
        var go = Instantiate(shark, new Vector3(40, Random.Range(-13f,4f),0), Quaternion.identity);
        shark.transform.localScale = new Vector3(Random.Range(0.3f, 0.8f), Random.Range(0.3f, 0.8f), 0.95f);


    }

    void SpawnStone()
    {

        //spawning stones
        var stillgo = Instantiate(stone, new Vector3(40, Random.Range(-13f, 4f), 0), Quaternion.identity);
        stone.transform.localScale = new Vector3(Random.Range(0.2f, 0.6f), Random.Range(0.2f, 0.8f), 0.9f);

    }
    void SpawnCrab()
    {

        //spawning crab
        var stillgo = Instantiate(crab, new Vector3(40, Random.Range(-13f, 4f), 0), Quaternion.identity);
        crab.transform.localScale = new Vector3(Random.Range(0.2f, 0.6f), Random.Range(0.2f, 0.8f), 0.9f);

    }


    // Update is called once per frame
    void Update()
    {
    
            if (Random.value < 0.006)
                SpawnShark();
            if (Random.value < 0.006)
                SpawnStone();
            if (Random.value < 0.006)
                SpawnCrab();
    }
    
}

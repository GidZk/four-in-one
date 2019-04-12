using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //List<GameObject> activeNPC = new List<GameObject>;

    public GameObject shark;
    public GameObject stone;
    public GameObject crab;

    private bool spawnedUp;
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
        var go = Instantiate(shark, new Vector3(44, Random.Range(-29f,29f),0), Quaternion.identity);
        float scaling = Random.Range(2f, 3f);
        shark.transform.localScale = new Vector3(scaling, scaling, 0.9f);



    }

    void SpawnStone()
    {

        //spawning stones
        
        var stillgo = Instantiate(stone, new Vector3(44, Random.Range(-29f, 29f), 0), Quaternion.Euler(new Vector3(0, 0, (Random.Range(0f, 90f)))));
        float scaling = Random.Range(0.2f, 0.9f);
        stone.transform.localScale = new Vector3(scaling, scaling, 0.9f);

    }
    void SpawnCrab()
    {

        //spawning crab

      
        var rotation = Quaternion.Euler(new Vector3(0, 0, (180 * (Random.Range(0, 2)))));
        var stillgo = Instantiate(crab, new Vector3(44, Random.Range(-29f, 29f), 0), rotation);
        //        SpriteRenderer. = true
        //   var stillgo = Instantiate(crab, new Vector3(44, Random.Range(-29f, 29f), 0), Quaternion.Euler(new Vector3(0, 0, (Random.Range(0f, 90f)))));
        // crab.transform.Rotate(Vector3.forward * 100);
        float scaling = Random.Range(0.2f, 0.8f);
        crab.transform.localScale = new Vector3(scaling, scaling, 0.9f);
        // crab.transform.localRotation = Quaternion.Euler(0, 0, 90);
        //crab.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
        //crab.inp
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




  //  private class 
    
}

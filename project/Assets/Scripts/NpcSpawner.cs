
using System;
using System.Collections.Generic;
using  UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class NpcSpawner : MonoBehaviour
{
  public float secondsBetweenSpawn;
  public List<float> crabScale;
  public List<float> sharkScale;
  public List<float>  stoneScale;
  
  
  private List<GameObject> spawnable;
  private float elapsedTime;

  private void Start()
  {
   spawnable = gameObject.GetComponent<NetworkController>().spawnable;
   
  }

  public void SpawnNPC()
  {
    
    elapsedTime += Time.deltaTime;
 
    if (elapsedTime > secondsBetweenSpawn)
    {            
      elapsedTime = 0;

      // TODO fix probability distribution
      float p = Random.Range(0, 4);
      if (p <= 1)
      {
        SpawnCrab(crabScale[0], crabScale[1]);
        
      }
      else if (p > 1 && p <= 2)
      {
        SpawnStone(stoneScale[0],stoneScale[1]);
        
      }
      else
      {
        SpawnShark(sharkScale[0],sharkScale[1]);
        
      }
      
      
    }

    
  }


  private void SpawnHelper()
  {       
    

    
    
    
    
  }



  private void SpawnCrab(float minScaling,float MaxScaling){
      
  Debug.Log("NpcSpawner:: Spawning Crab");
  Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, (180 * (Random.Range(0, 2)))));
  GameObject go = Instantiate(spawnable[3], new Vector3(44, Random.Range(-29f, 29f), 0), rotation);
  float scaling = Random.Range(minScaling,MaxScaling);
  go.transform.localScale = new Vector3(scaling, scaling, 0.9f);
  NetworkServer.Spawn	(go);
         
}

// spawns a shark on the network server
private void SpawnShark(float minScaling, float MaxScaling)
{
  Debug.Log("NpcSpawner:: Spawning Shark");
  Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, (180 * (Random.Range(0, 2)))));
  var go = Instantiate(spawnable[2], new Vector3(44, Random.Range(-29f,29f),0), Quaternion.identity);
  float scaling = Random.Range(minScaling,MaxScaling);
  go.transform.localScale = new Vector3(scaling, scaling, 0.9f);
  NetworkServer.Spawn	(go);    
  
}



private void SpawnStone(float minScaling, float maxScaling)
{
  GameObject go = Instantiate(spawnable[4], new Vector3(44, Random.Range(-29f, 29f), 0), Quaternion.Euler(new Vector3(0, 0, (Random.Range(0f, 90f)))));
  float scaling = Random.Range(minScaling, maxScaling);
  go.transform.localScale = new Vector3(scaling, scaling, 0.9f);
  NetworkServer.Spawn(go);

}


public void SpawnStone()
{
          
          
}

}

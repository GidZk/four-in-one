using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public float secondsBetweenSpawn;
    public bool spawnDisabled;
    public List<GameObject> spawnable;

    
    //todo let the user set the scaling form unity
    //public List<float> crabScale;
    //public List<float> sharkScale;
    //public List<float> stoneScale;

    private float elapsedTime;
    private bool isSpawningLeft_Crabby;
    private bool isSpawningLeft_Sharky;
    
    

    public static Spawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        isSpawningLeft_Crabby = true;
        isSpawningLeft_Sharky = true;
        
    }


    //TODO refactor this horribe spawning method to the more general ... idea : make NPC class with their own spawn, 
    //TODO place in list of distribution eg : [crab,crabcrab,shark,shark,stone] and find one index with Random.Range(0,list.length)
    // TODO then for each index call NPC.spawn()
    public void SpawnNpc(int nOfNpcs)
    {
        if (spawnDisabled) return;
        elapsedTime += Time.deltaTime;

        if (elapsedTime > secondsBetweenSpawn)
        {
            elapsedTime = 0;

            // TODO fix probability distribution
            float p = Random.Range(0, nOfNpcs + 1);

            if (p <= 0.4f)
            {
                SpawnShark(1.5f, 2);
            }
            else if (p > 0.3f && p <= 1.7f)
            {
                SpawnStone(1, 2);
            }
            else
            {
                SpawnCrabPlast(0.4f, 0.7f);
            }
        }
    }


    public void SpawnPlayer()
    {
        var prefab = Resources.Load<GameObject>("Spawnable/player");
        if (prefab == null)
            throw new Exception("Could not get player prefab");
        GameObject go = Instantiate(prefab);
        if (go == null)
            throw new Exception("Did not spawn player");
        go.transform.position = Vector3.zero;
        NetworkServer.Spawn(go);
    }


    private void SpawnShark(float minScaling, float MaxScaling)
    {
        
        float scaling = Random.Range(minScaling, MaxScaling);
        GameObject go = LoadPrefab("spawnable/shark");

        go.GetComponent<SharkMovement>().Direction = SetDirection(isSpawningLeft_Sharky);
        go.transform.localScale = new Vector3(scaling, scaling, 0.9f);
        go.transform.rotation = SetRotation(isSpawningLeft_Sharky);
        go.transform.position = SetStartPosition(isSpawningLeft_Sharky);
        
        Debug.Log("before : " + isSpawningLeft_Sharky);
        isSpawningLeft_Sharky = !isSpawningLeft_Sharky;
        Debug.Log("after : " + isSpawningLeft_Sharky);
        NetworkServer.Spawn(go);
    }

    public void SpawnCrabPlast(float minScaling, float maxScaling)
    {
        float scaling = Random.Range(minScaling, maxScaling);
        GameObject go = LoadPrefab("spawnable/crabplast");
        go.GetComponent<CrabMovement>().Direction = SetDirection(isSpawningLeft_Crabby);
        //go.transform.localScale = new Vector3(scaling, scaling, 0.9f);
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (180 * (Random.Range(0, 2)))));
        go.transform.position = SetStartPosition(isSpawningLeft_Crabby);
        
        
        isSpawningLeft_Crabby = !isSpawningLeft_Crabby;

        NetworkServer.Spawn(go);
    }

    private void SpawnCrab(float minScaling, float maxScaling)
    {
        float scaling = Random.Range(minScaling, maxScaling);

        Debug.Log("Spawner:: Spawning Crab");
        GameObject go = LoadPrefab("spawnable/crab");
        go.GetComponent<CrabMovement>().Direction = SetDirection(isSpawningLeft_Crabby);
        go.transform.localScale = new Vector3(scaling, scaling, 0.9f);
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (180 * (Random.Range(0, 2)))));
        go.transform.position = SetStartPosition(isSpawningLeft_Crabby);
        
        isSpawningLeft_Crabby = !isSpawningLeft_Crabby;
        
        NetworkServer.Spawn(go);
    }

    private void SpawnStone(float minScaling, float maxScaling)
    {
        float scaling = Random.Range(minScaling, maxScaling);
        GameObject go = LoadPrefab("spawnable/stone");
        go.transform.localScale = new Vector3(scaling, scaling, 0.9f);
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (Random.Range(0f, 90f))));
        go.transform.position = new Vector3(34,  SetYPosition(), 0);

        NetworkServer.Spawn(go);
    }

    private Vector3 SetStartPosition(bool shouldSpawnLeft)
    {
        if (shouldSpawnLeft)    
            return new Vector2(-34, SetYPosition());
        
         return new Vector3(34, SetYPosition());

    }

    // TODO set params to camera params hardcoded for now
    private float SetYPosition()
    {
        return Random.Range(-17f, 17f);

    }


    private Vector2 SetDirection(bool shouldSpawnLeft)
    {
        return shouldSpawnLeft ? new Vector2(1, 1) : new Vector2(-1,1);
    }


   private Quaternion SetRotation(bool shouldSpawnLeft)
    {
        return shouldSpawnLeft ? Quaternion.Euler(0,0,180) : Quaternion.identity; 
    }



    private GameObject LoadPrefab(String path)
    {
        return Instantiate(Resources.Load(path)) as GameObject;
        //go.transform.position = Vector3.zero;
        //NetworkServer.Spawn(go);
    }
}
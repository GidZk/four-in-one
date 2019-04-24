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
    private Vector2 camera_y_extremes;
    private bool spawnCrabLeft;
    private bool spawnSharkLeft;
    
    

    public static Spawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        spawnCrabLeft = true;
        spawnSharkLeft = true;
        
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

            if (p <= 0.8)
            {
                SpawnCrabPlast(0.2f, 0.3f);
            }
            else if (p > 0.8 && p <= 2.5)
            {
                SpawnStone(1, 2);
            }
            else
            {
                SpawnShark(1.5f, 2);
            }
        }
    }


    public void SpawnPlayer()
    {
        var prefab = Resources.Load<GameObject>("Spawnable/player");
        if (prefab == null)
            throw new Exception("Could not get player prefab");
        GameObject go =
            Instantiate(prefab);
        if (go == null)
            throw new Exception("Did not spawn player");
        go.transform.position = Vector3.zero;
        NetworkServer.Spawn(go);
    }


    private void SpawnShark(float minScaling, float MaxScaling)
    {
        
        float scaling = Random.Range(minScaling, MaxScaling);

        GameObject go = LoadPrefab("spawnable/shark");

        go.GetComponent<SharkController>().Direction = SetDirection(spawnSharkLeft);
        go.transform.position = SetStartPosition(spawnSharkLeft);
        go.transform.rotation = SetRotation(spawnSharkLeft);
        go.transform.localScale = new Vector3(scaling, scaling, 0.9f);
        
        Debug.Log("before : " + spawnSharkLeft);
        spawnSharkLeft = !spawnSharkLeft;
        Debug.Log("after : " + spawnSharkLeft);
        NetworkServer.Spawn(go);
    }

    public void SpawnCrabPlast(float minScaling, float maxScaling)
    {
        
        
        float scaling = Random.Range(minScaling, maxScaling);
        GameObject go = LoadPrefab("spawnable/crabplast");
        go.transform.position = SetStartPosition(spawnCrabLeft);
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (180 * (Random.Range(0, 2)))));
        go.transform.localScale = new Vector3(scaling, scaling, 0.9f);
        
        
        
        spawnCrabLeft = !spawnCrabLeft;

        NetworkServer.Spawn(go);
    }

    private void SpawnCrab(float minScaling, float MaxScaling)
    {
        float scaling = Random.Range(minScaling, MaxScaling);

        Debug.Log("Spawner:: Spawning Crab");
        GameObject go = LoadPrefab("spawnable/crab");
        go.transform.position = SetStartPosition(spawnCrabLeft);
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (180 * (Random.Range(0, 2)))));
        go.transform.localScale = new Vector3(scaling, scaling, 0.9f);
        spawnCrabLeft = !spawnCrabLeft;

        NetworkServer.Spawn(go);
    }

    private void SpawnStone(float minScaling, float maxScaling)
    {
        float scaling = Random.Range(minScaling, maxScaling);
        GameObject go = LoadPrefab("spawnable/stone");
        go.transform.position = new Vector3(44,  SetYPosition(), 0);
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (Random.Range(0f, 90f))));
        go.transform.localScale = new Vector3(scaling, scaling, 0.9f);

        NetworkServer.Spawn(go);
    }

    private Vector3 SetStartPosition(bool shouldSpawnLeft)
    {
        if (shouldSpawnLeft)    
            return new Vector2(-44, SetYPosition());
        
         return new Vector3(44, SetYPosition());

    }

    // TODO set params to camera params hardcoded for now
    private float SetYPosition()
    {
        return Random.Range(-29f, 29f);

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
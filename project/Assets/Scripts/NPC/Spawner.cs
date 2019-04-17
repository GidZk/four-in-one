using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public float secondsBetweenSpawn;

    public bool spawnDisabled;

    public List<GameObject> spawnable;

    // public List<float> crabScale;

    //public List<float> sharkScale;
    //public List<float> stoneScale;

    private float elapsedTime;

    public static Spawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
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
        go.transform.position = new Vector3(44, Random.Range(-29f, 29f), 0);
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = new Vector3(scaling, scaling, 0.9f);
        NetworkServer.Spawn(go);
    }

    public void SpawnCrabPlast(float minScaling, float maxScaling)
    {
        float scaling = Random.Range(minScaling, maxScaling);
        GameObject go = LoadPrefab("spawnable/crabplast");
        go.transform.position = new Vector3(44, Random.Range(-29f, 29f), 0);
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (180 * (Random.Range(0, 2)))));
        go.transform.localScale = new Vector3(scaling, scaling, 0.9f);

        NetworkServer.Spawn(go);
    }

    private void SpawnCrab(float minScaling, float MaxScaling)
    {
        float scaling = Random.Range(minScaling, MaxScaling);

        Debug.Log("Spawner:: Spawning Crab");
        GameObject go = LoadPrefab("spawnable/crab");
        go.transform.position = new Vector3(44, Random.Range(-29f, 29f), 0);
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (180 * (Random.Range(0, 2)))));
        go.transform.localScale = new Vector3(scaling, scaling, 0.9f);

        NetworkServer.Spawn(go);
    }

    private void SpawnStone(float minScaling, float maxScaling)
    {
        float scaling = Random.Range(minScaling, maxScaling);
        GameObject go = LoadPrefab("spawnable/stone");
        go.transform.position = new Vector3(44, Random.Range(-29f, 29f), 0);
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (Random.Range(0f, 90f))));
        go.transform.localScale = new Vector3(scaling, scaling, 0.9f);

        NetworkServer.Spawn(go);
    }


    private GameObject LoadPrefab(String path)
    {
        return Instantiate(Resources.Load(path)) as GameObject;
        //go.transform.position = Vector3.zero;
        //NetworkServer.Spawn(go);
    }
}
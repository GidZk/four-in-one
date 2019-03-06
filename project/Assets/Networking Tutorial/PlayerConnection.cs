
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnection : NetworkBehaviour
// here goes the data for each attached object
{
    
    public GameObject PlayerUnitPrefab;
    GameObject blobberUnit;
    // Start is called before the first frame update
    void Start() {

        if (isLocalPlayer == false)
        {return;}

        Debug.Log("--PlayerObject:: Spawning a unit --");
        CmdSpawnUnit();
        //Instantiate(PlayerUnitPrefab);

    }



    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer == false)
        {
            return;
        }
        CmdMoveUnit();



    }



    // ---------------- COMANDS -------------------
    [Command]
    void CmdSpawnUnit() {

        GameObject go = Instantiate(PlayerUnitPrefab);
        blobberUnit = go;
        NetworkServer.Spawn(go);

    }

    [Command]
    void CmdMoveUnit()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        { blobberUnit.transform.Translate(0,0.5f,0); }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        { blobberUnit.transform.Translate(0, -0.5f, 0); }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        { blobberUnit.transform.Translate(0.5f, 0, 0); }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        { blobberUnit.transform.Translate(-0.5f, 0, 0); }

    }


}

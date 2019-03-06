
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
        {
            // This object belongs to another player.
            return;
        }

        Debug.Log("--PlayerObject:: Spawning a unit --");
        CmdSpawnUnit();
        //Instantiate(PlayerUnitPrefab);

    }



    // Update is called once per frame
    void Update()
    {
        // hasAuth = true im allowed to change stuff myself
        if (hasAuthority == false || isLocalPlayer == false)
        { return; }
      
        CmdMoveUnit();
      
        // Debug.Log("hasAuthority = true");






    }



    // ---------------- COMANDS -------------------
    [Command]
    void CmdSpawnUnit() {
        //go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient); we tell this go that connectionToClient has auth
        GameObject go = Instantiate(PlayerUnitPrefab);
        NetworkServer.SpawnWithClientAuthority(go,connectionToClient);
        blobberUnit = go;
    }

    [Command]
    void CmdMoveUnit()
    {

        

        //Debug.Log("WHY WONT I JUST MOVE AS CLIENT");
        /*
        if (Input.GetKeyDown(KeyCode.UpArrow))
        { blobberUnit.transform.Translate(0,0.3f,0); }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        { blobberUnit.transform.Translate(0, -0.5f, 0); }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        { blobberUnit.transform.Translate(0.5f, 0, 0); }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        { blobberUnit.transform.Translate(-0.5f, 0, 0); }
        */
    }


}

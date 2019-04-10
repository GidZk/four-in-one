using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

#pragma warning disable 618

public class PlayerConnection : NetworkBehaviour
// here goes the data for each attached object
{
    class ControlMessage : MessageBase
    {
        public ControlMessage()
        {
        }

        public ControlMessage(float value)
        {
            this.value = value;
        }

        public float value;
    }


    private NetworkClient m_Client;
    public GameObject PlayerUnitPrefab;
    GameObject blobberUnit;
    private const short ControlMsg = 1002;

    public void SendControlMsg(float Val)
    {
        m_Client.Send(ControlMsg, new ControlMessage(Val));
    }


    // Start is called before the first frame update
    void Start()
    {
        m_Client = NetworkManager.singleton.client;
        NetworkServer.RegisterHandler(ControlMsg, OnServerRecvControlMsg);

        if (isLocalPlayer == false)
        {
            // This object belongs to another player.
            return;
        }

        Debug.Log("--PlayerObject:: Spawning a unit --");
        CmdSpawnUnit();
        //Instantiate(PlayerUnitPrefab);
    }

    private void OnServerRecvControlMsg(NetworkMessage netmsg)
    {
        Debug.Log("recieved: " + netmsg.ReadMessage<ControlMessage>().value);
    }


    // Update is called once per frame
    void Update()
    {
        // hasAuth = true im allowed to change stuff myself
        if (hasAuthority == false || isLocalPlayer == false)
        {
            return;
        }

        if (Input.anyKey && !isServer) SendControlMsg(Random.value);
        CmdMoveUnit();

        // Debug.Log("hasAuthority = true");
    }


    // ---------------- COMANDS -------------------
    [Command]
    void CmdSpawnUnit()
    {
        //go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient); we tell this go that connectionToClient has auth
        GameObject go = Instantiate(PlayerUnitPrefab);
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
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
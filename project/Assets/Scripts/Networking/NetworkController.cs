using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

#pragma warning disable 618

public class NetworkController : MonoBehaviour, BroadcastListener, ManagerListener
{
    public MyNetworkDiscovery discovery;
    public MyNetworkManager manager;

    public Canvas SelectCanvas;
    public Canvas WaitCanvas;

    public MemberDisplayController memberDisplayController;

    private State m_State = State.Idle;

    private bool m_Localhost;

    private int m_NetworkId;
    private float m_StartHostTime = -1;

    private int m_MemberCount;

    public int MemberCount
    {
        get { return m_MemberCount; }
        set
        {
            m_MemberCount = value;
            if (m_State == State.Host)
            {
                Debug.Log($"Sent member change message: {m_MemberCount}");
                NetworkServer.SendToAll(Messages.MessageGiveMembersJoined, new IntegerMessage(m_MemberCount));
            }
        }
    }


    private void Update()
    {
        EvaluateServerState();
    }

    private void Awake()
    {
        discovery.Register(this);
        manager.Register(this);
    }

    /* Block managing state switch from client to host after a delay
     * written in Update() because co-routines are weird with networking sometimes
     */
    private void EvaluateServerState()
    {
        if (m_State == State.ClientSearching && Time.time > m_StartHostTime)
        {
            if (discovery.isClient)
            {
                discovery.StopBroadcast();
            }

            discovery.StartAsServer();
            m_State = State.Host;
            manager.StartHost();
        }
    }


    enum State
    {
        Idle,
        Host,
        ClientSearching,
        ClientOnServer
    }

    /*
     * The client object currently active
     */
    private NetworkClient Client()
    {
        if (manager != null)
            return manager.client;
        Debug.Log(" >>>> Manager missing client");
        return null;
    }


    public void OnTeamButtonPressed(TeamGameObject obj) => JoinTeam(obj.team);

    /**
     * Attempts to join the specified team
     *
     * If no host is found within 2 seconds, start hosting itself
     */
    private void JoinTeam(Team team)
    {
        Debug.Log($"Starting to join team {team}");
        if (discovery.running)
        {
            // TODO make sure this stops properly
            Debug.Log("Tried to change team when discovery was running");
        }

        memberDisplayController.Color = TeamUtil.GetTeamColor(team);
        discovery.broadcastData = team.ToString();

        var success = discovery.Initialize();
        if (success)
        {
            // listen to broadcasts for 2 seconds, if none of team found, switch to host
            m_StartHostTime = Time.time + 2f;
            discovery.StartAsClient();
            m_State = State.ClientSearching;
        }
        else
        {
            Debug.Log("Failed to init network discovery");
        }
    }

    public void OnHostButtonPressed()
    {
        if (discovery.isClient || discovery.isServer)
        {
            discovery.StopBroadcast();
        }

        var success = discovery.Initialize();
        if (success)
        {
            discovery.StartAsServer();
            manager.StartHost();
            m_State = State.Host;
        }
        else
            Debug.Log(" >>>> Port already in use");
    }

    public void OnJoinButtonPressed()
    {
        if (discovery.isClient || discovery.isServer)
        {
            discovery.StopBroadcast();
        }

        var success = discovery.Initialize();
        if (success)
        {
            discovery.StartAsClient();
            m_State = State.ClientSearching;
        }
        else
            Debug.Log(" >>>> Port already in use");
    }

    public void OnToggleLocalHostButton()
    {
        m_Localhost = !m_Localhost;
        Debug.Log($"Toggle localhost, now {m_Localhost} ");
    }

    public void OnReceivedBroadcast(string fromAddress, string data)
    {
        var ipv4 = fromAddress.Substring(7);
        manager.networkAddress = ipv4;
        var client = manager.StartClient();


        if (manager.isNetworkActive)
        {
            Debug.Log(">>>> connected!");
            discovery.StopBroadcast();
        }
        else
        {
            Debug.Log(" >>>> Could not establish connection to host");
        }
    }

    /**
     * Called on the server when a client connects to the server
     */
    public void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log(">>>> Client joined");
        var cId = conn.connectionId;
        MemberCount++;
        NetworkServer.SendToClient(cId, Messages.MessageGiveClientId, new IntegerMessage(cId));
    }

    /**
     * Called on the server when a client disconnects from the server
     */
    public void OnServerDisconnect(NetworkConnection conn)
    {
        // TODO
        MemberCount--;
    }

    /**
     * Called on the client when it connects to a server
     */
    public void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log(">>>> connected to server");
        InitClientHandlers();
        SelectCanvas.gameObject.SetActive(false);
        WaitCanvas.gameObject.SetActive(true);
        memberDisplayController.SetNumberJoined(0);
    }

    private void InitClientHandlers()
    {
        Client().RegisterHandler(Messages.MessageGiveClientId, OnClientRcvGiveClientId);
        Client().RegisterHandler(Messages.MessageGiveMembersJoined, OnClientRcvMembersJoined);
    }

    /**
     * Called ont he client when it disconnects from a server
     */

    public void OnClientDisconnect(NetworkConnection conn)
    {
        // TODO
    }

    /**
     * Called when connecting to a server to give the client an id
     */
    private void OnClientRcvGiveClientId(NetworkMessage netmsg)
    {
        m_NetworkId = netmsg.ReadMessage<IntegerMessage>().value;
        Debug.Log($"Received network ID {m_NetworkId}");
    }

    /**
     * Called on each server whenever the number of members in the team change
     */
    private void OnClientRcvMembersJoined(NetworkMessage netmsg)
    {
        var n = netmsg.ReadMessage<IntegerMessage>().value;
        Debug.Log($"Recieved member number change {n}");
        memberDisplayController.SetNumberJoined(n);
    }
}
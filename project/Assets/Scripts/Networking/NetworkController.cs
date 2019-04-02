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

    private State m_State = State.Idle;

    private int m_NetworkId;

    private void Awake()
    {
        discovery.Register(this);
        manager.Register(this);
    }

    enum State
    {
        Idle,
        Host,
        ClientSearching,
        ClientOnServer
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
        NetworkServer.SendToClient(cId, Messages.MessageGiveClientId, new IntegerMessage(cId));
    }

    /**
     * Called on the server when a client disconnects from the server
     */
    public void OnServerDisconnect(NetworkConnection conn)
    {
        // TODO
    }

    /**
     * Called on the client when it connects to a server
     */
    public void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log(">>>> connected to server");
        Client().RegisterHandler(Messages.MessageGiveClientId, OnClientRcvGiveClientId);
    }

    /**
     * Called ont he client when it disconnects from a server
     */

    public void OnClientDisconnect(NetworkConnection conn)
    {
        // TODO
    }

    private NetworkClient Client()
    {
        if (manager != null)
            return manager.client;
        Debug.Log(" >>>> Manager missing client");
        return null;
    }

    /**
     * Called when connecting to a server to give the client an id
     */
    private void OnClientRcvGiveClientId(NetworkMessage netmsg)
    {
        m_NetworkId = netmsg.ReadMessage<IntegerMessage>().value;
        Debug.Log($"Received network ID {m_NetworkId}");
    }
}
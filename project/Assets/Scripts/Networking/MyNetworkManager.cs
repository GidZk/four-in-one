using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager
{
    private readonly List<ManagerListener> m_Listeners = new List<ManagerListener>();

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"On server connect {conn.address}");
        foreach (var listener in m_Listeners)
        {
            listener.OnServerConnect(conn);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log($"On server disconnect {conn.address}");
        foreach (var listener in m_Listeners)
        {
            listener.OnServerDisconnect(conn);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log($"On client connect {conn.address}");
        foreach (var listener in m_Listeners)
        {
            listener.OnClientConnect(conn);
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Debug.Log($"On client disconnect{conn.address}");
        foreach (var listener in m_Listeners)
        {
            listener.OnClientDisconnect(conn);
        }
    }

    public void Register(ManagerListener listener)
    {
        m_Listeners.Add(listener);
    }
}

public interface ManagerListener
{
    void OnServerConnect(NetworkConnection conn);
    void OnServerDisconnect(NetworkConnection conn);
    void OnClientConnect(NetworkConnection conn);
    void OnClientDisconnect(NetworkConnection conn);
}
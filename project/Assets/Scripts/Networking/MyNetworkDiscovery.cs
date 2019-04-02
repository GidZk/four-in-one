using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkDiscovery : NetworkDiscovery
{
    private readonly List<BroadcastListener> m_Listeners = new List<BroadcastListener>();

    public void Register(BroadcastListener o)
    {
        m_Listeners.Add(o);
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        foreach (var listener in m_Listeners)
        {
            listener.OnReceivedBroadcast(fromAddress, data);
        }
    }
}

public interface BroadcastListener
{
    void OnReceivedBroadcast(string fromAddress, string data);
}
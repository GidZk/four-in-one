using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 618

public class MyNetworkDiscovery : NetworkDiscovery
{
    private readonly List<BroadcastListener> m_Listeners = new List<BroadcastListener>();

    public void Register(BroadcastListener o)
    {
        m_Listeners.Add(o);
    }

    private void OnEnable()
    {
        Initialize();
        m_HasRecievedBroadcast = false;
        StartAsClient();
    }

    private bool m_HasRecievedBroadcast;

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if (m_HasRecievedBroadcast) return;
        m_HasRecievedBroadcast = true;

        foreach (var listener in m_Listeners)
        {
            listener.OnReceivedBroadcast(fromAddress, data);
        }

        gameObject.SetActive(false);
    }
}

public interface BroadcastListener
{
    void OnReceivedBroadcast(string fromAddress, string data);
}
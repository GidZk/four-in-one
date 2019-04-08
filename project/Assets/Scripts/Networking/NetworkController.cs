﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Directory = System.IO.Directory;

#pragma warning disable 618

public class NetworkController : MonoBehaviour, BroadcastListener, ManagerListener, InputListener, InputInterface
{
    // TODO delete dis
    public Text _logText;

    private void Log(string s, Color c)
    {
        _logText.text = s + "\n" + _logText.text;

        String colorCode = "#" + ((int) (c.r * 0xFF)).ToString("X2")
                               + ((int) (c.g * 0xFF)).ToString("X2")
                               + ((int) (c.b * 0xFF)).ToString("X2");
        Debug.Log($"<color={colorCode}>" + s + "</color>");
    }

    private void Log(string s)
    {
        var prev = _logText.text;
        _logText.text = s + "\n" + prev;
        Debug.Log(s);
    }

    public MyNetworkDiscovery discovery;
    public MyNetworkManager manager;
    public HashSet<InputListener> inputListeners;

    // TODO move this away to some sort of UIController
    public Canvas selectCanvas;
    public Canvas waitCanvas;
    public MemberDisplayController memberDisplayController;

    // Strictly for debugging, logic should NOT depend on this
#pragma warning disable 414
    private State m_State = State.Idle;
#pragma warning restore 414

    private bool UseLocalhost { get; set; }
    public int NetworkId { get; private set; }
    private float StartHostTime { get; set; }
    private Team Team { get; set; }

    private int m_MemberCount;

    public int MemberCount
    {
        get => m_MemberCount;
        private set
        {
            m_MemberCount = value;
            if (!NetworkServer.active) return;
            Log($"Sent member change message: {m_MemberCount}", Color.blue);
            NetworkServer.SendToAll(Messages.MemberCount, new IntegerMessage(m_MemberCount));
        }
    }

    public bool IsServer() => NetworkServer.active;
    public bool IsClient() => Client() != null && Client().isConnected;

    public bool IsConnected()
    {
        if (Client() != null)
            return Client().isConnected;
        return false;
    }

    private void Update()
    {
        EvaluateServerState();

        if (Input.GetKeyDown(KeyCode.N))
        {
            NetworkServer.SendToAll(Messages.StartGame, new EmptyMessage());
        }
    }

    private void Awake()
    {
        StartHostTime = -1;
        discovery.Register(this);
        discovery.StopBroadcast();
        manager.Register(this);
        inputListeners = new HashSet<InputListener>();
        InitHostHandlers();
        DontDestroyOnLoad(this);
    }

    // Scrapes the Assets/Prefabs/Resources/Spawnable folder for prefabs and registers them
    private void RegisterSpawnable()
    {
        return;
        var path = Application.dataPath + "/Prefabs/Resources/Spawnable";
        var files = Directory.GetFiles(path);

        foreach (var file in files)
        {
            if (!file.EndsWith(".prefab")) return;

            char[] chars = {'/', '\\'};
            var i = file.LastIndexOfAny(chars);
            var relative = "Spawnable/" + file.Substring(i + 1);
            var prefabPath = relative.Remove(relative.IndexOf('.'));
            var prefab = Resources.Load(prefabPath) as GameObject;

            Log($"Registering {prefabPath} as spawnable", Color.green);
            ClientScene.RegisterPrefab(prefab);
        }
    }

    /**
     * Block managing state switch from client to host after a delay
     * written in Update() because co-routines are weird with networking sometimes
     */
    private void EvaluateServerState()
    {
        if (StartHostTime >= 0 &&
            Time.time > StartHostTime &&
            !IsServer() &&
            !IsConnected() &&
            !UseLocalhost)
        {
            Log($" |||| Starting host at {Time.time}");

            if (discovery.isClient) discovery.StopBroadcast();

            discovery.StartAsServer();
            m_State = State.Host;
            manager.StartHost();
        }
    }

    //The client object currently active
    private NetworkClient Client()
    {
        if (manager != null)
            return manager.client;
        Log(" >>>> Manager missing client");
        return null;
    }

    public void OnTeamButtonPressed(TeamGameObject obj) => JoinTeam(obj.team);

    public void OnToggleLocalhost()
    {
        UseLocalhost = !UseLocalhost;
        Log($"localhost: {UseLocalhost}");
    }

    public void OnReceivedBroadcast(string fromAddress, string data)
    {
        Log($" ==== BC from {fromAddress} ");
        var otherTeam = TeamUtil.FromString(data);
        if (otherTeam != Team)
            return;

        var ipv4 = "localhost";
        if (fromAddress != "localhost")
            ipv4 = fromAddress.Substring(7);

        manager.networkAddress = ipv4;
        var client = manager.StartClient();

        Log($"Attempting connect to {ipv4}");

        if (client.isConnected)
        {
            Log(" >>>> connected!", Color.green);
            if (discovery.running)
                discovery.StopBroadcast();
        }
        else
        {
            Log(" >>>> Could not establish connection to host", Color.yellow);
        }
    }

    /**
     * Attempts to join the specified team
     * If no host is found within 2 seconds, start hosting itself
     * */
    private void JoinTeam(Team team)
    {
        memberDisplayController.Color = TeamUtil.GetTeamColor(team);
        Team = team;

        if (UseLocalhost)
        {
            Log("Using localhost, sending faux broadcast");
            OnReceivedBroadcast("localhost", team.ToString());
            return;
        }

        Log($"Starting to join team {team}");
        if (discovery.running)
            // TODO make sure this stops properly
            Log("Tried to change team when discovery was running");

        discovery.broadcastData = team.ToString();

        var success = discovery.Initialize();
        discovery.StartAsClient();
        if (success)
        {
            // listen to broadcasts for 2 seconds, if none of team found, switch to host
            StartHostTime = Time.time + 2;
            Log($"setting startHostTime = {StartHostTime}");
            m_State = State.ClientSearching;
        }
        else
        {
            Log("Failed to init network discovery", Color.red);
        }
    }

    // ### Server triggers ###

    public void OnServerConnect(NetworkConnection conn)
    {
        Log(">>>> Client joined");
        var cId = conn.connectionId;
        MemberCount++;

        if (conn.address.Equals("localClient"))
        {
            Log("Identified connecting to self");
            Task.Delay(10).ContinueWith(t =>
            {
                NetworkServer.SendToClient(cId,
                    Messages.ClientId,
                    new IntegerMessage(cId));
            });
        }
        else
        {
            NetworkServer.SendToClient(cId,
                Messages.ClientId,
                new IntegerMessage(cId));
        }

        if (MemberCount > 4)
        {
            Log($"More than 4 connected ({MemberCount})", Color.red);
            throw new Exception($"More than 4 connected ({MemberCount})");
        }

        if (MemberCount == 4)
        {
            OnLobbyFilled();
        }
    }

    public void OnServerDisconnect(NetworkConnection conn)
    {
        // TODO
        MemberCount--;
    }

    private void OnServerRcvControlMessage(NetworkMessage message)
    {
        var msg = message.ReadMessage<ControlMessage>();
        var type = msg.Type;
        var val = msg.Value;
        Log($" <<<< Received control message containing {val} of type {type}");
        switch (type)
        {
            case ControlType.Vertical:
                foreach (var l in inputListeners)
                {
                    l.OnVerticalMovementInput(val);
                }

                break;
            case ControlType.Horizontal:
                foreach (var l in inputListeners)
                {
                    l.OnVerticalMovementInput(val);
                }

                break;
            case ControlType.CannonAngle:
                foreach (var l in inputListeners)
                {
                    l.OnVerticalMovementInput(val);
                }

                break;
            case ControlType.CannonLaunch:
                foreach (var l in inputListeners)
                {
                    l.OnVerticalMovementInput(val);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // ### Client triggers ###

    public void OnClientConnect(NetworkConnection conn)
    {
        Log(">>>> connected to server");
        InitClientHandlers();
        RegisterSpawnable();
        selectCanvas.gameObject.SetActive(false);
        waitCanvas.gameObject.SetActive(true);
    }

    public void OnClientDisconnect(NetworkConnection conn)
    {
        // TODO
    }

    private void OnClientRcvGiveClientId(NetworkMessage message)
    {
        NetworkId = message.ReadMessage<IntegerMessage>().value;
        Log($" <<<< Received network ID {NetworkId}");
    }

    private void OnClientRcvMembersJoined(NetworkMessage message)
    {
        var n = message.ReadMessage<IntegerMessage>().value;
        Log($" <<<< Received member number change {n}");
        memberDisplayController.SetNumberJoined(n);
    }

    private void OnClientRcvStartGame(NetworkMessage netmsg)
    {
        Log("Received start message");
        StartGame();
    }


    // ### Misc ###

    private void OnLobbyFilled()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("LobbyScene"))
        {
            Log("OnLobbyFilled called from outside lobby!");
            return;
        }

        discovery.StopBroadcast();

        NetworkServer.SendToAll(Messages.StartGame, new EmptyMessage());
    }

    // Registers the message handling that is to be received on  the clients' side
    private void InitClientHandlers()
    {
        Client().RegisterHandler(Messages.ClientId, OnClientRcvGiveClientId);
        Client().RegisterHandler(Messages.MemberCount, OnClientRcvMembersJoined);
        Client().RegisterHandler(Messages.StartGame, OnClientRcvStartGame);
    }

    // Registers the message handling that is to be received on the host's side
    private void InitHostHandlers()
    {
        NetworkServer.RegisterHandler(Messages.Control, OnServerRcvControlMessage);
    }

    private void StartGame() => SceneManager.LoadScene("GameScene", LoadSceneMode.Single);

    // ### Input Management ###

    public void OnVerticalMovementInput(float value)
    {
        if (IsServer())
            Log("NetworkController should not be input listener while being host", Color.yellow);
        Client().Send(Messages.Control, new ControlMessage(value, ControlType.Vertical));
    }

    public void OnHorizontalMovementInput(float value)
    {
        if (IsServer())
            Log("NetworkController should not be input listener while being host", Color.yellow);
        Client().Send(Messages.Control, new ControlMessage(value, ControlType.Horizontal));
    }

    public void OnCannonAngleInput(float value)
    {
        if (IsServer())
            Log("NetworkController should not be input listener while being host", Color.yellow);
        Client().Send(Messages.Control, new ControlMessage(value, ControlType.CannonAngle));
    }

    public void OnCannonLaunchInput(float value)
    {
        if (IsServer())
            Log("NetworkController should not be input listener while being host", Color.yellow);
        Client().Send(Messages.Control, new ControlMessage(value, ControlType.CannonLaunch));
    }

    public void Register(InputListener il) => inputListeners.Add(il);

    public bool Unregister(InputListener il) => inputListeners.Remove(il);
}

enum State
{
    Idle,
    Host,
    ClientSearching,
    ClientOnServer
}
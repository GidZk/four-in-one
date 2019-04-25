using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using Task = System.Threading.Tasks.Task;

// ReSharper disable All

#pragma warning disable 618

public class NetworkController : MonoBehaviour, BroadcastListener, ManagerListener, InputListener, InputInterface
{
    // TODO delete dis
    private GameObject playerRef;
    private GameState gameState;

    public MyNetworkDiscovery discovery;
    public MyNetworkManager manager;

    /// Contains all recieved fromAdresses from hosting servers and their team
    private Dictionary<string, Team> broadcastTable;

    private HashSet<InputListener> inputListeners;

    // TODO move this away to some sort of UIController
    public Canvas selectCanvas;
    public Canvas waitCanvas;
    public MemberDisplayController memberDisplayController;

    public bool UseLocalhost { get; private set; }
    public bool SingleGameDebug { get; set; }
    public int NetworkId { get; private set; }
    private bool StartHost { get; set; }

    private Team Team { get; set; }
    private int m_MemberCount;
    public Spawner spawnManager;

    public static NetworkController Instance { get; private set; }

    private void Awake()
    {
        Team = Team.None;
        if (Instance != null)
        {
            Debug.Log(Util.C($"Multiple NetworkControllers!!!", Color.red));
            throw new Exception();
        }

        Instance = this;

        discovery.Register(this);
        discovery.Initialize();
        Debug.Log("Discovery as client in awake");
        discovery.StartAsClient();
        manager.Register(this);
        inputListeners = new HashSet<InputListener>();
        broadcastTable = new Dictionary<string, Team>();
        InitHostHandlers();
        InitAndRegisterSpawnable();
        DontDestroyOnLoad(this);
        gameState = GameState.Lobby;
    }


    private void Update()
    {
        EvaluateServerState();

        if (gameState == GameState.RunningGame && IsServer())
        {
            // TODO wtf is this in network controller
            spawnManager.SpawnNpc(3);
        }


        if (Input.GetKeyDown(KeyCode.D) && IsServer())
        {
            OnLobbyFilled();
        }
    }

    public void OnToggleDebugText() => debugText = !debugText;
    private bool debugText = false;


    public int MemberCount
    {
        get => m_MemberCount;
        private set
        {
            m_MemberCount = value;
            if (!NetworkServer.active) return;
            Debug.Log($"Sent member change message: {m_MemberCount}");
            NetworkServer.SendToAll(Messages.MemberCount, new IntegerMessage(m_MemberCount));
        }
    }

    public bool IsServer() => NetworkServer.active;
    public bool IsClient() => Client() != null && Client().isConnected;
    public bool IsConnected() => IsClient();

    // Scrapes the Assets/Prefabs/Resources/Spawnable folder for prefabs and registers them
    private void InitAndRegisterSpawnable()
    {
        // returns the 3'rd child of a spawnmanager, which should always be spawnManager
        spawnManager = GameObject.Find("NWController/SpawnManager").GetComponent<Spawner>();
        foreach (var o in spawnManager.spawnable)
        {
            if (o == null)
            {
                Debug.Log(Util.C("null object in spawnable", Color.red));
                continue;
            }

            Debug.Log($"Registering {o} as spawnable");
            ClientScene.RegisterPrefab(o);
        }

//        // TODO fix dis
//        var path = Application.dataPath + "/Prefabs/Resources/Spawnable";
//        var files = Directory.GetFiles(path);
//
//        foreach (var file in files)
//        {
//            if (!file.EndsWith(".prefab")) return;
//
//            char[] chars = {'/', '\\'};
//            var i = file.LastIndexOfAny(chars);
//            var relative = "Spawnable/" + file.Substring(i + 1);
//            var prefabPath = relative.Remove(relative.IndexOf('.'));
//            var prefab = Resources.Load(prefabPath) as GameObject;
//
//            Log($"Registering {prefabPath} as spawnable", Color.green);
//            ClientScene.RegisterPrefab(prefab);
//        }
    }

    /**
     * Block managing state switch from client to host after a delay
     * written in Update() because co-routines are weird with networking sometimes
     */
    private void EvaluateServerState()
    {
        if (StartHost &&
            !IsServer() &&
            !IsConnected() &&
            !UseLocalhost)
        {
            Debug.Log($"Starting host");
            discovery.StopBroadcast();
            Debug.Log("Stopped broadcast in evaluate server state");

            discovery.StartAsServer();
            discovery.Initialize();
            Debug.Log("Discovery as server in evaluate server state");
            manager.StartHost();
            StartHost = false;
        }
    }

    //The client object currently active
    public NetworkClient Client()
    {
        if (manager != null)
            return manager.client;
        Debug.Log("Manager missing client");
        return null;
    }

    public void OnTeamButtonPressed(TeamGameObject obj) => JoinTeam(obj.team);

    public void OnToggleLocalhost()
    {
        UseLocalhost = !UseLocalhost;
        Debug.Log($"localhost: {UseLocalhost}");
    }

    public void OnReceivedBroadcast(string fromAddress, string data)
    {
        var otherTeam = TeamUtil.FromIdent(data.Substring(0, 2));
        Debug.Log($"Broadcast from {fromAddress}, team: {otherTeam}");
        if (broadcastTable.ContainsKey(fromAddress))
        {
            broadcastTable[fromAddress] = otherTeam;
        }
        else
        {
            broadcastTable.Add(fromAddress, otherTeam);
        }

        if (otherTeam != Team)
            return;

        ConnectTo(fromAddress);
    }

    private bool ConnectTo(string fromAddress)
    {
        var ipv4 = "localhost";
        if (fromAddress != "localhost")
            ipv4 = fromAddress.Substring(7);
        manager.networkAddress = ipv4;
        var client = manager.StartClient();
        Debug.Log($"Attempting connect to {ipv4}");
        return true;
    }

    /**
     * Attempts to join the specified team
     * If no host is found within 2 seconds, start hosting itself
     * */
    private void JoinTeam(Team team)
    {
        Debug.Log($"Starting to join team {team}");
        memberDisplayController.Color = TeamUtil.GetTeamColor(team);
        Team = team;

        if (UseLocalhost)
        {
            Debug.Log("Using localhost, sending faux broadcast");
            OnReceivedBroadcast("localhost", team.ToString());
            return;
        }

        if (broadcastTable.ContainsValue(team))
        {
            Debug.Log("- appropriate team already found in broadcastTable, joining");
            foreach (var p in broadcastTable)
            {
                if (p.Value != team)
                    continue;

                ConnectTo(p.Key);
            }
        }

        discovery.broadcastData = TeamUtil.ToIdent(team).ToString();

        var success = discovery.Initialize();
        Debug.Log("Discovery as client in JoinTeam");
        discovery.StartAsClient();
        if (success)
        {
            // listen to broadcasts for 2 seconds, if none of same team found, switch to host
            Debug.Log($" - starting host in 2 seconds");
            Task.Delay(2000).ContinueWith(t => StartHost = true);
        }
        else
        {
            Debug.Log(Util.C("Failed to init network discovery", Color.red));
        }
    }

    // ### Server triggers ###

    public void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("Client joined");
        var cId = conn.connectionId;
        MemberCount++;

        if (conn.address.Equals("localClient"))
        {
            Debug.Log("Identified connecting to self");
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
            Debug.Log($"More than 4 connected ({MemberCount})");
            throw new Exception($"More than 4 users connected ({MemberCount})");
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

    private int nPuzzleReady;

    private void OnServerRcvPuzzleReady(NetworkMessage message)
    {
        var msg = message.ReadMessage<IntegerMessage>();
        if (msg.value == 0) // 0 = un-ready, 1 = ready
            nPuzzleReady--;
        else
            nPuzzleReady++;

        if (nPuzzleReady >= MemberCount)
            NetworkServer.SendToAll(Messages.ClearPuzzle, new EmptyMessage());

        Debug.Log($"Recieved puzzle ready: {nPuzzleReady}/{MemberCount}");
    }


    /*Callback registred on the server by host to be executed server side */
    private void OnServerRcvControlMessage(NetworkMessage message)
    {
        ControlMessage msg = message.ReadMessage<ControlMessage>();
        ControlType type = msg.Type;
        float val = msg.Value;
        Debug.Log($" Received control message containing {val} of type {type}");
        switch (type)
        {
            case ControlType.Vertical:
                foreach (var l in inputListeners)
                {
                    Debug.Log($" Received control message containing {val} of type {type}");

                    l.OnVerticalMovementInput(val);
                }

                break;
            case ControlType.Horizontal:
                foreach (var l in inputListeners)
                {
                    l.OnHorizontalMovementInput(val);
                }

                break;
            case ControlType.CannonAngle:
                foreach (var l in inputListeners)
                {
                    l.OnCannonAngleInput(val);
                }

                break;
            case ControlType.CannonLaunch:
                foreach (InputListener l in inputListeners)
                {
                    l.OnCannonLaunchInput(val);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // ### Client triggers ###

    public void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Connected to server");
        if (!IsServer())
        {
            discovery.StopBroadcast();
            Debug.Log("Stopped broadcast in on client connect");
        }

        InitClientHandlers();
        selectCanvas.gameObject.SetActive(false);
        waitCanvas.gameObject.SetActive(true);
    }

    public void OnClientDisconnect(NetworkConnection conn)
    {
        // TODO
        Debug.Log(Util.C("Disconnected from server", Color.red));
    }

    private void OnClientRcvGiveClientId(NetworkMessage message)
    {
        NetworkId = message.ReadMessage<IntegerMessage>().value;
        Debug.Log($" Received network ID {NetworkId}");
    }

    private void OnClientRcvMembersJoined(NetworkMessage message)
    {
        var n = message.ReadMessage<IntegerMessage>().value;
        Debug.Log($"Received member number change {n}");
        memberDisplayController.SetNumberJoined(n);
    }

    private void OnClientRcvStartGame(NetworkMessage netmsg)
    {
        Debug.Log("Received start message");
        StartGame();
    }

    private void OnClientRcvClearPuzzle(NetworkMessage netmsg)
    {
        Debug.Log("Recieved ok to clear puzzle!");
        var go = GameObject.FindWithTag("Puzzle");
        if (go == null)
            throw new Exception("Could not find puzzle");
        go.GetComponent<RemovePuzzle>().Clear();
    }

    // ### Misc ###

    private void OnLobbyFilled()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("LobbyScene"))
        {
            Debug.Log("OnLobbyFilled called from outside lobby!");
            return;
        }


        Debug.Log("Stopped broadcast in evaluate stop broadcast");
        discovery.StopBroadcast();

        Task.Delay(500).ContinueWith(
            t => NetworkServer.SendToAll(Messages.StartGame, new EmptyMessage()));
    }

    // Registers the message handling that is to be received on  the clients' side
    private void InitClientHandlers()
    {
        Client().RegisterHandler(Messages.ClientId, OnClientRcvGiveClientId);
        Client().RegisterHandler(Messages.MemberCount, OnClientRcvMembersJoined);
        Client().RegisterHandler(Messages.StartGame, OnClientRcvStartGame);
        Client().RegisterHandler(Messages.ClearPuzzle, OnClientRcvClearPuzzle);
    }

    // Registers the message handling that is to be received on the host's side
    private void InitHostHandlers()
    {
        NetworkServer.RegisterHandler(Messages.Control, OnServerRcvControlMessage);
        NetworkServer.RegisterHandler(Messages.PuzzleReady, OnServerRcvPuzzleReady);
    }

    public void StartGame()
    {
        if (MemberCount <= 1 && IsServer())
        {
            SingleGameDebug = true;
            Debug.Log("Starting in single player debug mode");
        }

        Debug.Log("NetworkController:: starting game --");
        this.gameState = GameState.RunningGame;
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);

        //spawn dude
    }


    private void NotifyHorizontalMovementListners(float value)
    {
        foreach (var l in inputListeners)
        {
            l.OnHorizontalMovementInput(value);
        }
    }

    private void NotifyVerticalMovementListners(float value)
    {
        foreach (var l in inputListeners)
        {
            l.OnVerticalMovementInput(value);
        }
    }


    // ### Input Management ###

    public void OnVerticalMovementInput(float value)
    {
        if (IsServer())
            NotifyVerticalMovementListners(value);
        else
            Client().Send(Messages.Control, new ControlMessage(value, ControlType.Vertical));
    }

    public void OnHorizontalMovementInput(float value)
    {
        if (IsServer())
            NotifyHorizontalMovementListners(value);
        else
            Client().Send(Messages.Control, new ControlMessage(value, ControlType.Horizontal));
    }

    public void OnCannonAngleInput(float value)
    {
        if (IsServer())
            foreach (var il in inputListeners)
            {
                il.OnCannonAngleInput(value);
            }

        //Log("NetworkController should not be input listener while being host", Color.yellow);
        if (IsConnected())
            Client().Send(Messages.Control, new ControlMessage(value, ControlType.CannonAngle));
    }

    public void OnCannonLaunchInput(float value)
    {
        if (IsServer())
            foreach (var il in inputListeners)
            {
                il.OnCannonLaunchInput(value);
            }

        //Log("NetworkController should not be input listener while being host", Color.yellow);
        if (IsConnected())
            Client().Send(Messages.Control, new ControlMessage(value, ControlType.CannonLaunch));
    }

    public void Register(InputListener il) => inputListeners.Add(il);

    public bool Unregister(InputListener il) => inputListeners.Remove(il);

    /// Signal to the controller that the puzzle on this client is ready
    public void PuzzleReady(bool b)
    {
        Client().Send(Messages.PuzzleReady, new IntegerMessage(b ? 1 : 0));
    }
}

enum State
{
    Idle,
    Host,
    ClientSearching,
    ClientOnServer,
    Lobby,
    Game
}

enum GameState
{
    Lobby,
    RunningGame
}

public static class Util
{
    /// <summary>
    /// Returns the input string wrapped with "color"-tags 
    /// </summary>
    /// <param name="s">a string</param> 
    /// <param name="c">a Color</param> 
    /// <returns>A string wrapped with color tags</returns>
    public static string C(String s, Color c)
    {
        String colorCode = "#" + ((int) (c.r * 0xFF)).ToString("X2")
                               + ((int) (c.g * 0xFF)).ToString("X2")
                               + ((int) (c.b * 0xFF)).ToString("X2");

        return $"<color={colorCode}>" + s + "</color>";
    }
}
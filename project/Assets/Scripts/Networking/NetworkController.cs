using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// ReSharper disable All

#pragma warning disable 618

public class NetworkController : MonoBehaviour, BroadcastListener, ManagerListener, InputListener, InputInterface
{
    // TODO delete dis
    public Text _logText;
    private GameObject playerRef;
    private GameState gameState;


    public MyNetworkDiscovery discovery;
    public MyNetworkManager manager;
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
        if (Instance != null)
        {
            Log($"Multiple NetworkControllers!!!", Color.red);
            throw new Exception();
        }

        Instance = this;

        discovery.Register(this);
        discovery.Initialize();
        discovery.StartAsClient();
        manager.Register(this);
        inputListeners = new HashSet<InputListener>();
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
            spawnManager.SpawnNpc(3);
        }


        if (Input.GetKeyDown(KeyCode.D) && IsServer())
        {
            OnLobbyFilled();
        }
    }

    private void ToDebugText(String s)
    {
        if (debugText)
        {
            _logText.text = s + "\n" + _logText.text;
        }
    }

    public void Log(string s, Color c)
    {
        ToDebugText(s);

        String colorCode = "#" + ((int) (c.r * 0xFF)).ToString("X2")
                               + ((int) (c.g * 0xFF)).ToString("X2")
                               + ((int) (c.b * 0xFF)).ToString("X2");


        Debug.Log($"<color={colorCode}>" + s + "</color>");
    }

    private void Log(string s)
    {
        ToDebugText(s);
        Debug.Log(s);
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
            Log($"Sent member change message: {m_MemberCount}", Color.magenta);
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
                Log("null object in spawnable", Color.yellow);
                continue;
            }

            Log($"Registering {o} as spawnable", new Color(128, 34, 80));
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
            Log($"Starting host", Color.green);
            if (discovery.isClient) discovery.StopBroadcast();
            discovery.StartAsServer();
            manager.StartHost();
            StartHost = false;
        }
    }

    //The client object currently active
    public NetworkClient Client()
    {
        if (manager != null)
            return manager.client;
        Log("Manager missing client", Color.yellow);
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
        Log($"BC from {fromAddress}", Color.cyan);
        var otherTeam = TeamUtil.FromInt(data[0]);
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
            Log("Connected!", Color.green);
            if (discovery.running)
                discovery.StopBroadcast();
        }
        else
        {
            Log("Could not establish connection to host", Color.yellow);
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
            Log("Tried to change team when discovery was running", Color.yellow);

        discovery.broadcastData = TeamUtil.ToInt(team).ToString();

        var success = discovery.Initialize();
        discovery.StartAsClient();
        if (success)
        {
            // listen to broadcasts for 2 seconds, if none of team found, switch to host
            Log($"starting host in 2 seconds");
            Task.Delay(2000).ContinueWith(t => StartHost = true);
        }
        else
        {
            Log("Failed to init network discovery", Color.red);
        }
    }

    // ### Server triggers ###

    public void OnServerConnect(NetworkConnection conn)
    {
        Log("Client joined", Color.cyan);
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


    /*Callback registred on the server by host to be executed server side */
    private void OnServerRcvControlMessage(NetworkMessage message)
    {
        ControlMessage msg = message.ReadMessage<ControlMessage>();
        ControlType type = msg.Type;
        float val = msg.Value;
        Log($" Received control message containing {val} of type {type}", Color.cyan);
        switch (type)
        {
            case ControlType.Vertical:
                foreach (var l in inputListeners)
                {
                    Log($" Received control message containing {val} of type {type}", Color.cyan);

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
        Log("Connected to server", Color.green);
        InitClientHandlers();
        selectCanvas.gameObject.SetActive(false);
        waitCanvas.gameObject.SetActive(true);
    }

    public void OnClientDisconnect(NetworkConnection conn)
    {
        // TODO
        Log("Disconnected from server", Color.red);
    }

    private void OnClientRcvGiveClientId(NetworkMessage message)
    {
        NetworkId = message.ReadMessage<IntegerMessage>().value;
        Log($" Received network ID {NetworkId}", Color.cyan);
    }

    private void OnClientRcvMembersJoined(NetworkMessage message)
    {
        var n = message.ReadMessage<IntegerMessage>().value;
        Log($"Received member number change {n}", Color.cyan);
        memberDisplayController.SetNumberJoined(n);
    }

    private void OnClientRcvStartGame(NetworkMessage netmsg)
    {
        Log("Received start message", Color.cyan);
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

    private void StartGame()
    {
        if (MemberCount <= 1 && IsServer())
            SingleGameDebug = true;
        Log("NetworkController:: starting game --", Color.green);
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
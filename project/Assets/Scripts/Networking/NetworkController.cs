using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Directory = System.IO.Directory;

#pragma warning disable 618

namespace Networking
{
    public class NetworkController : MonoBehaviour, BroadcastListener, ManagerListener
    {
        public MyNetworkDiscovery discovery;
        public MyNetworkManager manager;

        // TODO move this away to some sort of UIController
        public Canvas selectCanvas;
        public Canvas waitCanvas;
        public MemberDisplayController memberDisplayController;

        // Strictly for debugging, logic should NOT depend on this
        private State m_State = State.Idle;

        private bool UseLocalhost { get; set; }
        private int NetworkId { get; set; }
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
                Debug.Log($"Sent member change message: {m_MemberCount}");
                NetworkServer.SendToAll(Messages.MessageGiveMembersJoined, new IntegerMessage(m_MemberCount));
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

            // dummy function for testing that spawning/movement works
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsClient())
                {
                    Client().Send(Messages.ControlMessage, new IntegerMessage(NetworkId));
                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (IsServer())
                {
                    var go = Instantiate(Resources.Load("Spawnable/dummyobj"),
                        new Vector3(0, 0, 0),
                        Quaternion.identity) as GameObject;
                    NetworkServer.Spawn(go);
                }
            }
        }

        private void Awake()
        {
            StartHostTime = -1;
            discovery.Register(this);
            discovery.StopBroadcast();
            manager.Register(this);
            InitHostHandlers();
        }

        //Scrapes the Assets/Prefabs/Resources/Spawnable folder for prefabs and registers them
        private void RegisterSpawnable()
        {
            var path = Application.dataPath + "/Prefabs/Resources/Spawnable";
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                if (!file.EndsWith(".prefab")) return;
                char[] chars = new[] {'/', '\\'};
                var i = file.LastIndexOfAny(chars);
                var relative = "Spawnable/" + file.Substring(i + 1);
                var prefabPath = relative.Remove(relative.IndexOf('.'));
                var prefab = Resources.Load(prefabPath) as GameObject;
                Debug.Log($"Registering {prefabPath} as spawnable");
                ClientScene.RegisterPrefab(prefab);
            }
        }

        //Block managing state switch from client to host after a delay
        //written in Update() because co-routines are weird with networking sometimes
        private void EvaluateServerState()
        {
            if (StartHostTime >= 0 &&
                Time.time > StartHostTime
                && !IsServer()
                && !IsConnected())
            {
                Debug.Log($" |||| Starting host at {Time.time}");
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

        //The client object currently active
        private NetworkClient Client()
        {
            if (manager != null)
                return manager.client;
            Debug.Log(" >>>> Manager missing client");
            return null;
        }


        public void OnTeamButtonPressed(TeamGameObject obj) => JoinTeam(obj.team);

        // Attempts to join the specified team
        // If no host is found within 2 seconds, start hosting itself
        private void JoinTeam(Team team)
        {
            Debug.Log($"Starting to join team {team}");
            if (discovery.running)
            {
                // TODO make sure this stops properly
                Debug.Log("Tried to change team when discovery was running");
            }

            memberDisplayController.Color = TeamUtil.GetTeamColor(team);
            Team = team;
            discovery.broadcastData = team.ToString();

            var success = discovery.Initialize();
            discovery.StartAsClient();
            if (success)
            {
                // listen to broadcasts for 2 seconds, if none of team found, switch to host
                StartHostTime = Time.time + 2f;
                Debug.Log($"setting startHostTime = {StartHostTime}");
                m_State = State.ClientSearching;
            }
            else
            {
                Debug.Log("Failed to init network discovery");
            }
        }

        public void OnToggleLocalHostButton()
        {
            UseLocalhost = !UseLocalhost;
            Debug.Log($"Toggle localhost, now {UseLocalhost} ");
        }

        public void OnReceivedBroadcast(string fromAddress, string data)
        {
            Debug.Log($" ==== BC from {fromAddress} ");
            var otherTeam = TeamUtil.FromString(data);
            if (otherTeam != Team)
                return;


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

        // Called on the server when a client connects to the server
        public void OnServerConnect(NetworkConnection conn)
        {
            Debug.Log(">>>> Client joined");
            var cId = conn.connectionId;
            MemberCount++;

            if (conn.address.Equals("localClient"))
            {
                Debug.Log("Identified connecting to self");
                Task.Delay(10).ContinueWith(t =>
                {
                    NetworkServer.SendToClient(cId,
                        Messages.MessageGiveClientId,
                        new IntegerMessage(cId));
                });
            }
            else
            {
                NetworkServer.SendToClient(cId,
                    Messages.MessageGiveClientId,
                    new IntegerMessage(cId));
            }
        }

        // Called on the server when a client disconnects from the server
        public void OnServerDisconnect(NetworkConnection conn)
        {
            // TODO
            MemberCount--;
        }

        // Called on the client when it connects to a server
        public void OnClientConnect(NetworkConnection conn)
        {
            Debug.Log(">>>> connected to server");
            InitClientHandlers();
            RegisterSpawnable();
            selectCanvas.gameObject.SetActive(false);
            waitCanvas.gameObject.SetActive(true);
        }


        // Called ont he client when it disconnects from a server
        public void OnClientDisconnect(NetworkConnection conn)
        {
            // TODO
        }

        // Called when connecting to a server to give the client an id
        private void OnClientRcvGiveClientId(NetworkMessage message)
        {
            NetworkId = message.ReadMessage<IntegerMessage>().value;
            Debug.Log($" <<<< Received network ID {NetworkId}");
        }

        // Called on each server whenever the number of members in the team change
        private void OnClientRcvMembersJoined(NetworkMessage message)
        {
            var n = message.ReadMessage<IntegerMessage>().value;
            Debug.Log($" <<<< Received member number change {n}");
            memberDisplayController.SetNumberJoined(n);
        }

        private void OnServerRcvControlMessage(NetworkMessage message)
        {
            var n = message.ReadMessage<IntegerMessage>().value;
            Debug.Log($" <<<<     Received control message containing {n}");
        }

        // Registers the message handling that is to be received on  the clients' sidee
        private void InitClientHandlers()
        {
            Client().RegisterHandler(Messages.MessageGiveClientId, OnClientRcvGiveClientId);
            Client().RegisterHandler(Messages.MessageGiveMembersJoined, OnClientRcvMembersJoined);
        }

        private void InitHostHandlers()
        {
            NetworkServer.RegisterHandler(Messages.ControlMessage, OnServerRcvControlMessage);
        }
    }
}
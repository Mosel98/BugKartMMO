using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Delegates;
using System.IO;
using UnityEngine.SceneManagement;
using Network.Messages;
using Network.Lobby;

namespace Network
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        public bool IsServer
        {
            get
            {
                return m_isServer;
            }
        }

        public bool IsClient
        {
            get
            {
                return !m_isServer;
            }
        }

        
        [SerializeField]
        private Transform[] m_StartPoint = new Transform[5];

        public bool m_DisplayGUI = false;
        public string m_Address = "127.0.0.1";
        public int m_Port = 4242;
        public int m_MaxNumberOfEventsPerFrame = 3;
        public NetworkIdentity m_PlayerPrefab;
        public bool m_AutoSpawnPlayer = true;

        protected byte m_reliableChannel;
        protected byte m_unreliableChannel;
        protected byte m_unreliableStateUpdateChannel;

        protected HostTopology m_hostTopology;

        // SocketID
        protected int m_hostID;
        protected int m_clientID;
        protected bool m_isServer;

        protected List<int> m_allClients = new List<int>();
        protected Dictionary<uint, NetworkIdentity> m_allGameObjects
            = new Dictionary<uint, NetworkIdentity>();
        protected Dictionary<int, NetworkIdentity> m_allPrefabs
            = new Dictionary<int, NetworkIdentity>();

        [SerializeField]
        protected List<NetworkIdentity> m_prefabs = new List<NetworkIdentity>();

        protected event ClientConnectedDelegate m_onClientConnected;
        protected event ClientDisconnectedDelegate m_onClientDisconnected;
        protected event DataReceivedDelegate m_onDataReceived;

        protected uint m_nextNetID = 1;

        protected virtual void OnGUI()
        {
            if (!m_DisplayGUI)
                return;
            if (NetworkTransport.IsStarted)
            {
                if (GUILayout.Button("Send Message"))
                {
                    SendMessage(null);
                }
                if (GUILayout.Button("Spawn Object"))
                {
                    NetworkIdentity go = Instantiate(m_prefabs[2], new Vector3(2, 5, -2), Quaternion.identity);
                    SpawnGameObject(go.gameObject);
                }
                if (GUILayout.Button("Honk"))
                {
                    HonkMessage message = new HonkMessage();
                    message.ClipID = Random.Range(0, 3);
                    if (IsServer)
                    {
                        SendMessageToClients(message);
                    }
                    else
                    {
                        SendMessageToServer(message);
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Start Host"))
                {
                    StartHost();
                }

                if (GUILayout.Button("Start Client"))
                {
                    StartClient();
                }


            }

        }

        protected virtual void OnDestroy()
        {
            NetworkTransport.Shutdown();
        }

        protected virtual void Awake()
        {
            if (Instance is object)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            int i = 1;
            foreach (NetworkIdentity identity in m_prefabs)
            {
                identity.Init(true, 0, i, false);
                m_allPrefabs.Add(i, identity);
                i++;
            }
        }

        protected virtual void Start()
        {
            SubscribeToOnClientConnected((_id) => Debug.Log($"Client verbunden {_id}!"));
            SubscribeToOnClientDisconnected((_id) => Debug.Log($"Client weg {_id}!"));
            m_onDataReceived += NetworkManager_m_onDataReceived;
            m_onClientConnected += AddToAllClients;
            m_onClientConnected += SpawnPlayerPrefab;
            m_onClientConnected += SendAllObjectsToClient;
            SceneManager.sceneLoaded += SceneWasLoaded;
        }

        protected virtual void Update()
        {
            if (!NetworkTransport.IsStarted)
                return;
            if (m_hostID == -1)
                return;

            ReceiveEvents();
        }

        public virtual void SendAllObjectsToClient(int _id)
        {
            SpawnMessage spawnMessage;
            foreach (KeyValuePair<uint, NetworkIdentity> kv in m_allGameObjects)
            {
                spawnMessage = new SpawnMessage(kv.Value.gameObject);
                SendMessageToClient(_id, spawnMessage);
            }
        }

        public void StartAsHost()
        {
            StartHost();
        }

        public void StartAsClient()
        {
            StartClient();
        }

        protected virtual bool StartHost()
        {
            Initialize();

            m_hostID = NetworkTransport.AddHost(m_hostTopology, m_Port);

            byte error;
            m_clientID = NetworkTransport.Connect(m_hostID, m_Address,
                m_Port, 0, out error);

            NetworkError networkError = (NetworkError)error;
            if (networkError != NetworkError.Ok)
            {
                Debug.LogError($"Fehler beim Erstellen des Hosts! ({networkError})",
                                this);
                return false;
            }
            m_isServer = true;

            NetworkIdentity[] identities = FindObjectsOfType<NetworkIdentity>();
            foreach (NetworkIdentity identity in identities)
            {
                SpawnGameObject(identity.gameObject);
            }

            return true;
        }

        protected virtual bool StartClient()
        {
            Initialize();

            NetworkIdentity[] identities = FindObjectsOfType<NetworkIdentity>();

            foreach (NetworkIdentity identity in identities)
            {
                Destroy(identity.gameObject);
            }

            m_hostID = NetworkTransport.AddHost(m_hostTopology);

            byte error;
            m_clientID = NetworkTransport.Connect(m_hostID, m_Address, m_Port, 0, out error);

            NetworkError networkError = (NetworkError)error;
            if (networkError != NetworkError.Ok)
            {
                Debug.LogError($"Fehler beim Erstellen des Clients! ({networkError})",
                    this);
                return false;
            }
            m_isServer = false;
            return true;
        }

        public void BuildPrefabIDs()
        {
            int i = 1;
            foreach (NetworkIdentity identity in m_prefabs)
            {
                identity.Init(true, 0, i, false);
                Debug.Log($"Set prefab id for {identity.gameObject}: {i}");
                i++;
            }
        }

        public void AddToIdentities(NetworkIdentity _identity)
        {
            if (_identity is null || m_allGameObjects.ContainsKey(_identity.NetID))
                return;

            m_allGameObjects.Add(_identity.NetID, _identity);
        }

        public void RemoveIdentities(NetworkIdentity _identity)
        {
            m_allGameObjects.Remove(_identity.NetID);
        }

        public NetworkIdentity GetIdentity(uint _id)
        {
            if (m_allGameObjects.ContainsKey(_id))
            {
                if (m_allGameObjects[_id] is null)
                    return null;
                return m_allGameObjects[_id];
            }
            return null;
        }

        public NetworkIdentity GetPrefabIdentity(int _id)
        {
            if (m_allPrefabs.ContainsKey(_id))
            {
                return m_allPrefabs[_id];
            }
            return null;
        }

        public void SpawnGameObject(GameObject _go)
        {
            NetworkIdentity identity = _go.GetComponent<NetworkIdentity>();
            identity.Init(true, m_nextNetID++, identity.PrefabID, false);
            AddToIdentities(identity);
            SpawnMessage message = new SpawnMessage(_go);
            SendMessageToClients(message);
        }

        public void SpawnGameObjectAsLocalPlayer(GameObject _go, int _playerID)
        {
            NetworkIdentity identity = _go.GetComponent<NetworkIdentity>();
            if (_playerID == 2)
            {
                identity.Init(true, m_nextNetID++, identity.PrefabID, true);
            }
            else
            {
                identity.Init(true, m_nextNetID++, identity.PrefabID, false);
            }
            AddToIdentities(identity);
            SpawnMessage localPlayerMessage = new SpawnMessage(_go, true);
            SpawnMessage spawnMessage = new SpawnMessage(_go);

            foreach (int client in m_allClients)
            {
                if (client == _playerID)
                {
                    SendMessageToClient(client, localPlayerMessage);
                }
                else
                {
                    SendMessageToClient(client, spawnMessage);
                }
            }
        }

        public void DestroyGameObject(GameObject _go)
        {
            DestroyMessage message = new DestroyMessage(_go);
            SendMessageToClients(message);
        }

        public void SendMessageToServer(AMessageBase _message, QosType _channel = QosType.Reliable)
        {
            int bytesLength;
            byte[] buffer = _message.Serialize(out bytesLength);

            byte channelID = 0;
            switch (_channel)
            {
                case QosType.Unreliable:
                    channelID = m_unreliableChannel;
                    break;
                case QosType.Reliable:
                    channelID = m_reliableChannel;
                    break;
                case QosType.StateUpdate:
                    channelID = m_unreliableStateUpdateChannel;
                    break;
                default:
                    Debug.LogError("Could not find channel! " + _channel);
                    break;
            }

            byte error;
            NetworkTransport.Send(m_hostID, m_clientID, channelID,
                buffer, bytesLength, out error);

            NetworkError networkError = (NetworkError)error;
            if (networkError != NetworkError.Ok)
            {
                Debug.LogError("Fehler beim Senden!" + networkError);
            }
        }

        public void SendMessageToClients(AMessageBase _message, QosType _channel = QosType.Reliable)
        {
            int bytesLength;
            byte[] buffer = _message.Serialize(out bytesLength);

            byte error;

            byte channelID = 0;
            switch (_channel)
            {
                case QosType.Unreliable:
                    channelID = m_unreliableChannel;
                    break;
                case QosType.Reliable:
                    channelID = m_reliableChannel;
                    break;
                case QosType.StateUpdate:
                    channelID = m_unreliableStateUpdateChannel;
                    break;
                default:
                    Debug.LogError("Could not find channel! " + _channel);
                    break;
            }

            foreach (int id in m_allClients)
            {
                if (id == m_clientID || id == m_clientID + 1)
                    continue;

                NetworkTransport.Send(m_hostID, id, channelID,
                    buffer, bytesLength, out error);

                NetworkError networkError = (NetworkError)error;
                if (networkError != NetworkError.Ok)
                {
                    Debug.LogError("Fehler beim Senden!" + networkError);
                }
            }

        }

        public void SendMessageToClient(int _id, AMessageBase _message, QosType _channel = QosType.Reliable)
        {
            int bytesLength;
            byte[] buffer = _message.Serialize(out bytesLength);

            byte error;

            byte channelID = 0;
            switch (_channel)
            {
                case QosType.Unreliable:
                    channelID = m_unreliableChannel;
                    break;
                case QosType.Reliable:
                    channelID = m_reliableChannel;
                    break;
                case QosType.StateUpdate:
                    channelID = m_unreliableStateUpdateChannel;
                    break;
                default:
                    Debug.LogError("Could not find channel! " + _channel);
                    break;
            }

            NetworkTransport.Send(m_hostID, _id, channelID,
                buffer, bytesLength, out error);

            NetworkError networkError = (NetworkError)error;
            if (networkError != NetworkError.Ok)
            {
                Debug.LogError("Fehler beim Senden!" + networkError);
            }
        }

        #region ---Event Methods---
        public void SubscribeToOnClientConnected(ClientConnectedDelegate _func)
        {
            m_onClientConnected -= _func;
            m_onClientConnected += _func;
        }

        public void UnsubscribeFromOnClientConnected(ClientConnectedDelegate _func)
        {
            m_onClientConnected -= _func;
        }

        public void SubscribeToOnClientDisconnected(ClientDisconnectedDelegate _func)
        {
            m_onClientDisconnected -= _func;
            m_onClientDisconnected += _func;
        }

        public void UnsubscribeFromOnClientDisconnected(ClientDisconnectedDelegate _func)
        {
            m_onClientDisconnected -= _func;
        }

        public void SubscribeToOnDataReceived(DataReceivedDelegate _func)
        {
            m_onDataReceived -= _func;
            m_onDataReceived += _func;
        }

        public void UnsubscribeFromOnDataReceived(DataReceivedDelegate _func)
        {
            m_onDataReceived -= _func;
        }
        #endregion ---Event Methods---

        private void SceneWasLoaded(Scene _scene, LoadSceneMode _mode)
        {
            if (IsClient)
            {
                NetworkIdentity[] identities = FindObjectsOfType<NetworkIdentity>();

                foreach (NetworkIdentity identity in identities)
                {
                    if (identity.GetComponent<LobbyPlayer>())
                        continue;

                    Destroy(identity.gameObject);
                }

                SceneLoadedMessage message = new SceneLoadedMessage();
                SendMessageToServer(message);
            }
            else if (IsServer)
            {
                SwitchSceneMessage message = new SwitchSceneMessage();
                message.ScenePath = _scene.path;

                SendMessageToClients(message);

                NetworkIdentity[] identities = FindObjectsOfType<NetworkIdentity>();

                foreach (NetworkIdentity identity in identities)
                {
                    if (identity.GetComponent<LobbyPlayer>())
                        continue;
                    SpawnGameObject(identity.gameObject);
                }
            }
        }

        protected virtual void Initialize()
        {
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            m_reliableChannel = config.AddChannel(QosType.Reliable);
            m_unreliableChannel = config.AddChannel(QosType.Unreliable);
            m_unreliableStateUpdateChannel = config.AddChannel(QosType.StateUpdate);

            m_hostTopology = new HostTopology(config, 10);
        }

        protected virtual void ReceiveEvents()
        {
            int connectionID, channelID, receivedBytes;
            byte[] buffer = new byte[1024];
            byte error;
            for (int i = 0; i < m_MaxNumberOfEventsPerFrame; i++)
            {
                NetworkEventType eventType = NetworkTransport.ReceiveFromHost
                    (
                        m_hostID, out connectionID,
                        out channelID, buffer, buffer.Length, out receivedBytes,
                        out error
                    );

                NetworkError networkError = (NetworkError)error;
                if (networkError != NetworkError.Ok)
                {
                    Debug.LogError($"Fehler beim Empfangen der Daten! ({networkError})", this);
                    return;
                }

                switch (eventType)
                {
                    case NetworkEventType.DataEvent:
                        m_onDataReceived?.Invoke(connectionID, buffer, receivedBytes);
                        break;
                    case NetworkEventType.ConnectEvent:
                        if (connectionID == m_clientID)
                            break;

                        m_onClientConnected?.Invoke(connectionID);
                        break;
                    case NetworkEventType.DisconnectEvent:
                        m_onClientDisconnected?.Invoke(connectionID);
                        break;
                    case NetworkEventType.Nothing:
                        return;
                    case NetworkEventType.BroadcastEvent:
                        break;
                    default:
                        Debug.LogError($"Event exisitiert nicht! {eventType}", this);
                        break;
                }
            }
        }

        protected virtual void SpawnPlayerPrefab(int _id)
        {
            m_PlayerPrefab.tag = "Player";
            //NetworkIdentity go = Instantiate(m_PlayerPrefab, Vector3.zero, Quaternion.identity);
            StartPosition(_id);
        }

        void StartPosition(int _id)
        {
            NetworkIdentity go;
            Renderer rend;
            // Check in which Slot ever Player was in the Lobby and at which Start Position he is allowed to spawn
            switch (SlotPosition.SlotID)
            {
                
                case 0:
                    Debug.Log("Player One");
                    // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                    go = Instantiate(m_PlayerPrefab, m_StartPoint[0].position, Quaternion.identity);

                    // change Color of Player
                    rend = m_PlayerPrefab.GetComponent<Renderer>();
                    rend.material.color = Color.blue;
                    SpawnGameObjectAsLocalPlayer(go.gameObject, _id);
                    break;
                case 1:
                    Debug.Log("Player Two");
                    // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                    go = Instantiate(m_PlayerPrefab, m_StartPoint[1].position, Quaternion.identity);

                    // change Color of Player
                    rend = m_PlayerPrefab.GetComponent<Renderer>();
                    rend.material.color = Color.red;
                    
                    SpawnGameObjectAsLocalPlayer(go.gameObject, _id);
                    break;
                case 2:
                    Debug.Log("Player Three");
                    // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                    go = Instantiate(m_PlayerPrefab, m_StartPoint[2].position, Quaternion.identity);

                    // change Color of Player
                    rend = m_PlayerPrefab.GetComponent<Renderer>();
                    rend.material.color = Color.green;

                    SpawnGameObjectAsLocalPlayer(go.gameObject, _id);
                    break;
                case 3:
                    Debug.Log("Player Four");
                    // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                    go = Instantiate(m_PlayerPrefab, m_StartPoint[3].position, Quaternion.identity);


                    // change Color of Player
                    rend = m_PlayerPrefab.GetComponent<Renderer>();
                    rend.material.color = Color.magenta;

                    SpawnGameObjectAsLocalPlayer(go.gameObject, _id);
                    break;
                case 4:
                    Debug.Log("Player Five");
                    // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                    go = Instantiate(m_PlayerPrefab, m_StartPoint[4].position, Quaternion.identity);


                    // change Color of Player
                    rend = m_PlayerPrefab.GetComponent<Renderer>();
                    rend.material.color = Color.gray;

                    SpawnGameObjectAsLocalPlayer(go.gameObject, _id);
                    break;
            }

            
        }

        protected virtual void AddToAllClients(int _id)
        {
            if (m_allClients.Contains(_id))
            {
                Debug.LogException(new System.InvalidOperationException("ID bereits hinzugefügt! " + _id));
                return;
            }

            m_allClients.Add(_id);
        }

        protected virtual void NetworkManager_m_onDataReceived(int _sender, byte[] _data, int _receivedBytes)
        {
            Debug.Log("Received message from " + (IsServer ? "Server" : "Client"));
            AMessageBase message = AMessageBase.ConstructMessage(_sender, _data,
                _receivedBytes);
            message?.Use();
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete

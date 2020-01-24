using Network.Messages.Lobby;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Delegates;

namespace Network.Lobby
{
    public class LobbyManager : NetworkManager
    {
        public int CurrentPlayerCount
        {
            get
            {
                return m_slots.Count;
            }
        }

        public int MaxPlayerCount
        {
            get
            {
                return m_maxPlayer;
            }
        }

        public LobbyPlayer m_LobbyPlayerPrefab;

        public string m_LobbyScene;
        public string m_GameScene;

        [SerializeField]
        protected int m_minPlayer;
        [SerializeField]
        protected int m_maxPlayer;

        protected List<LobbyPlayer> m_slots = new List<LobbyPlayer>();

        protected event AllPlayersReadyDelegate m_onAllPlayersReady;
        private bool m_wasReady = false;

        protected override bool StartHost()
        {
            bool b = base.StartHost();

            if (b)
            {
                SceneManager.LoadScene(m_LobbyScene);
            }
            return b;
        }

        protected override bool StartClient()
        {
            bool b = base.StartClient();

            if (b)
            {
                StartCoroutine(AsyncWait());
            }

            return b;
        }

        private IEnumerator AsyncWait()
        {
            yield return new WaitForSeconds(1.0f);

            LobbyRequestJoinMessage message = new LobbyRequestJoinMessage();
            LobbyManager.Instance.SendMessageToServer(message);

        }

        protected override void Start()
        {
            base.Start();
            m_onClientConnected -= SpawnPlayerPrefab;
            m_onClientConnected += PlayerJoinedLobby;

            m_onAllPlayersReady 
                += ((int _i, List<LobbyPlayer> _l) => SceneManager.LoadScene(m_GameScene));
        }

        public void PlayerJoinedLobby(int _id)
        {
            LobbyPlayer lobbyPlayer = Instantiate(m_LobbyPlayerPrefab);
            lobbyPlayer.m_SlotID = m_slots.Count;
            lobbyPlayer.PlayerName = "";
            m_slots.Add(lobbyPlayer);

            SpawnGameObjectAsLocalPlayer(lobbyPlayer.gameObject, _id);
        }

        public bool ReadyCheck()
        {
            if (m_wasReady)
                return true;
            int readyPlayers = 0;

            foreach (LobbyPlayer player in m_slots)
            {
                if (!player.IsReady)
                    return false;
                readyPlayers++;
            }

            if (readyPlayers >= m_minPlayer)
            {
                m_wasReady = true;
                m_onAllPlayersReady?.Invoke(m_slots.Count, m_slots);
                return true;
            }
            return false;
        }
    }
}

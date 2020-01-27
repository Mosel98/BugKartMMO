using Network.Messages.Lobby;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Network.Lobby
{
    public class LobbyPlayer : NetworkBehaviour
    {
        public static LobbyPlayer LocalLobbyPlayer { get; private set; }

        [SyncVar("PlayerNameChanged")]
        public string PlayerName
        {
            get
            {
                return m_playerName;
            }
            set
            {
                m_playerName = value;
                LobbyUIManager.Instance?.DisplayName(m_playerName, m_SlotID);
            }
        }
        [SyncVar]
        public int m_SlotID;
        [SyncVar("ReadyChanged")]
        public bool IsReady
        {
            get
            {
                return m_isReady;
            }
            set
            {
                m_isReady = value;
                ReadyChanged(m_isReady);
                ((LobbyManager)LobbyManager.Instance).ReadyCheck();
            }
        }

        protected bool m_isReady;

        protected string m_playerName;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        protected override void Start()
        {
            base.Start();

            if (IsLocalPlayer)
            {
                LocalLobbyPlayer = this;
                PlayerName = "Player " + NetId.NetID;
                LobbySendPlayerInformationMessage message = new LobbySendPlayerInformationMessage();
                message.PlayerName = PlayerName;
                message.IsReady = false;
                message.LobbyPlayer = this;
                LobbyManager.Instance.SendMessageToServer(message);

                SlotPosition.SlotID = m_SlotID;
            }
        }

        private void PlayerNameChanged(string _name)
        {
            LobbyUIManager.Instance?.DisplayName(_name, m_SlotID);
        }

        private void ReadyChanged(bool _isReady)
        {
            LobbyUIManager.Instance?.DisplayReadyState(_isReady, m_SlotID);
        }
    }
}

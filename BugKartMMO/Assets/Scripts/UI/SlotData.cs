using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Network.Lobby;
using Network.Messages.Lobby;

namespace UI
{
    public class SlotData : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_nameText;
        [SerializeField]
        private TextMeshProUGUI m_readyText;
        [SerializeField]
        private Button m_readyButton;

        private void Start()
        {
            m_readyButton.onClick.AddListener(ChangeReadyState);
        }

        public void SetName(string _name)
        {
            m_nameText.text = _name;
        }

        public void SetReadyState(bool _isReady)
        {
            m_readyText.text = _isReady ? "Breit" : "Schmal";
        }

        public void ToggleButton(bool _isEnabled)
        {
            m_readyButton.interactable = _isEnabled;
        }

        public void ChangeReadyState()
        {
            LobbyPlayer.LocalLobbyPlayer.IsReady
                = !LobbyPlayer.LocalLobbyPlayer.IsReady;

            LobbySendPlayerInformationMessage message = new LobbySendPlayerInformationMessage();
            message.PlayerName = LobbyPlayer.LocalLobbyPlayer.PlayerName;
            message.IsReady = LobbyPlayer.LocalLobbyPlayer.IsReady;
            message.LobbyPlayer = LobbyPlayer.LocalLobbyPlayer;
            LobbyManager.Instance.SendMessageToServer(message);
        }
    }
}

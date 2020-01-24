using Network.Lobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class LobbyUIManager : MonoBehaviour
    {
        public static LobbyUIManager Instance { get; private set; }

        public SlotData m_SlotPrefab;
        public RectTransform m_SlotPanel;

        private List<SlotData> m_slots = new List<SlotData>();

        private void Awake()
        {
            Instance = this;
        }

        private IEnumerator Start()
        {
            for (int i = 0; i < ((LobbyManager)LobbyManager.Instance).MaxPlayerCount; i++)
            {
                SlotData slot = Instantiate(m_SlotPrefab);
                m_slots.Add(slot);
                slot.transform.SetParent(m_SlotPanel.transform, false);
            }

            LobbyPlayer[] lobbyPlayers = FindObjectsOfType<LobbyPlayer>();
            foreach (LobbyPlayer player in lobbyPlayers)
            {
                DisplayName(player.PlayerName, player.m_SlotID);
                DisplayReadyState(player.IsReady, player.m_SlotID);
                
            }

            yield return new WaitWhile(() => LobbyPlayer.LocalLobbyPlayer is null);

            m_slots[LobbyPlayer.LocalLobbyPlayer.m_SlotID].ToggleButton(true);
        }

        public void DisplayName(string _name, int _slotID)
        {
            m_slots[_slotID].SetName(_name);
        }

        public void DisplayReadyState(bool _state, int _slotID)
        {
            m_slots[_slotID].SetReadyState(_state);
        }
    }
}
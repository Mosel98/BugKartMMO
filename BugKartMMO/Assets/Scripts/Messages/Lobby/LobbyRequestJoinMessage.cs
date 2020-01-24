using Network.IO;
using Network.Lobby;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Network.Messages.Lobby
{

    public class LobbyRequestJoinMessage : AMessageBase
    {
        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_senderID, _data, _receivedBytes);
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.LOBBY_REQUEST_JOIN);
                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            LobbyManager lobbyManager = NetworkManager.Instance as LobbyManager;
            if (lobbyManager is object)
            {
                if (lobbyManager.CurrentPlayerCount < lobbyManager.MaxPlayerCount)
                {
                    LobbyAcceptJoinMessage lobbyAcceptJoinMessage = new LobbyAcceptJoinMessage();
                    lobbyManager.SendMessageToClient(SenderID, lobbyAcceptJoinMessage);
                }
                else
                {
                    // TODO: Disconnect client
                }
            }
        }
    }
}
using Network.IO;
using Network.Lobby;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Network.Messages.Lobby
{
    public class LobbySendPlayerInformationMessage : AMessageBase
    {
        public string PlayerName { get; set; }
        public bool IsReady { get; set; }
        public LobbyPlayer LobbyPlayer { get; set; }

        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_senderID, _data, _receivedBytes);
            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    nr.ReadInt16();
                    PlayerName = nr.ReadString();
                    IsReady = nr.ReadBoolean();
                    LobbyPlayer = nr.ReadNetworkIdentity().GetComponent<LobbyPlayer>();
                }
            }
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.LOBBY_PLAYER_INFO);
                    nw.Write(PlayerName);
                    nw.Write(IsReady);
                    nw.Write(LobbyPlayer.GetComponent<NetworkIdentity>());
                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            LobbyPlayer.PlayerName = PlayerName;
            LobbyPlayer.IsReady = IsReady;
            LobbyPlayer.SetIsDirty();
        }
    }
}
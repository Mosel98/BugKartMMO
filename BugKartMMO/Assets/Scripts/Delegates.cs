using Network.Lobby;
using System.Collections.Generic;

namespace Delegates
{
    public delegate void ClientConnectedDelegate(int _id);
    public delegate void ClientDisconnectedDelegate(int _id);
    public delegate void DataReceivedDelegate(int _sender, byte[] _data, 
                                      int _receivedBytes);
    public delegate void AllPlayersReadyDelegate(int _playerCount, List<LobbyPlayer> _slots);
}
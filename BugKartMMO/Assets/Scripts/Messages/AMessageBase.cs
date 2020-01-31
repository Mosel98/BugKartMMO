using Network.IO;
using Network.Messages.Lobby;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Network.Messages
{
    public abstract class AMessageBase
    {
        public enum EMessageType
        {
            NONE,
            LOG,
            SPAWN,
            UPDATE_POSITION,
            NETWORK_BEHAVIOUR_UPDATE,
            DESTROY_OBJECT,
            SHOOT_ITEM,
            SCENE_LOADED,
            SCENE_READY,
            SWITCH_SCENE,
            HONK_MESSAGE,

            COUNTDOWN, // Frank
            PLAYER_IN_GAME, // Frank
            ACCELERATION_CHANGE, // Frank
            FINISH_LINE, // Frank
            COLLISION_CHECK, // Mario
            UPDATE_VARIABLE, // Mario
            USE_ITEM, // Mario

            LOBBY_MESSAGES = 10_000,
            LOBBY_REQUEST_JOIN,
            LOBBY_ACCEPT_JOIN,
            LOBBY_PLAYER_INFO,
        }

        /*
         *      2 Byte => Message Type 
         */

        public int SenderID { get; set; }

        public abstract byte[] Serialize(out int _bytes);
        public virtual void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            SenderID = _senderID;
        }
        public abstract void Use();

        public static AMessageBase ConstructMessage(int _sender, byte[] _data, int _receivedBytes)
        {
            EMessageType messageType = EMessageType.NONE;
            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    ushort type = nr.ReadUInt16();
                    messageType = (EMessageType)type;
                }
            }
            Debug.Log($"Received {messageType} from {_sender}");
            AMessageBase message = null;
            switch (messageType)
            {
                case EMessageType.NONE:
                    break;
                case EMessageType.LOG:
                    message = new LogMessage();
                    break;
                case EMessageType.SPAWN:
                    message = new SpawnMessage();
                    break;
                case EMessageType.UPDATE_POSITION:
                    message = new UpdatePositionMessage();
                    break;
                case EMessageType.NETWORK_BEHAVIOUR_UPDATE:
                    message = new UpdateNetworkBehaviourMessage();
                    break;
                case EMessageType.DESTROY_OBJECT:
                    message = new DestroyMessage();
                    break;
                case EMessageType.SCENE_LOADED:
                    message = new SceneLoadedMessage();
                    break;
                case EMessageType.SCENE_READY:
                    message = new SceneReadyMessage();
                    break;
                case EMessageType.SWITCH_SCENE:
                    message = new SwitchSceneMessage();
                    break;

                // Frank
                case EMessageType.COUNTDOWN:
                    message = new CountdownMessage();
                    break;
                // Frank
                case EMessageType.PLAYER_IN_GAME:
                    message = new PlayerInGameMessage();
                    break;
                // Frank
                case EMessageType.ACCELERATION_CHANGE:
                    message = new AccelerationMessage();
                    break;
                // Frank
                case EMessageType.FINISH_LINE:
                    message = new FinishLineMessage();
                    break;

                // Mario
                case EMessageType.COLLISION_CHECK:
                    message = new CollisionCheckMessage();
                    break;
                // Mario
                case EMessageType.UPDATE_VARIABLE:
                    message = new UpdateVariableMessage();
                    break;
                // Mario
                case EMessageType.USE_ITEM:
                    message = new UseItemMessage();
                    break;

                case EMessageType.LOBBY_ACCEPT_JOIN:
                    message = new LobbyAcceptJoinMessage();
                    break;
                case EMessageType.LOBBY_REQUEST_JOIN:
                    message = new LobbyRequestJoinMessage();
                    break;
                case EMessageType.LOBBY_PLAYER_INFO:
                    message = new LobbySendPlayerInformationMessage();
                    break;
                case EMessageType.HONK_MESSAGE:
                    message = new HonkMessage();
                    break;
                default:
                    break;
            }
            message?.Deserialize(_sender, _data, _receivedBytes);

            return message;
        }
    }
}
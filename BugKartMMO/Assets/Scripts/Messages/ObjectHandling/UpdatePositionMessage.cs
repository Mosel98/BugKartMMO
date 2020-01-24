using Network.IO;
using System.IO;
using UnityEngine;

namespace Network.Messages
{
    public class UpdatePositionMessage : AMessageBase
    {
        public Quaternion Rotation { get; set; }
        public Vector3 NewPosition { get; set; }
        public GameObject GameObject { get; set; }
        public float Timestamp { get; set; }

        public UpdatePositionMessage()
        {

        }

        public UpdatePositionMessage(GameObject _go)
        {
            GameObject = _go;
            Rotation = _go.transform.rotation;
            NewPosition = _go.transform.position;
            Timestamp = Time.realtimeSinceStartup;
        }

        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_senderID, _data, _receivedBytes);
            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    nr.ReadInt16();
                    NewPosition = nr.ReadVector3();
                    Rotation = nr.ReadQuaternion();
                    GameObject = nr.ReadGameObject();
                    Timestamp = nr.ReadSingle();
                }
            }
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.UPDATE_POSITION);
                    nw.Write(NewPosition);
                    nw.Write(Rotation);
                    nw.Write(GameObject);
                    nw.Write(Timestamp);

                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            Debug.Log("Received UpdatePositionMessage for " + GameObject, GameObject);
            if (GameObject is object)
            {
                NetworkTransform networkTransform = GameObject.GetComponent<NetworkTransform>();
                if (networkTransform is object)
                {
                    networkTransform.ReceivedNewPosition(NewPosition, Timestamp);
                    networkTransform.ReceivedNewRotation(Rotation, Timestamp);
                }
                else
                {
                    Debug.LogWarning("NetworkTransform was not found! " + GameObject, GameObject);
                }
            }
            else
            {
                Debug.LogWarning("Object was not found!");
            }
        }
    }
}
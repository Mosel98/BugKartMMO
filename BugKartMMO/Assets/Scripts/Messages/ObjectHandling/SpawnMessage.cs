using Network.IO;
using System.IO;
using UnityEngine;

namespace Network.Messages
{
    public class SpawnMessage : AMessageBase
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public GameObject Prefab { get; set; }
        public Transform Parent { get; set; }
        public uint NetID { get; set; }
        public bool IsLocalPlayer { get; set; }
        public byte[][] NetworkBehaviours { get; set; }

        public SpawnMessage()
        {

        }

        public SpawnMessage(GameObject _go, bool _localPlayerAuthority = false)
        {
            Position = _go.transform.position;
            Rotation = _go.transform.rotation;
            Scale = _go.transform.localScale;
            Prefab = NetworkManager.Instance.GetPrefabIdentity(
                _go.GetComponent<NetworkIdentity>().PrefabID).gameObject;
            Parent = _go.transform.parent;
            NetID = _go.GetComponent<NetworkIdentity>().NetID;
            IsLocalPlayer = _localPlayerAuthority;

            NetworkBehaviour[] networkBehaviours = _go.GetComponents<NetworkBehaviour>();
            NetworkBehaviours = new byte[networkBehaviours.Length][];

            for (int i = 0; i < networkBehaviours.Length; i++)
            {
                NetworkBehaviours[i] = networkBehaviours[i].SerializedBytes;
            }

        }

        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_senderID, _data, _receivedBytes);
            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    nr.ReadUInt16();
                    Position = nr.ReadVector3();
                    Rotation = nr.ReadQuaternion();
                    Scale = nr.ReadVector3();
                    Prefab = nr.ReadPrefab();
                    Parent = nr.ReadTransform();
                    NetID = nr.ReadUInt32();
                    IsLocalPlayer = nr.ReadBoolean();
                    NetworkBehaviours = new byte[nr.ReadInt32()][];

                    int byteLength;
                    for (int i = 0; i < NetworkBehaviours.Length; i++)
                    {
                        byteLength = nr.ReadInt32();
                        NetworkBehaviours[i] = nr.ReadBytes(byteLength);
                    }
                }
            }
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.SPAWN);
                    nw.Write(Position);
                    nw.Write(Rotation);
                    nw.Write(Scale);
                    nw.Write(Prefab);
                    nw.Write(Parent);
                    nw.Write(NetID);
                    nw.Write(IsLocalPlayer);
                    nw.Write(NetworkBehaviours.Length);
                    foreach (byte[] bytes in NetworkBehaviours)
                    {
                        nw.Write(bytes.Length);
                        nw.Write(bytes);
                    }
                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            if (NetworkManager.Instance.GetIdentity(NetID) is object)
                return;

            GameObject go = NetworkManager.Instantiate(Prefab, Position,
                Rotation, Parent);
            NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
            identity.Init(false, NetID, identity.PrefabID, IsLocalPlayer);
            NetworkManager.Instance.AddToIdentities(identity);

            NetworkBehaviour[] networkBehaviours = go.GetComponents<NetworkBehaviour>();
            for (int i = 0; i < networkBehaviours.Length; i++)
            {
                networkBehaviours[i].Deserialize(NetworkBehaviours[i]);
            }
        }
    }
}

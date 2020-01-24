using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
using Type = System.Type;
using System.IO;
using Network.Messages;
using Network.IO;

namespace Network
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkBehaviour : MonoBehaviour
    {
        public NetworkIdentity NetId
        {
            get
            {
                if (m_netID is null)
                {
                    m_netID = GetComponent<NetworkIdentity>();
                }
                return m_netID;
            }
        }
        public bool IsServer
        {
            get
            {
                return NetId.IsServer;
            }
        }
        public bool IsClient
        {
            get
            {
                return NetId.IsClient;
            }
        }

        public bool IsLocalPlayer
        {
            get
            {
                return NetId.IsLocalPlayer;
            }
        }

        public uint ComponentID { get; set; }

        public byte[] SerializedBytes
        {
            get
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (NetworkWriter nr = new NetworkWriter(ms))
                    {
                        if (Serialize(nr))
                        {
                            byte[] bytes = new byte[ms.Position];
                            byte[] allBytes = ms.ToArray();
                            for (int i = 0; i < bytes.Length; i++)
                            {
                                bytes[i] = allBytes[i];
                            }
                            return bytes;
                        }
                        return null;
                    }
                }
            }
        }

        private bool m_isDirty;
        private NetworkIdentity m_netID;

        public void SetIsDirty()
        {
            m_isDirty = true;
        }

        protected virtual void Start()
        {
            NetId.GotNewComponent(this);
        }

        protected virtual void Update()
        {
            if (m_isDirty && IsServer)
            {
                UpdateNetworkBehaviourMessage message = new UpdateNetworkBehaviourMessage();
                message.NetID = NetId;
                message.ComponentID = ComponentID;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (NetworkWriter nw = new NetworkWriter(ms))
                    {
                        if (Serialize(nw))
                        {
                            // TODO Cut short
                            message.Bytes = ms.ToArray();
                        }
                    }
                }
                NetworkManager.Instance.SendMessageToClients(message);
                m_isDirty = false;
            }
        }
        #region ---Serialization---
        private bool Serialize(NetworkWriter _writer)
        {
            Type type = GetType();

            FieldInfo[] allFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            PropertyInfo[] allProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            IEnumerable<FieldInfo> fields = allFields
                .Where(o => o.GetCustomAttributes(typeof(SyncVar)).Count() > 0)
                .OrderBy(o => o.Name);
            IEnumerable<PropertyInfo> properties = allProperties
                .Where(o => o.GetCustomAttributes(typeof(SyncVar)).Count() > 0)
                .OrderBy(o => o.Name);

            Type fieldType;

            foreach (FieldInfo info in fields)
            {
                fieldType = info.FieldType;

                if (fieldType.IsEnum)
                {
                    _writer.Write((int)info.GetValue(this));
                    continue;
                }

                string stringType = fieldType.Name;
                object tmpValue = info.GetValue(this);
                switch (stringType)
                {
                    case "Boolean":
                        _writer.Write((bool)tmpValue);
                        break;
                    case "Byte":
                        _writer.Write((byte)tmpValue);
                        break;
                    case "SByte":
                        _writer.Write((sbyte)tmpValue);
                        break;
                    case "Int16":
                        _writer.Write((short)tmpValue);
                        break;
                    case "Int32":
                        _writer.Write((int)tmpValue);
                        break;
                    case "Int64":
                        _writer.Write((long)tmpValue);
                        break;
                    case "UInt16":
                        _writer.Write((ushort)tmpValue);
                        break;
                    case "UInt32":
                        _writer.Write((uint)tmpValue);
                        break;
                    case "UInt64":
                        _writer.Write((ulong)tmpValue);
                        break;
                    case "Single":
                        _writer.Write((float)tmpValue);
                        break;
                    case "Double":
                        _writer.Write((double)tmpValue);
                        break;
                    case "Decimal":
                        _writer.Write((decimal)tmpValue);
                        break;
                    case "Char":
                        _writer.Write((char)tmpValue);
                        break;
                    case "String":
                        _writer.Write((string)tmpValue);
                        break;
                    case "Byte[]":
                        _writer.Write((byte[])tmpValue, 0, ((byte[])tmpValue).Length);
                        break;
                    case "Vector2":
                        _writer.Write((Vector2)tmpValue);
                        break;
                    case "Vector3":
                        _writer.Write((Vector3)tmpValue);
                        break;
                    case "Vector4":
                        _writer.Write((Vector4)tmpValue);
                        break;
                    case "GameObject":
                        _writer.Write((GameObject)tmpValue);
                        break;
                    case "Transform":
                        _writer.Write((Transform)tmpValue);
                        break;
                    case "Color":
                        _writer.Write((Color)tmpValue);
                        break;
                    case "Color32":
                        _writer.Write((Color32)tmpValue);
                        break;
                    default:
                        Debug.LogWarning("Could not serialize field! " + info.Name + "(" + info.FieldType.Name + ")", this);
                        break;
                }
            }

            foreach (PropertyInfo info in properties)
            {
                fieldType = info.PropertyType;

                if (fieldType.IsEnum)
                {
                    _writer.Write((int)info.GetValue(this));
                    continue;
                }

                string stringType = fieldType.Name;
                object tmpValue = info.GetValue(this);
                switch (stringType)
                {
                    case "Boolean":
                        _writer.Write((bool)tmpValue);
                        break;
                    case "Byte":
                        _writer.Write((byte)tmpValue);
                        break;
                    case "SByte":
                        _writer.Write((sbyte)tmpValue);
                        break;
                    case "Int16":
                        _writer.Write((short)tmpValue);
                        break;
                    case "Int32":
                        _writer.Write((int)tmpValue);
                        break;
                    case "Int64":
                        _writer.Write((long)tmpValue);
                        break;
                    case "UInt16":
                        _writer.Write((ushort)tmpValue);
                        break;
                    case "UInt32":
                        _writer.Write((uint)tmpValue);
                        break;
                    case "UInt64":
                        _writer.Write((ulong)tmpValue);
                        break;
                    case "Single":
                        _writer.Write((float)tmpValue);
                        break;
                    case "Double":
                        _writer.Write((double)tmpValue);
                        break;
                    case "Decimal":
                        _writer.Write((decimal)tmpValue);
                        break;
                    case "Char":
                        _writer.Write((char)tmpValue);
                        break;
                    case "String":
                        _writer.Write((string)tmpValue);
                        break;
                    case "Byte[]":
                        _writer.Write((byte[])tmpValue, 0, ((byte[])tmpValue).Length);
                        break;
                    case "Vector2":
                        _writer.Write((Vector2)tmpValue);
                        break;
                    case "Vector3":
                        _writer.Write((Vector3)tmpValue);
                        break;
                    case "Vector4":
                        _writer.Write((Vector4)tmpValue);
                        break;
                    case "GameObject":
                        _writer.Write((GameObject)tmpValue);
                        break;
                    case "Transform":
                        _writer.Write((Transform)tmpValue);
                        break;
                    case "Color":
                        _writer.Write((Color)tmpValue);
                        break;
                    case "Color32":
                        _writer.Write((Color32)tmpValue);
                        break;
                    default:
                        Debug.LogWarning("Could not serialize property! " + info.Name + "(" + info.PropertyType.Name + ")", this);
                        break;
                }
            }

            return true;
        }

        public bool Deserialize(byte[] _bytes)
        {
            using (MemoryStream ms = new MemoryStream(_bytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    Type type = GetType();

                    FieldInfo[] allFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    PropertyInfo[] allProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    IEnumerable<FieldInfo> fields = allFields
                        .Where(o => o.GetCustomAttributes(typeof(SyncVar)).Count() > 0)
                        .OrderBy(o => o.Name);
                    IEnumerable<PropertyInfo> properties = allProperties
                        .Where(o => o.GetCustomAttributes(typeof(SyncVar)).Count() > 0)
                        .OrderBy(o => o.Name);

                    Type fieldType;

                    foreach (FieldInfo info in fields)
                    {
                        fieldType = info.FieldType;

                        string typeString = fieldType.Name;
                        object tmpValue = null;

                        bool switchOnType = true;

                        if (fieldType.IsEnum)
                        {
                            tmpValue = nr.ReadInt32();
                            switchOnType = false;
                        }
                        if (switchOnType)
                        {
                            switch (typeString)
                            {
                                case "Boolean":
                                    tmpValue = nr.ReadBoolean();
                                    break;
                                case "Byte":
                                    tmpValue = nr.ReadByte();
                                    break;
                                case "SByte":
                                    tmpValue = nr.ReadSByte();
                                    break;
                                case "Int16":
                                    tmpValue = nr.ReadInt16();
                                    break;
                                case "Int32":
                                    tmpValue = nr.ReadInt32();
                                    break;
                                case "Int64":
                                    tmpValue = nr.ReadInt64();
                                    break;
                                case "UInt16":
                                    tmpValue = nr.ReadUInt16();
                                    break;
                                case "UInt32":
                                    tmpValue = nr.ReadUInt32();
                                    break;
                                case "UInt64":
                                    tmpValue = nr.ReadUInt64();
                                    break;
                                case "Single":
                                    tmpValue = nr.ReadSingle();
                                    break;
                                case "Double":
                                    tmpValue = nr.ReadDouble();
                                    break;
                                case "Decimal":
                                    tmpValue = nr.ReadDecimal();
                                    break;
                                case "Char":
                                    tmpValue = nr.ReadChar();
                                    break;
                                case "String":
                                    tmpValue = nr.ReadString();
                                    break;
                                case "Byte[]":
                                    int length = nr.ReadInt32();
                                    tmpValue = nr.ReadBytes(length);
                                    break;
                                case "Vector2":
                                    tmpValue = nr.ReadVector2();
                                    break;
                                case "Vector3":
                                    tmpValue = nr.ReadVector3();
                                    break;
                                case "Vector4":
                                    tmpValue = nr.ReadVector4();
                                    break;
                                case "GameObject":
                                    tmpValue = nr.ReadGameObject();
                                    break;
                                case "Transform":
                                    tmpValue = nr.ReadTransform();
                                    break;
                                case "Color":
                                    tmpValue = nr.ReadColor();
                                    break;
                                case "Color32":
                                    tmpValue = nr.ReadColor32();
                                    break;
                                default:
                                    Debug.LogWarning("Could not serialize field! " + info.Name + "(" + info.FieldType.Name + ")", this);
                                    break;
                            }
                        }
                        info.SetValue(this, tmpValue);
                        SyncVar syncVar = info.GetCustomAttribute<SyncVar>();
                        if (syncVar is object  && syncVar.Hook is object)
                        {
                            SendMessage(syncVar.Hook, tmpValue, SendMessageOptions.RequireReceiver);
                        }
                    }

                    foreach (PropertyInfo info in properties)
                    {
                        fieldType = info.PropertyType;

                        string typeString = fieldType.Name;
                        object tmpValue = null;

                        bool switchOnType = true;

                        if (fieldType.IsEnum)
                        {
                            tmpValue = nr.ReadInt32();
                            switchOnType = false;
                        }
                        if (switchOnType)
                        {
                            switch (typeString)
                            {
                                case "Boolean":
                                    tmpValue = nr.ReadBoolean();
                                    break;
                                case "Byte":
                                    tmpValue = nr.ReadByte();
                                    break;
                                case "SByte":
                                    tmpValue = nr.ReadSByte();
                                    break;
                                case "Int16":
                                    tmpValue = nr.ReadInt16();
                                    break;
                                case "Int32":
                                    tmpValue = nr.ReadInt32();
                                    break;
                                case "Int64":
                                    tmpValue = nr.ReadInt64();
                                    break;
                                case "UInt16":
                                    tmpValue = nr.ReadUInt16();
                                    break;
                                case "UInt32":
                                    tmpValue = nr.ReadUInt32();
                                    break;
                                case "UInt64":
                                    tmpValue = nr.ReadUInt64();
                                    break;
                                case "Single":
                                    tmpValue = nr.ReadSingle();
                                    break;
                                case "Double":
                                    tmpValue = nr.ReadDouble();
                                    break;
                                case "Decimal":
                                    tmpValue = nr.ReadDecimal();
                                    break;
                                case "Char":
                                    tmpValue = nr.ReadChar();
                                    break;
                                case "String":
                                    tmpValue = nr.ReadString();
                                    break;
                                case "Byte[]":
                                    int length = nr.ReadInt32();
                                    tmpValue = nr.ReadBytes(length);
                                    break;
                                case "Vector2":
                                    tmpValue = nr.ReadVector2();
                                    break;
                                case "Vector3":
                                    tmpValue = nr.ReadVector3();
                                    break;
                                case "Vector4":
                                    tmpValue = nr.ReadVector4();
                                    break;
                                case "GameObject":
                                    tmpValue = nr.ReadGameObject();
                                    break;
                                case "Transform":
                                    tmpValue = nr.ReadTransform();
                                    break;
                                case "Color":
                                    tmpValue = nr.ReadColor();
                                    break;
                                case "Color32":
                                    tmpValue = nr.ReadColor32();
                                    break;
                                default:
                                    Debug.LogWarning("Could not serialize field! " + info.Name + "(" + info.PropertyType.Name + ")", this);
                                    break;
                            }
                        }
                        info.SetValue(this, tmpValue);
                        SyncVar syncVar = info.GetCustomAttribute<SyncVar>();
                        if (syncVar is object && syncVar.Hook is object)
                        {
                            SendMessage(syncVar.Hook, tmpValue, SendMessageOptions.RequireReceiver);
                        }
                    }
                }

                return true;
            }
        }
        #endregion
    }
}

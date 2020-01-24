using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    [DisallowMultipleComponent]
    public class NetworkIdentity : MonoBehaviour
    {
        public bool IsInitialized { get; private set; }

        public bool IsServer
        {
            get
            {
                return m_isServer;
            }
        }

        public bool IsClient
        {
            get
            {
                return !m_isServer;
            }
        }

        public bool IsLocalPlayer { get; private set; }

        public uint NetID { get; private set; }
        public int PrefabID
        {
            get
            {
                return m_prefabID;
            }
        }

        private bool m_isServer;
        [SerializeField]
        private int m_prefabID;

        private uint m_nextComponentID = 1;

        public void Init(bool _isServer, uint _id, int _prefabID, bool _isLocalPlayer)
        {
            m_isServer = _isServer;
            NetID = _id;
            m_prefabID = _prefabID;
            IsLocalPlayer = _isLocalPlayer;
            IsInitialized = true;
        }

        public void GotNewComponent(NetworkBehaviour _behaviour)
        {
            _behaviour.ComponentID = m_nextComponentID++;
        }

        private void OnDestroy()
        {
            NetworkManager.Instance.RemoveIdentities(this);
        }
    }
}
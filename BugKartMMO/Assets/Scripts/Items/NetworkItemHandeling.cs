using Network;
using Network.Messages;
using UnityEngine;

// Mario
public class NetworkItemHandeling : MonoBehaviour
{
    public int m_SyncInterval;

    [SerializeField]
    private GameObject m_greenShell;
    [SerializeField]
    private GameObject m_redShell;
    [SerializeField]
    private GameObject m_köttel;

    private GameObject m_player;
    private GameObject m_itemBox;

    private float m_speed { get; set; }
    private float m_accel { get; set; }

    [SerializeField]
    private float m_boostTime = 1.0f;

    [SerializeField]
    private float m_itemBoxTime = 5.0f;

    private float m_timer = 0.0f;

    private float m_tmpAccel;

    private bool m_boostSpeed = false;
    private bool m_respItemBox = false;

    #region ----- UseItem -----
    public void UseItem(GameObject _go, float _item)
    {
        m_player = _go;

        switch ((EItems)_item)
        {
            case EItems.COIN:
                ConstantSpeed();
                break;
            case EItems.MUSHROOM:
                BoostSpeed();
                break;
            case EItems.KÖTTEL:
                SpawnItem(m_köttel, -5.0f);
                break;
            case EItems.GREENSHELL:
                SpawnItem(m_greenShell, 5.0f);
                break;
        }
    }

    private void SpawnItem(GameObject _item, float _spawnDist)
    {
        Vector3 spawnPos = m_player.transform.position + m_player.transform.forward * _spawnDist;

        GameObject tmp = Instantiate(_item, spawnPos, m_player.transform.rotation);

        if (tmp == m_greenShell)
        {
            tmp.GetComponent<Rigidbody>().AddForce(tmp.transform.forward * 100.0f);
        }
    }
    #endregion

    #region ----- ItemBox -----
    public void ItemBoxCheck(GameObject _go, GameObject _ib)
    {
        int item = Random.Range(0, 3);

        UpdateVariableMessage message = new UpdateVariableMessage();
        message.Player = _go;
        message.Item = item;

        Debug.Log("Send Updated Item To Client");

        if (NetworkManager.Instance.isHost)
        {
            message.Use();
        }
        else
        {
            NetworkManager.Instance.SendMessageToClients(message);
        }

        m_itemBox = _ib;
        m_itemBox.SetActive(false);
        m_respItemBox = true;
    }
    #endregion

    #region ----- ItemCollision -----
    public void CollisionCheck(GameObject _go, string _tag, float _speed, float _accel)
    {
        string tag = _tag;
        m_player = _go;
        m_speed = _speed;
        m_accel = _accel;

        switch (tag)
        {
            case "Coin":
                ConstantSpeed();
                break;
            case "Shell":
                StopPlayer();
                break;
            case "Boost":
                BoostSpeed();
                break;
        }
    }

    private void ConstantSpeed()
    {
        m_speed += 0.1f;

        UpdateVariable();
    }

    private void BoostSpeed()
    {
        m_tmpAccel = m_accel;
        m_accel += 1.5f;

        m_boostSpeed = true;

        UpdateVariable();
    }

    private void StopPlayer()
    {
        m_accel = 0.0f;

        UpdateVariable();
    }

    protected void Update()
    {
        if (m_boostSpeed)
        {
            m_timer += Time.deltaTime;

            if (m_timer >= m_boostTime)
            {
                m_accel = m_tmpAccel;

                m_timer = 0.0f;
                m_boostSpeed = false;

                UpdateVariable();
            }
        }

        if (m_respItemBox)
        {
            m_timer += Time.deltaTime;

            if (m_timer >= m_itemBoxTime)
            {
                m_itemBox.SetActive(true);

                m_timer = 0.0f;
                m_respItemBox = false;
            }
        }
    }
    #endregion

    private void UpdateVariable()
    {
        UpdateVariableMessage message = new UpdateVariableMessage();
        message.Player = m_player;
        message.Speed = m_speed;
        message.Accel = m_accel;

        NetworkManager.Instance.SendMessageToClients(message, UnityEngine.Networking.QosType.StateUpdate);
        Debug.Log("Send Updated Variable To Client");
    }
}



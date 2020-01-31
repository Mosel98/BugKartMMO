using Network;
using Network.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


// Frank
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [SyncVar]
    public static bool m_isInGame = false;

    private GameUIManager m_gameUIManager;

    [SerializeField]
    private Sprite m_imgEmpty;

    [SerializeField]
    private Sprite m_imgCoin;

    [SerializeField]
    private Sprite m_imgMush;

    [SerializeField]
    private Sprite m_imgKöttel;

    [SerializeField]
    private Sprite m_imgShell;

    [SyncVar]
    private float m_countdown = 5.0f;

    private static bool m_canStart = false;



    [SyncVar]
    private float m_speed = 1.0f;

    [SyncVar]
    public float m_Acceleration = 0.0f;

    [SyncVar]
    private Vector3 m_position;

    //[SyncVar]
    //public Quaternion m_Rotation;

    private Camera m_camera; // Position?! --> testen und festlegen!

    // vector shift of the camera based on player position
    // private Vector3 m_cameraPositionShift;

    [SyncVar]
    public int m_finishPlace;

    [SyncVar]
    public static bool m_FinishedRace;

    //[SyncVar]
    private EItems m_eItem = EItems.EMPTY;

    private Rigidbody m_rigidbody;

    // dictionary to see which key is pressed down
    public Dictionary<KeyCode, bool> m_KeysPressed = new Dictionary<KeyCode, bool>();

    // dictionary to see which player are spawned in the scene
    public Dictionary<int, bool> m_AllPlayersReady = new Dictionary<int, bool>();

    // dictionary to see which player have finished the race
    public Dictionary<int, bool> m_FinishedPlayers = new Dictionary<int, bool>();

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        // add player to dictionary
        m_AllPlayersReady.Add(SlotPosition.SlotID, false);

        m_FinishedPlayers.Add(SlotPosition.SlotID, false);

        // add keys for movement messages
        m_KeysPressed.Add(KeyCode.W, false);
        m_KeysPressed.Add(KeyCode.S, false);
        m_KeysPressed.Add(KeyCode.A, false);
        m_KeysPressed.Add(KeyCode.D, false);
        m_KeysPressed.Add(KeyCode.Space, false);

        m_camera = GetComponent<Camera>();

        //m_gameUIManager = GameObject.Find("GameManager").GetComponent<GameUIManager>();
        //m_gameUIManager.UpdateItemImage(EItems.EMPTY);

        // m_cameraPositionShift.Set(0.0f, 15.0f, -25.0f); // Verschiebung der Kamera
        // m_camera.transform.position = transform.position + m_cameraPositionShift;
    }

    protected override void Start()
    {
        base.Start();

        #region --- != localPlayer ---
        if (!IsLocalPlayer)
        {
            return;
        }
        #endregion

        if (IsLocalPlayer)
        {
            PlayerInGameMessage message = new PlayerInGameMessage();
            message.PlayerID = 0;
            message.PlayerController = this;
            message.SlotID = SlotPosition.SlotID;

            // Server controls if all player are in game scene
            NetworkManager.Instance.SendMessageToServer(message);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (IsServer) // && START_GAME)
        {
            StartCountdown();
        }


        #region --- Server ---
        if (IsServer)
        {
            #region --- W & S ---
            // keys pressed down (true)
            if (m_KeysPressed[KeyCode.W])
            {
                m_Acceleration += 0.5f * Time.deltaTime;
            }

            if (m_KeysPressed[KeyCode.S])
            {
                m_Acceleration -= 0.5f * Time.deltaTime;
            }
            #endregion

            #region --- Space ---
            // handbreak, faster decrease speed
            if (m_KeysPressed[KeyCode.Space])
            {
                m_Acceleration -= 1.0f * Time.deltaTime;
                // don't go backwards with handbreak!
                m_Acceleration = Mathf.Clamp(m_Acceleration, 0.0f, m_speed);
            }
            #endregion

            #region --- No Key ---
            // no move key pressed
            if (!(m_KeysPressed[KeyCode.W] ||
                m_KeysPressed[KeyCode.A] ||
                m_KeysPressed[KeyCode.S] ||
                m_KeysPressed[KeyCode.D]))
            {
                if (m_Acceleration > 0)
                {
                    // get slower without key press, but not moving backwards
                    m_Acceleration -= 0.5f * Time.deltaTime;
                    m_Acceleration = Mathf.Clamp(m_Acceleration, 0.0f, m_speed);
                }

                else if (m_Acceleration < 0)
                {
                    // get slower backwards without key press, but not moving forwards
                    m_Acceleration += 0.5f * Time.deltaTime;
                    m_Acceleration = Mathf.Clamp(m_Acceleration, -0.5f * m_speed, 0.0f);
                }
            }
            #endregion

            // clamp acceleration between (max)speed & half (max)speed for backwards movement
            m_Acceleration = Mathf.Clamp(m_Acceleration, -0.5f * m_speed, m_speed);

            SetIsDirty();
        }
        #endregion

        #region --- != localPlayer ---
        if (!IsLocalPlayer)
        {
            return;
        }
        #endregion

        #region --- localPlayer ---
        // rotation & movement
        Rotate();
        Move();

        // update position of camera
        //m_camera.transform.position = transform.position + m_cameraPositionShift;


        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && m_eItem != EItems.EMPTY)
        {
            // message use item (Mario)
            UseItemMessage message = new UseItemMessage(gameObject, (float)m_eItem);
            NetworkManager.Instance.SendMessageToServer(message);

            m_eItem = EItems.EMPTY;

            m_gameUIManager.UpdateItemImage(m_eItem);
        }

        #endregion
    }

    private void StartCountdown()
    {
        // Countdown zählt runter

        if (m_countdown <= 0.0f)
            m_canStart = true;
    }

    private void Move()
    {
        #region --- KeyDown W & S ---
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            AccelerationMessage message = new AccelerationMessage();
            message.PlayerID = 0;
            message.PressedDown = true;
            message.PlayerController = this;

            if (Input.GetKeyDown(KeyCode.W))
            {
                message.PressedKey = KeyCode.W;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                message.PressedKey = KeyCode.S;
            }

            // Server handles acceleration
            NetworkManager.Instance.SendMessageToServer(message);
        }
        #endregion

        #region --- KeyUp W & S ---
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            AccelerationMessage message = new AccelerationMessage();
            message.PlayerID = 0;
            message.PressedDown = false;
            message.PlayerController = this;

            if (Input.GetKeyUp(KeyCode.W))
            {
                message.PressedKey = KeyCode.W;
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                message.PressedKey = KeyCode.S;
            }

            NetworkManager.Instance.SendMessageToServer(message);
        }
        #endregion

        #region --- KeyDown Space ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AccelerationMessage message = new AccelerationMessage();
            message.PlayerID = 0;
            message.PressedDown = true;
            message.PressedKey = KeyCode.Space;
            message.PlayerController = this;

            // Server handles acceleration
            NetworkManager.Instance.SendMessageToServer(message);
        }
        #endregion

        #region --- KeyUp Space ---
        if (Input.GetKeyUp(KeyCode.Space))
        {
            AccelerationMessage message = new AccelerationMessage();
            message.PlayerID = 0;
            message.PressedDown = false;
            message.PressedKey = KeyCode.Space;
            message.PlayerController = this;

            // Server handles acceleration
            NetworkManager.Instance.SendMessageToServer(message);
        }
        #endregion

        #region --- movement ---
        Vector3 direction = transform.forward;
        direction = direction.normalized * m_Acceleration;
        //direction.y = m_rigidbody.velocity.y;
        m_rigidbody.MovePosition(transform.position + direction);

        #endregion
    }

    #region --- Rotate ---
    private void Rotate()
    {
        #region --- KeyDown A & D ---
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                // rotation left 
                transform.Rotate(Vector3.up * -90.0f * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D))
            {
                // rotation right 
                transform.Rotate(Vector3.up * 90.0f * Time.deltaTime);
            }
        }
        #endregion
    }
    #endregion

    // get position of specific network ID / player
    public Vector3 GetPosition(NetworkIdentity _netID)
    {
        Vector3 _position = _netID.transform.position;
        return _position;
    }

    // Mario
    public void UpdateVariable(float _speed, float _accel)
    {
        m_speed = _speed;
        m_Acceleration = _accel;
    }

    // Mario
    public void UpdateItem(float _item)
    {
        m_eItem = (EItems)_item;

        m_gameUIManager.UpdateItemImage(m_eItem);
    }

    #region --- OnTriggerEnter ---
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            // Spielerelevante Ereignisse
        }
        //if (IsLocalPlayer)
        //{

        //}
        if (IsClient)
        {

        }

        // Mario
        if (IsLocalPlayer && other.tag == "Coin" || other.tag == "Shell" || other.tag == "Boost" || other.tag == "ItemBox")
        {
            if (other.tag == "ItemBox")
            {
                // message CollisionCheck
                CollisionCheckMessage message = new CollisionCheckMessage(gameObject, other.gameObject, other.tag);
                NetworkManager.Instance.SendMessageToServer(message);
            }
            else
            {
                // message CollisionCheck
                CollisionCheckMessage message = new CollisionCheckMessage(gameObject, other.tag, m_speed, m_Acceleration);
                NetworkManager.Instance.SendMessageToServer(message);
            }
        }

        if (IsLocalPlayer && other.tag == "Finish")
        {
            FinishLineMessage message = new FinishLineMessage();
            message.PlayerID = 0;
            message.PlayerController = this;

            // Server handles finish race
            NetworkManager.Instance.SendMessageToServer(message);
        }
    }
    #endregion

    // bool that all players are spawn in the game scene
    public static bool IsInGame()
    {
        return m_isInGame;
    }

    // bool turns true after countdown is zero
    public static bool GetCanStart()
    {
        return m_canStart;
    }

    public static bool GetIsFinished()
    {
        return m_FinishedRace;
    }

}

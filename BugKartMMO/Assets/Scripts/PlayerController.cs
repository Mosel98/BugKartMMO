using Network;
using Network.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


// Frank
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [SyncVar]
    private float m_speed = 1.0f;

    [SyncVar]
    public float m_acceleration = 0.0f;

    [SyncVar]
    private Vector3 m_position;

    [SyncVar]
    public Quaternion m_rotation;

    //private Camera m_camera; // Position?! --> testen und festlegen!

    // vector shift of the camera based on player position
    private Vector3 m_cameraPositionShift;

    //[SyncVar]
    private int m_finishPlace;

    [SyncVar]
    public bool m_finishedRace;

    //[SyncVar]
    private EItems m_eItem;

    private Rigidbody m_rigidbody;

    // dictionary to see which key is pressed down
    public Dictionary<KeyCode, bool> m_keysPressed = new Dictionary<KeyCode, bool>();

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        // add keys for movement messages
        m_keysPressed.Add(KeyCode.W, false);
        m_keysPressed.Add(KeyCode.S, false);
        m_keysPressed.Add(KeyCode.A, false);
        m_keysPressed.Add(KeyCode.D, false);
        m_keysPressed.Add(KeyCode.Space, false);

        // m_camera = GetComponent<Camera>();

        // m_cameraPositionShift.Set(0.0f, 15.0f, -25.0f); // Verschiebung der Kamera
        // m_camera.transform.position = transform.position + m_cameraPositionShift;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        #region --- Server ---
        if (IsServer)
        {
            #region --- W & S ---
            // keys pressed down (true)
            if (m_keysPressed[KeyCode.W])
            {
                m_acceleration += 0.5f * Time.deltaTime;
            }

            if (m_keysPressed[KeyCode.S])
            {
                m_acceleration -= 0.5f * Time.deltaTime;
            }
            #endregion

            #region --- Space ---
            // handbreak, faster decrease speed
            if (m_keysPressed[KeyCode.Space])
            {
                m_acceleration -= 1.0f * Time.deltaTime;
                // don't go backwards with handbreak!
                m_acceleration = Mathf.Clamp(m_acceleration, 0.0f, m_speed);
            }
            #endregion

            #region --- No Key ---
            // no move key pressed
            if (!(m_keysPressed[KeyCode.W] ||
                m_keysPressed[KeyCode.A] ||
                m_keysPressed[KeyCode.S] ||
                m_keysPressed[KeyCode.D]))
            {
                if (m_acceleration > 0)
                {
                    // get slower without key press, but not moving backwards
                    m_acceleration -= 0.5f * Time.deltaTime;
                    m_acceleration = Mathf.Clamp(m_acceleration, 0.0f, m_speed);
                }

                else if (m_acceleration < 0)
                {
                    // get slower backwards without key press, but not moving forwards
                    m_acceleration += 0.5f * Time.deltaTime;
                    m_acceleration = Mathf.Clamp(m_acceleration, -0.5f * m_speed, 0.0f);
                }
            }
            #endregion

            // clamp acceleration between (max)speed & half (max)speed for backwards movement
            m_acceleration = Mathf.Clamp(m_acceleration, -0.5f * m_speed, m_speed);

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


        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            // message use item (Mario)
            UseItemMessage message = new UseItemMessage(gameObject, (float)m_eItem);
            NetworkManager.Instance.SendMessageToServer(message);
        }

        #endregion
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
        direction = direction.normalized * m_acceleration;
        direction.y = m_rigidbody.velocity.y;
        m_rigidbody.velocity = direction;

        transform.position += direction;
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

    // Mario
    public void UpdateVariable(float _speed, float _accel)
    {
        m_speed = _speed;
        m_acceleration = _accel;
    }

    // Mario
    public void UpdateItem(float _item)
    {
        m_eItem = (EItems)_item;
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
                CollisionCheckMessage message = new CollisionCheckMessage(gameObject, other.tag);
                NetworkManager.Instance.SendMessageToServer(message);
            }
            else
            {
                // message CollisionCheck
                CollisionCheckMessage message = new CollisionCheckMessage(gameObject, other.tag, m_speed, m_acceleration);
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
}

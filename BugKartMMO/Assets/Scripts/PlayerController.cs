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
    private float m_speed = 10.0f;

    [SyncVar]
    private float m_acceleration = 0.0f;

    //[SyncVar]
    private int m_finishPlace;

    [SyncVar]
    private Vector3 m_position; // nur localplayer ändern, keine Message!!!

    [SyncVar]
    private Quaternion m_rotation; // nur localplayer ändern, keine Message!!!

    //private Camera m_camera; // Position?! --> testen und festlegen!

    private Vector3 m_cameraPositionShift; // Verschiebung der Kamera vom Player aus gesehen


    //[SyncVar]
    private bool m_finishedRace;

    //[SyncVar]
    private EItems m_eItem;

    private Rigidbody m_rigidbody;

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        //m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
        //m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;

        // m_camera = GetComponent<Camera>();
        //
        // m_cameraPositionShift.Set(0.0f, 15.0f, -25.0f); // Verschiebung der Kamera
        // m_camera.transform.position = transform.position + m_cameraPositionShift;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!IsLocalPlayer)
        {
            return;
        }

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
        
    }

    private void Move()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ShootItemMessage message = new ShootItemMessage();
        //    message.ItemPrefab = null;
        //    message.PlayerID = 0;
        //}

        // message increase decrease acceleration

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            AccelerationMessage message = new AccelerationMessage();
            message.PlayerID = 0; // ist das richtig?!
            message.Acceleration = m_acceleration;
            message.Speed = m_speed;
            if (Input.GetKey(KeyCode.W))
            {
                message.PressedKey = KeyCode.W;
                //m_acceleration += 0.5f * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                message.PressedKey = KeyCode.S;
                //m_acceleration -= 0.5f * Time.deltaTime;
            }

            if (IsServer)
            {
                NetworkManager.Instance.SendMessageToClients(message);
            }
            else
            {
                NetworkManager.Instance.SendMessageToServer(message);
            }

        }


        if (Input.GetKey(KeyCode.Space))
        {
            // message handbreak
            HandbreakMessage message = new HandbreakMessage();
            message.PlayerID = 0; // ist das richtig?!
            //message.Speed = m_speed;
            message.Acceleration = m_acceleration;

            if (IsServer)
            {
                NetworkManager.Instance.SendMessageToClients(message);
            }
            else
            {
                NetworkManager.Instance.SendMessageToServer(message);
            }
        }


        // no key pressed --> acceleration decreases
        else 
        {
            AccelerationMessage message = new AccelerationMessage();
            message.PlayerID = 0; // ist das richtig?!
            message.Acceleration = m_acceleration;
            message.Speed = m_speed;
            message.PressedKey = KeyCode.None;

            //if (m_acceleration > 0)
            //    m_acceleration -= 0.5f * Time.deltaTime;
            //else if (m_acceleration < 0)
            //    m_acceleration += 0.5f * Time.deltaTime;
            //else
            //{
            //    m_acceleration = 0.0f;
            //}

            //if (m_speed > 0)
            //{
            //    m_speed -= 0.1f * Time.deltaTime;
            //}
            //else if (m_speed < 0)
            //{
            //    m_speed += 0.1f * Time.deltaTime;
            //}


            //SetIsDirty(); ??? 
            if (IsServer)
            {
                NetworkManager.Instance.SendMessageToClients(message);
            }
            else
            {
                NetworkManager.Instance.SendMessageToServer(message);
            }
        }

        // noch neu ordnen!!! & sortieren / rauswerfen
        //m_acceleration = Mathf.Clamp(m_acceleration, -5.0f, 5.0f);
        //m_speed += m_acceleration;
        // m_speed = Mathf.Clamp(m_speed, -m_maxSpeed * 0.5f, m_maxSpeed);

        Vector3 direction = transform.forward;// * m_acceleration;
        //direction = direction.normalized * m_speed;

        //Quaternion _rot = transform.rotation * m_rotation.normalized;
        Quaternion _rot = transform.localRotation * m_rotation.normalized;
        
        //transform.position.forward * m_acceleration;

        direction = direction.normalized * m_acceleration;
        direction.y = m_rigidbody.velocity.y;
        m_rigidbody.velocity = direction;

        transform.position += direction;
        //transform.localRotation *= _rot;

        //direction *= Time.deltaTime * m_speed;

        //direction = transform.TransformDirection(direction);

        //m_rigidbody.MoveRotation(m_rotation);
    }
    
    private void Rotate()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            RotationMessage message = new RotationMessage();
            message.PlayerID = 0; // ist das richtig?!
            message.Rotation = m_rotation;

            if (Input.GetKey(KeyCode.A))
            {
                // - y -> Inverse richtig?!
                //m_rotation = Quaternion.Inverse(m_rotation) * Quaternion.Euler(0.0f, 5.0f, 0.0f);
                //m_rotation = transform.rotation * Quaternion.Euler(0.0f, -5.0f, 0.0f);

               // transform.Rotate(0.0f, -1.0f, 0.0f);
                message.PressedKey = KeyCode.A;
            }

            if (Input.GetKey(KeyCode.D))
            {
                // + y
                // m_rotation = transform.rotation * Quaternion.Euler(0.0f, 5.0f, 0.0f);

               // transform.Rotate(0.0f, 1.0f, 0.0f);
                message.PressedKey = KeyCode.D;
            }

            if (IsServer)
            {
                NetworkManager.Instance.SendMessageToClients(message);
            }
            else
            {
                NetworkManager.Instance.SendMessageToServer(message);
            }
        }
    }

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
            if(other.tag == "ItemBox")
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
    }
}

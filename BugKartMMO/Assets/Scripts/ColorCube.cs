using Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCube : NetworkBehaviour
{
    [SyncVar]
    public string m_CubeName;
    [SyncVar]
    public Color m_Color;
    [SyncVar]
    public int m_Age;
    [SyncVar]
    public KeyCode m_KeyCode;

    protected override void Start()
    {
        base.Start();

        if (IsServer)
        {
            GetComponent<Rigidbody>().useGravity = true;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (IsServer)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                m_CubeName = "Hallo Welt";
                SetIsDirty();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                m_Color = Color.red;
                SetIsDirty();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                m_Age++;
                SetIsDirty();
            }
        }
    }
}

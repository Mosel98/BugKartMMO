using UnityEngine;

public class StartPosition : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Player;
    [SerializeField]
    private Transform[] m_StartPoint = new Transform[5];



    // Start is called before the first frame update
    void Start()
    {
        // Check in which Slot ever Player was in the Lobby and at which Start Position he is allowed to spawn
        switch (SlotPosition.SlotID)
        {
            case 0:
                Debug.Log("Player One");
                // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                Instantiate(m_Player, m_StartPoint[0].position, transform.parent.rotation);

                // change Color of Player
                Renderer rendP1 = m_Player.GetComponent<Renderer>();
                rendP1.material.color = Color.blue;
                break;
            case 1:
                Debug.Log("Player Two");
                // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                Instantiate(m_Player, m_StartPoint[1].position, transform.parent.rotation);

                // change Color of Player
                Renderer rendP2 = m_Player.GetComponent<Renderer>();
                rendP2.material.color = Color.red;
                break;
            case 2:
                Debug.Log("Player Three");
                // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                Instantiate(m_Player, m_StartPoint[2].position, transform.parent.rotation);

                // change Color of Player
                Renderer rendP3 = m_Player.GetComponent<Renderer>();
                rendP3.material.color = Color.green;
                break;
            case 3:
                Debug.Log("Player Four");
                // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                Instantiate(m_Player, m_StartPoint[3].position, transform.parent.rotation);


                // change Color of Player
                Renderer rendP4 = m_Player.GetComponent<Renderer>();
                rendP4.material.color = Color.magenta;
                break;
            case 4:
                Debug.Log("Player Five");
                // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                Instantiate(m_Player, m_StartPoint[4].position, transform.parent.rotation);


                // change Color of Player
                Renderer rendP5 = m_Player.GetComponent<Renderer>();
                rendP5.material.color = Color.gray;
                break;
        }
    }
}

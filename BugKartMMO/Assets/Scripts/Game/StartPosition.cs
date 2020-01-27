using UnityEngine;

public class StartPosition : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Player;
    [SerializeField]
    private Transform[] m_StartPoint;

    // Start is called before the first frame update
    void Start()
    {
        // Check in which Slot ever Player was in the Lobby and at which Start Position he is allowed to spawn
        switch (SlotPosition.SlotID)
        {
            case 0:
                Debug.Log("Player One");
                // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                Instantiate(m_Player);
                break;
            case 1:
                Debug.Log("Player Two");
                // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                break;
            case 2:
                Debug.Log("Player Three");
                // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                break;
            case 3:
                Debug.Log("Player Four");
                // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                break;
            case 4:
                Debug.Log("Player Five");
                // Instantiate(m_Player, Hier Gewünschte Position eingeben, transform.parent.rotation);
                break;
        }
    }
}

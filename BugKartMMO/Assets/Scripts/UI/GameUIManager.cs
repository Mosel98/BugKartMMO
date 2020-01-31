using Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Mario
public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private Image m_itemImage;

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

    public void BackToLobby()
    {
        Debug.Log("HI");

        NetworkManager.Instance.BroadcastMessage("BackToLobby");
    }

    private void SceneSwitchLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void UpdateItemImage(EItems _eItem)
    {
        switch (_eItem)
        {
            case EItems.EMPTY:
                m_itemImage.sprite = m_imgEmpty;
                break;
            case EItems.COIN:
                m_itemImage.sprite = m_imgCoin;
                break;
            case EItems.MUSHROOM:
                m_itemImage.sprite = m_imgMush;
                break;
            case EItems.KÖTTEL:
                m_itemImage.sprite = m_imgKöttel;
                break;
            case EItems.GREENSHELL:
                m_itemImage.sprite = m_imgShell;
                break;
        }
    }
}

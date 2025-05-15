using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class OnlineOflineManager : MonoBehaviour
{
    [SerializeField] private GameObject onlineOflineBtn;
    [SerializeField] private GameObject offlineGirisBtn;
    [SerializeField] private TextMeshProUGUI onlineOflineTxt;
    public static bool isOnline;

    private void Start()
    {
        isOnline = false;
    }
    public void OnlineOflineBtn()
    {
        isOnline = !isOnline;

        if (isOnline)
        {
            onlineOflineBtn.GetComponent<Image>().color = Color.green;
            onlineOflineTxt.text = "Online";
            offlineGirisBtn.SetActive(false);
        }
        else
        {           
            onlineOflineBtn.GetComponent<Image>().color = Color.red;
            onlineOflineTxt.text = "Offline";
            offlineGirisBtn.SetActive(true);
        }
    }

    public void OfflineGiris()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}

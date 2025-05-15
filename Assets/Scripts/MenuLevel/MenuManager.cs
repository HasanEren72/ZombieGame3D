using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour 
{
    [Space(10)]
    [Header("Panel Nesneleri")]
    [SerializeField] private GameObject silahSatinAlmaMenu;
    [SerializeField] private GameObject KaynakCevirmeMenu;

    public GameObject[] silahPanelleri;
    [SerializeField] private GameObject[] kaynakPanelleri;
    
    [Space(10)]
    [Header("Diğer")]
    [SerializeField] private TextMeshProUGUI market_btnTxt;
    public TextMeshProUGUI bilgiMesajiTxt;

    public static bool bolum1;
    public static bool Bolumgecti;

    public AudioClip backGrounMusic;
 
    private void Start()
    {     
        Bolumgecti = false;
        silahSatinAlmaMenu.SetActive(false);
        KaynakCevirmeMenu.SetActive(false);
        bilgiMesajiTxt.GetComponent<CanvasGroup>().alpha = 0;
        AudioManager.Instance.PlayMusic(backGrounMusic);
        AudioManager.Instance.ButonlaraClickSesiEkle();
    }

    public void marketBtn()
    {
        market_btnTxt.color = Color.red;
        silahSatinAlmaMenu.SetActive(true);
    }

    public void KaynakCevirmeMenuAcBtn()
    {
        KaynakCevirmeMenu.SetActive(true);
        silahSatinAlmaMenu.SetActive(false);
    }
    public void silahSatinAlmaMenuAcBtn()
    {
        silahSatinAlmaMenu.SetActive(true);
        KaynakCevirmeMenu.SetActive(false);
    }

    public void SilahPanelAc(int index)
    {
        for (int i = 0; i < silahPanelleri.Length; i++)
        {
            silahPanelleri[i].SetActive(i == index); //burada sadece i=index olduğunda true donecek 
        }                                           //yani sadece istenilen panel true olacak diğerleri false olacak        
    }

    public void kaynakPanelAc(int index)
    {
        for (int i = 0; i < silahPanelleri.Length; i++)
        {
            kaynakPanelleri[i].SetActive(i == index); //burada sadece i=index olduğunda true donecek 
        }                                           //yani sadece istenilen panel true olacak diğerleri false olacak
    }

    public void hayir_btn(int index)
    {
        silahPanelleri[index].SetActive(false);
        kaynakPanelleri[index].SetActive(false);
        MesajGizle();            
    }

    public void TumpPanelleriKapat()
    {
        foreach (var item in silahPanelleri)
        {
            item.SetActive(false);
        }
        foreach (var item in kaynakPanelleri)
        {
            item.SetActive(false);
        }
        silahSatinAlmaMenu.SetActive(false);
        KaynakCevirmeMenu.SetActive(false);
        market_btnTxt.color = Color.yellow;
        MesajGizle();     
    }

    public void bolum1_basla()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Level1");
        bolum1 = true;
        Cursor.lockState = CursorLockMode.Locked;// fare imlecini kilidini
    }
    public void bolum2_basla()
    {
        if (Bolumgecti == false) //bolum geçilmedi ise açmaz
        {
            StartCoroutine(mesajGoster("Bölüm2 Kilitli !"));
        }
        else  // 1. bolüm geçildi ise açar
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Level2");
            bolum1 = false;
            Cursor.lockState = CursorLockMode.Locked;// fare imlecini kiliTLER
        }
    }

    public void MesajGizle()
    {
        bilgiMesajiTxt.GetComponent<CanvasGroup>().alpha = 0;
    }
    IEnumerator mesajGoster(string mesaj)
    {
        bilgiMesajiTxt.text = mesaj;
        bilgiMesajiTxt.GetComponent<CanvasGroup>().alpha = 1;
        yield return new WaitForSeconds(3);
        bilgiMesajiTxt.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void SesAcKapa()
    {
        AudioManager.Instance.ToggleMusic();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

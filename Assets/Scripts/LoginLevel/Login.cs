using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [Header("Input Field Nesneleri")]
    [SerializeField] private TMP_InputField kullaniciAdi_IF;
    [SerializeField] private TMP_InputField sifre_IF;

    [Space(10)]
    [Header("Statik Değerler")]
    public static string kullaniciAdi;
    public static string sifre;
    
    UI_Control uiControl;

    private void Awake()
    {
        uiControl = GetComponent<UI_Control>();
    }

    private void Start()
    {
        PlayerPrefs.DeleteAll();
    }

    public void girisYap_B() // Giriş yapma butonu işlevi.
    {
        if (string.IsNullOrEmpty(kullaniciAdi_IF.text) || string.IsNullOrEmpty(sifre_IF.text))
        {
            StartCoroutine(uiControl.hataPanel("Boş BIRAKMAYINIZ!"));
        }
        else
        {
            StartCoroutine(uiControl.hataPanel("Giriş Başarılı"));
            StartCoroutine(girisYap());
        }
    }

    IEnumerator girisYap() // Sunucuya giriş isteği gönderir ve sonucu işler.
    {
        WWWForm form = new WWWForm();
        form.AddField("unity", "girisYapma");
        form.AddField("kullaniciAdi", kullaniciAdi_IF.text);
        form.AddField("sifre", sifre_IF.text);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_DB/user.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Hata: " + www.error);
            }
            else
            {
                Debug.Log("Sorgu Sonucu: " + www.downloadHandler.text);

                if (www.downloadHandler.text.Contains("giris başarili"))
                {
                    KullaniciGirisiKaydet();
                    SceneManager.LoadScene("Menu", LoadSceneMode.Single);
                }
                else
                {
                    StartCoroutine(uiControl.hataPanel(www.downloadHandler.text));
                }
            }
        }
    }

    public void KullaniciGirisiKaydet()  // Kullanıcı adı ve şifre değerlerini statik değişkenlere atar.
    {
        kullaniciAdi = kullaniciAdi_IF.text;
        sifre = sifre_IF.text;
    }
}

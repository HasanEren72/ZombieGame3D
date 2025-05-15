using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class create : MonoBehaviour
{
    [Header("Input Alanları")]
    [SerializeField] private TMP_InputField kullaniciAdi_IF;
    [SerializeField] private TMP_InputField sifre_IF;
    [SerializeField] private TMP_InputField sifreTekrar_IF;

    [Space(10)][Header("Onay Kutusu")]
    [SerializeField] private Toggle sozlesme;

    public static bool kayitoldu;

    private UI_Control uiControl;

    private void Start()
    {
        kayitoldu = false;
        uiControl = GetComponent<UI_Control>();
    }

    public void uyeligiOlustur_Buton() // Kayıt butonuna basıldığında gerekli kontrolleri yapar ve kayıt işlemini başlatır.
    {
        if (string.IsNullOrEmpty(kullaniciAdi_IF.text) || string.IsNullOrEmpty(sifre_IF.text) || string.IsNullOrEmpty(sifreTekrar_IF.text))
        {
            StartCoroutine(uiControl.hataPanel("Boş BIRAKMAYINIZ!"));
            return;
        }

        if (!sifre_IF.text.Equals(sifreTekrar_IF.text))
        {
            StartCoroutine(uiControl.hataPanel("Şifreler Eşleşmiyor!"));
            return;
        }

        if (!sozlesme.isOn)
        {
            StartCoroutine(uiControl.hataPanel("Lütfen Sözleşmeyi Kabul Ediniz!"));
            return;
        }

        Debug.Log("Veritabanı Bağlantısı");
        StartCoroutine(KayitOl());
    }

    IEnumerator KayitOl() // Kullanıcı bilgilerini veritabanına kaydeder.
    {
        WWWForm form = new WWWForm();
        form.AddField("unity", "kayitOlma");
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
                string result = www.downloadHandler.text;
                Debug.Log("Cevap: " + result);

                if (result.Contains("kayit basarili"))
                {
                    PlayerPrefs.DeleteAll();
                    kayitoldu = true;
                    StartCoroutine(IlkSkorlarKayit());
                    StartCoroutine(uiControl.hataPanel(result));
                }
                else
                {
                    StartCoroutine(uiControl.hataPanel(result));
                }
            }
        }
    }

    IEnumerator IlkSkorlarKayit() // Kullanıcının ilk skorlarını veritabanına kaydeder.
    {
        WWWForm form = new WWWForm();
        form.AddField("unity", "ilk_skorlar_ekleme");
        form.AddField("kullaniciAdi", kullaniciAdi_IF.text);
        form.AddField("sifre", sifre_IF.text);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_DB/user.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("İlk Skorlar ekleme hatası: " + www.error);
            }
            else
            {
                Debug.Log("İlk skorlar ekleme sonucu: " + www.downloadHandler.text);
            }
        }
    }
}

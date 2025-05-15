using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class DataBase : MonoBehaviour
{
    [Header("Text Nesneleri")]
    [SerializeField] private TextMeshProUGUI puanTxt;
    [SerializeField] private TextMeshProUGUI altinTxt;
    [SerializeField] private TextMeshProUGUI elmasTxt;
    [SerializeField] private TextMeshProUGUI kullaniciAdiTxt;
    [SerializeField] private TextMeshProUGUI Convert_btnTxt;

    [Space(10)]
    [Header("Butonlar")]
    [SerializeField] private Button onlineOflineBtn;
    [SerializeField] private Button level2Btn;

    [Space(10)]
    [Header("Silah Satýn Alma")]
    [SerializeField] private GameObject[] silahPaneliAcButonlari;
    [SerializeField] private GameObject[] silahSatinAlindiMesajlari;

    [Space(10)]
    [Header("Çevirme Butonlarý")]
    [SerializeField] private Button ConvertEvetBtn1;
    [SerializeField] private Button ConvertEvetBtn2;
    [SerializeField] private Button ConvertEvetBtn3;

    [Space(10)]
    [Header("Silah Satýn Alma Butonlarý (Evet Butonlarý)")]
    [SerializeField] private Button silahSatinAlEvetBtn1;
    [SerializeField] private Button silahSatinAlEvetBtn2;
    [SerializeField] private Button silahSatinAlEvetBtn3;

    [Space(10)]
    [Header("Diðer Deðiþkenler")]
    public string kullaniciAdi;
    public string sifre;
    [SerializeField] private bool isOnline;
    [SerializeField] private GameObject kilitÝmg;
    [SerializeField] private AudioClip SatinAlmaSesi;
  
    MenuManager menuManager;

    string[] Skorlar;//puan ,altin ,elmas (14455,1000,200)

    private void Awake()
    {
        menuManager = Object.FindObjectOfType<MenuManager>(); 
    }
    public void Start()
    {
        level2Btn.GetComponent<Button>().interactable = false;
        isOnline = OnlineOflineManager.isOnline;
        ilkverileriAl();
        MethodAtama();
    }
    void MethodAtama()
    {
        ConvertEvetBtn1.onClick.AddListener(() => ConvertKaynak("elmas", 500, int.Parse(Skorlar[2])));
        ConvertEvetBtn2.onClick.AddListener(() => ConvertKaynak("elmas", 100, int.Parse(Skorlar[2])));
        ConvertEvetBtn3.onClick.AddListener(() => ConvertKaynak("altin", 5000, int.Parse(Skorlar[1])));

        silahSatinAlEvetBtn1.onClick.AddListener(() => SilahSatinAl(1, "ak47", "altin", 5000, int.Parse(Skorlar[1])));
        silahSatinAlEvetBtn2.onClick.AddListener(() => SilahSatinAl(2, "m416", "elmas", 200, int.Parse(Skorlar[2])));
        silahSatinAlEvetBtn3.onClick.AddListener(() => SilahSatinAl(3, "m16a4", "altin", 5000, int.Parse(Skorlar[1])));
    }

    void ilkverileriAl()
    {
        //Veriyi al       
        kullaniciAdi = Login.kullaniciAdi;
        sifre = Login.sifre;

        kullaniciAdiTxt.text = kullaniciAdi;

        PlayerPrefs.DeleteKey("Altin_verisi"); // deðerleri siler 
        PlayerPrefs.DeleteKey("Puan_verisi");
        PlayerPrefs.DeleteKey("Elmas_verisi");

        if (isOnline)
        {
            SkorlariCek(); //skor iþlemleri skor çekme iþlemleri
            onlineOflineBtn.GetComponent<Image>().color = Color.green;
        }
    }

    public void OnlineOflineBtn()
    {
        isOnline = !isOnline;

        if (isOnline)
        {
            SkorlariCek();
            onlineOflineBtn.GetComponent<Image>().color = Color.green;
        }
        else
        {
            onlineOflineBtn.GetComponent<Image>().color = Color.red;
        }
    }
    public void SkorlariCek()
    {
        StartCoroutine(TumVerileriCek());
        StartCoroutine(SilahCekme(0 ,"ak47"));
        StartCoroutine(SilahCekme(1 ,"m416"));
        StartCoroutine(SilahCekme(2 ,"m16a4"));
        StartCoroutine(bolumKilidiAcmaCekmeÝslemi());
    }

    public  IEnumerator TumVerileriCek()
    {
        if (isOnline)
        {
            WWWForm form = new WWWForm();
            form.AddField("unity", "Tum_Skor_verileri_cek");
            form.AddField("kullaniciAdi", kullaniciAdi);
            form.AddField("sifre", sifre);

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_DB/user.php", form))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log("Baðlantý hatasý: " + www.error);
                }
                else
                {
                    string Result = www.downloadHandler.text;
                    Debug.Log("Gelen Veri: " + Result);

                    yield  return Result;
                    if (Result != "basarisiz")
                    {
                        Skorlar = Result.Split("|"); //diziye 

                        puanTxt.text = Skorlar[0];
                        altinTxt.text = Skorlar[1];
                        elmasTxt.text = Skorlar[2];
                    }
                    else
                    {
                        Debug.LogWarning("Veri çekilemedi (basarisiz)");
                    }
                }
            }
        }
    }

    IEnumerator SilahCekme(int index ,string silahAdi)
    {
        if (isOnline)
        {
            WWWForm form = new WWWForm();
            form.AddField("unity", "SilahCekme");
            form.AddField("kullaniciAdi", kullaniciAdi);
            form.AddField("sifre", sifre);
            form.AddField("silahAdi", silahAdi);

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_DB/user.php", form))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    string result = www.downloadHandler.text;

                    if (int.Parse(result) == 1) //silah satin alýndý ise
                    {
                        silahPaneliAcButonlari[index].SetActive(false);
                        silahSatinAlindiMesajlari[index].SetActive(true);
                        menuManager.silahPanelleri[index].SetActive(false);
                    }
                    else
                    {
                        Debug.Log("silah envaterde yok!");
                    }
                }
            }
        }
    }

    public IEnumerator bolumKilidiAcmaCekmeÝslemi()
    {
        if (isOnline)
        {
            WWWForm form = new WWWForm();
            form.AddField("unity", "YeniBolumKilitCekme");
            form.AddField("kullaniciAdi", kullaniciAdi);
            form.AddField("sifre", sifre);

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_DB/user.php", form))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    string cevap = www.downloadHandler.text;

                    if (int.Parse(cevap) == 1)
                    {
                        kilitÝmg.SetActive(false);
                        level2Btn.GetComponent<Button>().interactable = true;
                    }
                    else
                    {
                        kilitÝmg.SetActive(true);
                    }
                    www.Dispose();
                }
            }
        }
    }

    public void ConvertKaynak(string kaynak, int HarcamaMiktari, int MevcutMiktar)
    {
        if (isOnline)
        {
            if (MevcutMiktar >= HarcamaMiktari)
            {
                StartCoroutine(Donusturme_Guncelleme(kaynak, HarcamaMiktari));              
            }
            else
            {
                MesajGoster($"Dönüþtürme baþarýsýz ! {kaynak} yetersiz!");
            }
        }
    }

    IEnumerator Donusturme_Guncelleme(string kaynak, int HarcamaMiktari)
    {
        if (isOnline)
        {
            WWWForm form = new WWWForm();
            form.AddField("unity", "Donusturme_Guncelleme");
            form.AddField("kullaniciAdi", kullaniciAdi);
            form.AddField("sifre", sifre);
            form.AddField("kaynak", kaynak);
            form.AddField("HarcamaMiktari", HarcamaMiktari);

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_DB/user.php", form))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    MesajGoster("Donusturme sonucu :" + www.downloadHandler.text);
                    AudioManager.Instance.PlaySFX(SatinAlmaSesi);
                    StartCoroutine(TumVerileriCek());
                }
            }
        }
    }

    public void SilahSatinAl(int silahId , string silahAdi ,string hangiKaynak, int HarcamaMiktari, int MevcutMiktar)//int silahFiyati,int silahId
    {
        if (isOnline)
        {
            if (MevcutMiktar>= HarcamaMiktari)
            {
                StartCoroutine(KaynakGuncelleme($"{hangiKaynak}", HarcamaMiktari));
                StartCoroutine(SilahlarGuncelleme_ekleme(silahId)); //akm47 old. belirtmek için
                AudioManager.Instance.PlaySFX(SatinAlmaSesi);//1 kere sesi çalar
                MesajGoster($"{silahAdi} silahý Satýn Alýndý .");
            }
            else
            {
                MesajGoster($"satýn alma baþarýsýz ! {hangiKaynak} yetersiz!");
            }
        }
    }

    IEnumerator KaynakGuncelleme(string kaynak, int HarcamaMiktari) //elmas ,altin  /200 5000
    {
        if (isOnline)
        {
            WWWForm form = new WWWForm();
            form.AddField("unity", "Kaynak_Guncelleme");
            form.AddField("kullaniciAdi", kullaniciAdi);
            form.AddField("sifre", sifre);
            form.AddField("kaynak", kaynak);
            form.AddField("HarcamaMiktari", HarcamaMiktari);

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_DB/user.php", form))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    MesajGoster("kaynak Guncelleme sonucu :" + www.downloadHandler.text);
                    SkorlariCek();
                    www.Dispose();
                }
            }
        }
    }

    IEnumerator SilahlarGuncelleme_ekleme(int silahId)
    {
        if (isOnline)
        {
            WWWForm form = new WWWForm();
            form.AddField("unity", "silahSatinAlma_Guncelleme");
            form.AddField("kullaniciAdi", kullaniciAdi);
            form.AddField("sifre", sifre);

            if (silahId == 1)
            {
                form.AddField("ak47", 1);
            }
            else if (silahId == 2)
            {
                form.AddField("m416", 1);
            }
            else if (silahId == 3)
            {
                form.AddField("m16a4", 1);
            }

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_DB/user.php", form))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    MesajGoster("Silahý satýn alma  sonucu :" + www.downloadHandler.text);
                    SkorlariCek();
                    www.Dispose();
                }
            }
        }
    }

    void MesajGoster(string mesaj)
    {
        menuManager.bilgiMesajiTxt.text = mesaj;
        menuManager.bilgiMesajiTxt.GetComponent<CanvasGroup>().alpha = 1;     
    }
}

using System.Collections;
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    [Header("UI Elemanları")]
    [SerializeField] private TextMeshProUGUI CanText;
    [SerializeField] private TextMeshProUGUI MermiText;
    [SerializeField] private TextMeshProUGUI oluText;   
    [SerializeField] private TextMeshProUGUI infoText;  
    [SerializeField] private GameObject OyunDurduPaneli;
    [SerializeField] private GameObject ZombileriOldurBtn;
    [SerializeField] private GameObject infoTextObjesi;

    [Space(10)]
    [Header("Zombi ve Canavarlar")]
    [SerializeField] private GameObject[] ClonZombiler1; // her bir dizide farklı özellikte zombiler olacak;
    [SerializeField] private GameObject[] ClonZombiler2;  
    [SerializeField] private GameObject[] ClonZombiler3;   
    [SerializeField] private GameObject BolumCanavari;

    [Space(10)]
    [Header("Kameralar")]
    [SerializeField] private GameObject oyuncuKamerasi;
    [SerializeField] private GameObject DurduKamerasi;

    [Space(10)]
    [Header("Kullanıcı Verileri")]
    string kullaniciAdi;
    string sifre;

    [Space(10)]
    [Header("Skor Verileri")]
    [SerializeField] private int altin = 0; // veriler için bu değişkenler kullanılır
    [SerializeField] private int elmas = 0; 
    [SerializeField] private int puan = 0;

    [Space(10)]
    [Header("Zombilerin rastgele spawn pozisyonları")]
    [SerializeField] private Vector4 Bolge1; //(x,y,z,w)
    [SerializeField] private Vector4 Bolge2;
    [SerializeField] private Vector4 Bolge3;

    [Space(10)]
    [Header("Diğer Veriler")]
    public int oluSayisi = 0;
    public int sahnedekiZombiSayisi = 0;
    public bool pausePanelinde = false;
    bool OyunBittimi;

    [Space(10)]
    [Header("Sesler")]
    [SerializeField] private AudioClip oyunBittiSesi;

    AtesEtmeManager atesEtmeManager;
    PlayerControl playerControl;
    MenuManager menuManager;

    private void Awake()
    {
        playerControl =Object.FindObjectOfType<PlayerControl>();
        menuManager = Object.FindObjectOfType<MenuManager>();
        atesEtmeManager = Object.FindObjectOfType<AtesEtmeManager>();
    }

    void Start()
    {
        kullaniciAdi = Login.kullaniciAdi;
        sifre = Login.sifre;
        OyunBittimi = false;
        DalgaBaslat("1.Dalga Başlıyor ...",10,ClonZombiler1);
    }

    void Update()     
    {
        UIGuncelle();

        if (!OyunBittimi) //oyun bitmedi ise
        {
            AzaltNesneSayisi();
        }

        if (!playerControl.hayattaMi) //oyuncu oldu ise
        {
            StartCoroutine(SkorlariGuncelle());//skorları veritabanına ekler
            playerControl.hayattaMi = true;
        }

        if (Input.GetKey(KeyCode.Escape)) // esc ye basınca oyunu durdurur.
        {
            OyunuDurdur();
        }

        if (Input.GetKey(KeyCode.K)) //k ye basıldığı zaman pause panelinde ZombileriOldurBtn aktif eder.
        {
            ZombileriOldurBtn.SetActive(true);
        }
    }

    void UIGuncelle()
    {
        MermiText.text = $"{atesEtmeManager.GetSarjor()}/{atesEtmeManager.GetCephane()}";
        CanText.text = $"Can: %{playerControl.GetKarekerCan()}";
    }

    public void DalgaBaslat(string mesaj, int zombiSayisi, GameObject[] zombiListesi)
    {
        BilgiMesajiGoster(mesaj);

        for (int i = 0; i < zombiSayisi; i++)
        {
            Vector3 pozisyon = GetRandomPozisyon();
            Quaternion rotasyon = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
            int zombiIndeks = Random.Range(0, zombiListesi.Length);

            Instantiate(zombiListesi[zombiIndeks], pozisyon, rotasyon);
            sahnedekiZombiSayisi++;
        }
    }

    private Vector3 GetRandomPozisyon()
    {
        int bolge = Random.Range(1, 4);
        float x = 0f, z = 0f;

        switch (bolge)
        {
            case 1:
                x = Random.Range(Bolge1.x, Bolge1.y);
                z = Random.Range(Bolge1.z, Bolge1.w);
                break;
            case 2:
                x = Random.Range(Bolge2.x, Bolge2.y);
                z = Random.Range(Bolge2.z, Bolge2.w);
                break;
            case 3:
                x = Random.Range(Bolge3.x, Bolge3.y);
                z = Random.Range(Bolge3.z, Bolge3.w);
                break;
        }

        return new Vector3(x, 0, z);
    }

    public void BolumSonuCanavari()
    {
        BilgiMesajiGoster("Dikkat Bölüm Sonu Canavarı Geliyor , Çok Tehlikeli Ve  Güçlü Savunmaya Gücüne Sahip !!!");

        Vector3 randomPozisyon = GetRandomPozisyon();                 
        Quaternion randomRotasyon = Quaternion.Euler(0f, Random.Range(0, 360), 0f);

        GameObject nesne = Instantiate(BolumCanavari, randomPozisyon, randomRotasyon);
        sahnedekiZombiSayisi++;
    }

    void DalgaKontrol()
    {
        switch (oluSayisi)
        {
            case 10:
                DalgaBaslat("2.Dalga Başlıyor ... Dikkatli olun evrim geçirmişler artık daha güçlüler !!!", 20, ClonZombiler2);
                elmas += 50;
                break;

            case 30:
                DalgaBaslat("3.Dalga Başlıyor ... Dikkatli olun evrim geçirmişler artık daha güçlüler !!!", 40, ClonZombiler3);
                elmas += 100;
                break;

            case 70:
                BolumSonuCanavari(); //bolum sonu canavarı
                elmas += 500;
                break;

            case 71:         //kazanmak
                elmas += 1000;
                OyunBitti();
                break;
        }
    }

    void AzaltNesneSayisi()
    {
        GameObject[] zombiNesneleri = GameObject.FindGameObjectsWithTag("Zombi");
        int aktifZombiSayisi = zombiNesneleri.Length;

        if (aktifZombiSayisi < sahnedekiZombiSayisi && !pausePanelinde)
        {
            oluSayisi++;
            oluText.text = $"Ölü Sayısı: {oluSayisi}";
            altin += 100;
            puan += 200;
            sahnedekiZombiSayisi = aktifZombiSayisi;
        }

        if (aktifZombiSayisi == 0)
            DalgaKontrol();
    }

    void OyunBitti()
    {
        BilgiMesajiGoster("*** Tebrikler Kazandınız *** Sonraki bölüme geçmek için AnaMenuye dönün");
        Time.timeScale = 0;
        AudioManager.Instance.PlaySFX(oyunBittiSesi);
        KameraVeCursorAyarla(true);
        StartCoroutine(YeniBomumKilitAc());
        StartCoroutine(SkorlariGuncelle());//skorları veritabanına ekler
        OyunBittimi = true;
        MenuManager.Bolumgecti = true;
    }

    public void ZombilerioldurTest()  //ZombileriOldurBtn butona basılırsa çağırılacak
    {                             //tagı zombi olan tum nesneleri bir diziye attık sonra for ile tek tek sildik öldürdük
        GameObject[] zombiNesneleri = GameObject.FindGameObjectsWithTag("Zombi"); // "Zombi" etiketine sahip nesneleri bul
        int aktifZombiSayisi = zombiNesneleri.Length;

        for (int i = 0; i < aktifZombiSayisi; i++)
        {
            Destroy(zombiNesneleri[i]);
            oluSayisi += 1;
        }
        oluSayisi -= 1; // AzaltNesneSayisi fonksiyonunda 1 tane fazla artırdığı için 1 azaltık
        ZombileriOldurBtn.SetActive(false);
    }

    // skor guncelleme işlemleri
    public IEnumerator SkorlariGuncelle() 
    {
        WWWForm form = new WWWForm();
        form.AddField("unity", "Skorlari_Guncelle");
        form.AddField("kullaniciAdi", kullaniciAdi);
        form.AddField("sifre", sifre);
        form.AddField("puan", puan);
        form.AddField("toplamAltin", altin);
        form.AddField("elmas", elmas);

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
                Debug.Log(result);
            }
        }
    }

    public IEnumerator YeniBomumKilitAc()
    {
        WWWForm form = new WWWForm();
        form.AddField("unity", "YeniBolumKilitAcma");
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
                string result = www.downloadHandler.text;
                Debug.Log(result);
            }
        }
    }

    public void OyunuDevamEt()
    {
        Time.timeScale = 1;
        OyunDurduPaneli.SetActive(false);
        KameraVeCursorAyarla(false);

        // Sesleri aç
        SesleriAyarla("Zombi", true);
        SesleriAyarla("Mutant", true);

        Physics.queriesHitTriggers = true;
        pausePanelinde = false;
    }

    public void OyunuDurdur()
    {
        pausePanelinde = true;
        Time.timeScale = 0;
        OyunDurduPaneli.SetActive(true);

        KameraVeCursorAyarla(true);

        // Sesleri kapat
        SesleriAyarla("Zombi", false);
        SesleriAyarla("Mutant", false);

        Physics.queriesHitTriggers = false;
    }

    void SesleriAyarla(string etiket, bool sesDurumu)
    {
        GameObject[] dusmanlar = GameObject.FindGameObjectsWithTag(etiket);

        foreach (GameObject dusman in dusmanlar)
        {
            AudioSource sesKaynak = dusman.GetComponent<AudioSource>();
            if (sesKaynak != null)
                sesKaynak.enabled = sesDurumu;
        }
   
        AudioManager.Instance.musicSource.enabled=sesDurumu;  //backGround ses için                      
    }

    void BilgiMesajiGoster(string mesaj)
    {
        infoText.text = mesaj;
        infoTextObjesi.SetActive(true);
        StartCoroutine(InfoTextKapat());
    }

    IEnumerator InfoTextKapat()
    {
        yield return new WaitForSeconds(3); //3 saniye bekler
        infoTextObjesi.SetActive(false);
    }

    void KameraVeCursorAyarla(bool oyunDursunmu)
    {
        oyuncuKamerasi.SetActive(!oyunDursunmu);
        DurduKamerasi.SetActive(oyunDursunmu);
        Cursor.lockState = oyunDursunmu ? CursorLockMode.None : CursorLockMode.Locked; //fare imlecini oyun durdu ise kilit açma devam etderse kilitleme
    }

    public void Home()
    {
        SceneManager.LoadScene("Menu");
        Cursor.lockState = CursorLockMode.None;// fare imlecini kilidini acar
        StartCoroutine(SkorlariGuncelle());//skorları veritabanına ekler
    }

    public void Exit()
    {
        Application.Quit();  //uygulamadan çıkış yapar
        StartCoroutine(SkorlariGuncelle());//skorları veritabanına ekler
    }
}

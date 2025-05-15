using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AtesEtmeManager : MonoBehaviour
{
    [Space(10)]
    [Header("Katman Ayarlar�")]
    [SerializeField] private LayerMask zombiKatman;
    [SerializeField] private LayerMask NesneKatmani;

    [Space(10)]
    [Header("Efektler")]
    [SerializeField] private ParticleSystem Muzzleflash;
    [SerializeField] private GameObject mermiefekti;
    [SerializeField] private GameObject Tozmermiefekti;
    
    [Space(10)]
    [Header("Cephane Ayarlar�")]
    [SerializeField] private float Sarjor = 30f;
    [SerializeField] private float Sarjorkapasitesi = 30f;
    [SerializeField] private float Cephane = 300f;

    [Space(10)]
    [Header("Ate�leme ve Menzil")]
    private RaycastHit hit;
    private RaycastHit hit2;
    private float GunTimer;
    public float TaramaHizi =0.07f;
    public float Menzil = 100f;
    public float mermiKutusuMesafesi;

    [Space(10)]
    [Header("Sesler - Silah Ate� Sesleri")]
    [SerializeField] private AudioClip ak47AtesSesi;
    [SerializeField] private AudioClip M416AtesSesi;
    [SerializeField] private AudioClip m16AtesSesi;
    [SerializeField] private AudioClip ump45AtesSesi;

    [Space(5)]
    [Header("Sesler - Di�er")]
    [SerializeField] private AudioClip MermiToplamaSesi;
    [SerializeField] private AudioClip SarjorDegistirSesi;
    [SerializeField] private AudioClip SilahDegistirmeSesi;

    [SerializeField] private List<GameObject> mevcutSilahlarListesi ;
    [SerializeField] private GameObject[] tumSilahlar;

    [Space(10)]
    [Header("UI Nesneleri")]
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject Etext;

    [Space(10)]
    [Header("Di�er De�i�kenler")]
    private string kullaniciAdi;
    private string sifre;
    bool isOnline;
    bool isReloading; //�arjor de�itirme animasyonu s�ras�nda s�ras�nda  ates etme ve ates etme animasyonu kontrolu i�in

    PlayerControl playerControl;
    AnimationManager animationManager;
    Level1Manager level1Manager;

    private void Awake()
    {
        playerControl = gameObject.GetComponent<PlayerControl>();
        animationManager = Object.FindObjectOfType<AnimationManager>();
        level1Manager = Object.FindObjectOfType<Level1Manager>();   
    }
    void Start()
    {  
        kullaniciAdi = Login.kullaniciAdi;
        sifre = Login.sifre;
        isOnline= OnlineOflineManager.isOnline;
        isReloading = false;
        StartCoroutine(SilahlariEnvantereEkle()); 
    }

    void Update()
    {
        if (playerControl.hayattaMi == true)
        {
            MermiTopla();
            SliahDegistir();
            AtesKontrol();
            SarjorKontrol();          
        }       
    }
    void AtesKontrol()
    {
        if (Input.GetMouseButton(0) && Time.time > GunTimer && !isReloading) //mause sol t�k bas�ld���nda
        {
            if (Sarjor > 0 )  //sarjorda mermi varsa ve reoading false ise
            {
                GunTimer = Time.time + TaramaHizi;
                AtesEtme();
                animationManager.PlaySetAnim("atesEt");  //atesEt animasyonu aktif ediyoruz animasyonda AtesEtme fonksinu zaten �a�r�ld��� i�in burada �a��rmaya gerek yok                          
                silahSesiCal();                            
            }
            else  //�arjor bo� ise
            {
                animationManager.StopSetAnim("atesEt");  //animasyonu durdurur.
            }
        }

        if (Input.GetMouseButtonUp(0)) //mause sol t�k kald�r�ld���nda
        {
            animationManager.StopSetAnim("atesEt");  //animasyonu durdurur.
        }
    }
    void silahSesiCal()
    {
        if (isReloading) return; // reload yap�l�yorsa ses �alma

        if (tumSilahlar[0].activeSelf) //ump45 secili ise true dondurur
            AudioManager.Instance.PlaySFX(ump45AtesSesi);
        else if (tumSilahlar[1].activeSelf)
            AudioManager.Instance.PlaySFX(ak47AtesSesi);
        else if (tumSilahlar[2].activeSelf)
            AudioManager.Instance.PlaySFX(M416AtesSesi);
        else if (tumSilahlar[3].activeSelf)
            AudioManager.Instance.PlaySFX(m16AtesSesi);                 
    }

    void SarjorKontrol()
    {
        if (Sarjor < Sarjorkapasitesi && Cephane > 0 && Input.GetKeyDown(KeyCode.R)) // sarjor da mermi azalm��sa ve cephane de mermi varsa
        {           
            StartCoroutine(SarjorDegistir());                  
        }
        else if (Sarjor <= 0 && Cephane > 0) // sarjor da mermi bittiyse ve cephane de mermi varsa
        {
            StartCoroutine(SarjorDegistir());
        }
    }

    IEnumerator SarjorDegistir()
    {
        isReloading = true;
        animationManager.PlaySetAnim("SarjorDegistirme");      
        AudioManager.Instance.PlaySFX(SarjorDegistirSesi);
       
        yield return new  WaitForSeconds(3f);

        Cephane -= Sarjorkapasitesi - Sarjor;
        Sarjor = Sarjorkapasitesi;

        animationManager.StopSetAnim("SarjorDegistirme");
        isReloading = false;
    }

    IEnumerator SilahlariEnvantereEkle()
    {
        for (int i = 0; i <tumSilahlar.Length; i++)
        {
            yield return StartCoroutine(SilahCekme(i, tumSilahlar[i].name));
        }
        mevcutSilahlarListesi[0].SetActive(true);
    }
    IEnumerator SilahCekme(int index, string silahAdi)
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

                    if (int.Parse(result) == 1) //silah satin al�nd� ise
                    {
                        if (tumSilahlar[index] != null)
                        {
                            mevcutSilahlarListesi.Add(tumSilahlar[index]);
                        }                       
                    }
                    else
                    {
                        Debug.Log("silah envaterde yok!");
                    }
                }
            }
        }
    }

    int index = 1; //0. indisteki zaten se�ili old i�in
    public void SliahDegistir() //silah de�i�tirme i�lemleri
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isReloading)
        {
            AudioManager.Instance.PlaySFX(SilahDegistirmeSesi);//1 kere sesi �alar
                   
            if (index < mevcutSilahlarListesi.Count)
            {
                change(index);             
            }
            else
            {
                index = 0;
                change(index);              
            }           
        }
    }
    public void change(int number)
    {
        for (int i = 0; i < mevcutSilahlarListesi.Count; i++) //t�m silahlar�n set aktifini false yapar
        {
            mevcutSilahlarListesi[i].SetActive(false);
        }
        mevcutSilahlarListesi[number].SetActive(true);
        index++;
    }

    public void MermiTopla() //yerden mermi toplamak i�in
    {
        if (Camera.main == null)
        {
            Debug.Log("Ana kamera bulunamad�.");
            return;
        }

        Vector3 baslangicNoktasi = Camera.main.transform.position;
        Vector3 yon = Camera.main.transform.forward;

        if (Physics.Raycast(baslangicNoktasi, yon, out hit2, mermiKutusuMesafesi))
        {
            bool mermiKutusuMu = hit2.collider.CompareTag("MermiKutusu");

            crosshair.GetComponent<Image>().material.color = mermiKutusuMu ? Color.red : Color.white;
            Etext.SetActive(mermiKutusuMu);

            if (mermiKutusuMu && Input.GetKeyDown(KeyCode.E) && !isReloading)
            {
                Cephane += 100;
                AudioManager.Instance.PlaySFX(MermiToplamaSesi);
                Destroy(hit2.collider.gameObject);
            }
        }
        else
        {
            crosshair.GetComponent<Image>().material.color = Color.white;
            Etext.SetActive(false);
        }
    }

    public void AtesEtme()
    {
        if (Sarjor <= 0 || isReloading || !playerControl.hayattaMi || level1Manager.pausePanelinde) return;

        Vector3 baslangicNoktasi = Camera.main.transform.position;
        Vector3 yon = Camera.main.transform.forward;
       
        if (Physics.Raycast(baslangicNoktasi, yon, out hit, Menzil, zombiKatman))
        {
            Muzzleflash.Play();
            Instantiate(mermiefekti, hit.point, Quaternion.LookRotation(hit.normal));
            Debug.Log(hit.collider.name);

            string tag = hit.collider.gameObject.tag;

            if (tag == "Zombi")
            {
                hit.collider.GetComponent<zombi>().HasarAl();
            }
               
        }
        else if (Physics.Raycast(baslangicNoktasi, yon, out hit, Menzil, NesneKatmani))
        {
            Muzzleflash.Play();
            Instantiate(Tozmermiefekti, hit.point, Quaternion.LookRotation(hit.normal));
            Debug.Log(hit.collider.name);
        }

        Sarjor--;
    }

    public float GetSarjor()  // private de�i�kenleri ba�ka s�n�flardan  �a��r�p kullanabilmek i�in 
    {
        return Sarjor;
    }
    public float GetCephane()// private de�i�kenleri ba�ka s�n�flardan  �a��r�p kullanabilmek i�in
    {
        return Cephane;
    }
}

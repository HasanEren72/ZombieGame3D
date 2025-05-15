using System.Collections;
using UnityEngine;
using UnityEngine.AI;  //yapay zeka k�t�phanesi  mesh e g�re nesne takip i�in 
using UnityEngine.UI;

public class zombi : MonoBehaviour
{
    [Header("Zombi Sağlık ve Durum")]
    [SerializeField] private int ZombiCan = 100;  
    public bool zombiOldu;                      

    [Space(10)]
    [Header("Zombi Hedef ve Hareket")]
    [SerializeField] private GameObject hedefOyuncu;  // Zombinin hedef olarak takip edeceği oyuncu
    [SerializeField] private float mesafe;  // Zombi ile oyuncu arasındaki mesafe
    [SerializeField] private float KovalamaMesafesi;  // Zombinin oyuncuyu kovalayacağı mesafe
    [SerializeField] private float SaldirmaMesafesi;  // Zombinin oyuncuya saldıracağı mesafe

    [Space(10)]
    [Header("Animasyon ve NavMesh")]
    [SerializeField] private Animator Anim;  // Zombi animasyon kontrolörü
    [SerializeField] private NavMeshAgent zombiNavmesh;  // Zombi navmesh agent'ı

    [Space(10)]
    [Header("Ses Efektleri")]
    [SerializeField] private AudioSource sesKaynak;  // Zombi ses kaynağı
    [SerializeField] private AudioClip solVurusSesi;  // Zombi sol vuruş sesi
    [SerializeField] private AudioClip sagVurusSesi;  // Zombi sağ vuruş sesi

    [Space(10)]
    [Header("Can Bar ve UI")]
    [SerializeField] private Slider slider;  // Zombi can barı slider'ı
    [SerializeField] private Gradient gradient;  // Can barını renklendirmek için gradient
    [SerializeField] private Image fill;  // Can barı dolum kısmı
    [SerializeField] private GameObject Canbar;  // Can barı oyun objesi

    PlayerControl playerControl;
    public bool bolumSonuCanavariMi;
    public int rastgelevurus;

    private void Awake()
    {
        playerControl =Object.FindObjectOfType<PlayerControl>();//her iki s�n�f farkl� objelerde ise find ie arat�p compenentlerine ula��r�z.         
    }

    void Start()
    {
        sesKaynak = gameObject.GetComponent<AudioSource>();
        Anim = gameObject.GetComponent<Animator>();
        hedefOyuncu = GameObject.Find("Player");  // hedefoyuncu yu public olarak tan�mlayabilirdik  ve burda ekstradan nesneyi tan�tmayabilirdik
                                                     //ama her zombi nesnesi i�in hedefoyuncu(character) imizi atamam�z gerekecekti
        zombiNavmesh = this.GetComponent<NavMeshAgent>(); //gezinme a�� arac� atad�k  nesne takibi i�in
              
        SetStartCanBar(ZombiCan); //can barin ilk de�eri atan�r.
        Canbar.SetActive(false);
    }

    void Update()
    {
        SetCanBar(ZombiCan); 

        if (ZombiCan <= 0) 
        {
            zombiOldu = true;
            Canbar.SetActive(false); 
        }

        if (zombiOldu) // Zombi öldüyse
        {
            Anim.SetBool("die", true); 
            StartCoroutine(Yoketme()); 
        }
        else // Zombi yasiyorsa
        {
            if (playerControl.hayattaMi) // Oyuncu hayattaysa
            {
                mesafe = Vector3.Distance(transform.position, hedefOyuncu.transform.position); // Zombi ile oyuncu arasındaki mesafeyi ölçer

                if (mesafe <= KovalamaMesafesi && mesafe > SaldirmaMesafesi) // Takip mesafesindeyse
                {
                    Kovala(); 
                }
                else if (mesafe <= SaldirmaMesafesi) // Saldırı mesafesindeyse
                {
                    Saldir(); 
                }
                else // Ne takipte ne saldırıdaysa
                {
                    Bekle(); 
                }
            }
            else // Oyuncu öldüyse
            {
                StartCoroutine(YemeAnimasyunu()); 
            }
        }
    }

    void Kovala()
    {
        zombiNavmesh.SetDestination(hedefOyuncu.transform.position); // hedef oyuncuyu pozisyonunu navmeshe atadik hedef oyuncuyu takip etmesi için
        zombiNavmesh.isStopped = false; // NavMesh'i çalıştır takip başlar

        Canbar.SetActive(mesafe <= 10); // Mesafe 10'dan küçükse can barını göster

        Anim.SetBool("run", true); //y�r�me animasyonu
        Anim.SetBool("SolSaldırı", false);
        Anim.SetBool("SagSaldırı", false);    
    }

    void Saldir()
    {
        zombiNavmesh.isStopped = true; // NavMesh'i durdur takip etme durur
        transform.LookAt(hedefOyuncu.transform.position); //( Hedefe doğru döner ) zombi nesnesinin  hedefe bakmasini saglar. 

        Canbar.SetActive(true);

        Anim.SetBool("run", false); 

        rastgelevurus = Random.Range(0, 50);

        if (rastgelevurus < 25)
        {
            Anim.SetBool("SolSaldırı", false);//vurma animasyonu
            Anim.SetBool("SagSaldırı", true);                    
        }
        else
        {
            Anim.SetBool("SolSaldırı", true);
            Anim.SetBool("SagSaldırı", false);         
        }        
    }

    void Bekle()
    {
        zombiNavmesh.isStopped = true; // NavMesh'i durdur takip etme durur.
        Canbar.SetActive(false);

        Anim.SetBool("run", false); //durma animasyonu
        Anim.SetBool("SagSaldırı", false);
        Anim.SetBool("SolSaldırı", false);
    }

    public void SetStartCanBar(int can)
    {
        slider.maxValue = can;
        slider.value = can;
        fill.color = gradient.Evaluate(1f); // fill imagesine gradient 1f yani full de�erinde olan ye�il rengi atad�k
    }
    public void SetCanBar(int can)
    {
        slider.value = can;
        fill.color = gradient.Evaluate(slider.normalizedValue);//fill imagesine gradient�n 
    }                          //sliderin normalize de�eri atad�k boylece slider degeri de�i�ince renkde de�i�ecek

    public void Capsule_Colider_Kapat() //zombi olunce  olme(die) animasyonunda bu fonk cagirir
    {
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;  // ama� zombi olunce �lme animasyonunda hemen coliderini kapatmak 
    }                                                                    //bu sayede �len zombinin �arp��mas�n� engellemi� oluruz.
    IEnumerator YemeAnimasyunu()
    {     
        Anim.SetBool("oyuncuYeme", true);
        yield return new WaitForSeconds(3); //3 saniye bekler
    }
    IEnumerator Yoketme()
    {
        yield return new WaitForSeconds(5); //5 saniye bekler
        Destroy(this.gameObject);       // bu nesneyi(zombiyi) siler
    }
   
    public void HasarAl()
    {
        ZombiCan -= Random.Range(15, 25);      
    }
    public void HasarVer() //HasarAL fonk �a��rd�k string ifadesi olarakda mutant g�nderdik ona g�re hasar vermesi i�in
    {
        if (bolumSonuCanavariMi)
        {
            hedefOyuncu.GetComponent<PlayerControl>().HasarAL("mutant");
        }
        else
        {
            hedefOyuncu.GetComponent<PlayerControl>().HasarAL("zombi");
        }
    }
    public void SolHasarVerSes() // sald�rma animasyonunda bu fonk �a��r�l�r.
    {
        sesKaynak.PlayOneShot(solVurusSesi);
    }
    public void SagHasarVerSes() // sald�rma animasyonunda bu fonk �a��r�l�r.
    {
        sesKaynak.PlayOneShot(sagVurusSesi);
    }
}

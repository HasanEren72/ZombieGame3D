using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{  
    [Header("Karekter Kontrolleri")]
  
    [SerializeField] private float Speed = 5f, FastSpeed = 10f, ZiplamaUzunlugu = 3f;
    [SerializeField] private int KarekerCan = 100;
    public bool hayattaMi = true;

    [Space(10)][Header("Zemin Kontrolleri")]
    public Transform groundCheck;  // zemin kontrolü için
    public float groundDistance = 0.4f; // yer mesafesi
    public LayerMask groundMask;   // zemin maskesi   zemini ayırt etmek için
    bool isGrounded; // toprak mı  kontorl için 

    [Space(10)] [Header("Kamera ve Efektler")]
    [SerializeField] private GameObject olduKamerasi;
    [SerializeField] private GameObject oyuncuKamerasi;
    [SerializeField] private GameObject YerdeKanEfekti;

    [Space(10)][Header("Can Barı")]
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fillImage;

    AnimationManager animationManager;

    private void Awake()
    {
        animationManager = Object.FindObjectOfType<AnimationManager>();
    }
    void Start()
    {       
        hayattaMi = true;
        SetStartCanBar(KarekerCan); //can barın ilk değeri atanır.
    }

    void Update()
    {
        if (KarekerCan <= 0)  // Karakter öldü ise ve kan efekti henüz oluşturulmadıysa
        {
            KarekerCan = 0;
            hayattaMi = false;
            StartCoroutine(GameOver());
        }

        if (hayattaMi)
        {
            animationManager.AnimsayonControl();
            Hareket();
            Ziplama();
        }

        SetCanBar(KarekerCan);
    }
    public void SetStartCanBar(int can)
    {
        slider.maxValue = can;
        slider.value = can;
        fillImage.color = gradient.Evaluate(1f); // fill imagesine gradient 1f yani full değerinde olan yeşil rengi atadık
    }
    public void SetCanBar(int can)
    {
        slider.value = can;
        fillImage.color = gradient.Evaluate(slider.normalizedValue);//fill imagesine gradientın 
    } //sliderin normalize değeri atadık boylece slider degeri değişince renkde değişecek

    IEnumerator GameOver()
    {
        animationManager.PlaySetAnim("die");
        Cursor.lockState = CursorLockMode.None;

        yield return new WaitForSeconds(2);

        float xPos = this.gameObject.transform.position.x;
        float yPos = this.gameObject.transform.position.y + 0.4f;
        float zPos = this.gameObject.transform.position.z;

        Instantiate(YerdeKanEfekti, new Vector3(xPos, yPos, zPos), this.gameObject.transform.rotation);

        olduKamerasi.SetActive(true);
        oyuncuKamerasi.SetActive(false);

        yield return new WaitForSeconds(3); //3 saniye bekler

        Time.timeScale = 1.0f; 
        SceneManager.LoadScene("GameOver"); //game over sahnesini açar
    }

    void Hareket()
    {
        float yatay = Input.GetAxis("Horizontal") * Time.deltaTime; //yatay girişler
        float dikey = Input.GetAxis("Vertical") * Time.deltaTime;  // dikey girişler

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(yatay * FastSpeed, 0, dikey * FastSpeed);
            animationManager.PlaySetAnim("fastrun"); 
        }
        else
        {
            transform.Translate(yatay * Speed, 0, dikey * Speed);
            animationManager.StopSetAnim("fastrun");
        }
    }

    void Ziplama()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //groundCheck pozisyonuna dayalı bir küre yaratacaktır 
                                                                                            //yer mesafesi ve zemin maskesi de eklenir
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            transform.Translate(Vector3.up * ZiplamaUzunlugu);
            animationManager.PlaySetAnim("jump"); 
        }
        else
        {
            animationManager.StopSetAnim("jump");
        }
    }

    public void HasarAL(string dusman)
    {
        if (dusman == null) return;

        if (dusman == "zombi") //düşman zombi ise hasar miktarı
        {
            KarekerCan -= Random.Range(5, 15);
        }
        else if (dusman == "mutant")//düşman mutant ise hasar miktarı
        {
            KarekerCan -= Random.Range(15, 20);
        }
    }
    public float GetKarekerCan()  // private değişkenleri başka sınıflardan  çağırıp kullanabilmek için 
    {                             //public fonk oluşturup private değişkeni return ettik.
        return KarekerCan;
    }
}

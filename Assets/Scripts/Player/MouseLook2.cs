using UnityEngine;

public class MouseLook2 : MonoBehaviour
{
    [SerializeField] private Transform hedefOyuncu;  // Kameranýn takip edeceði hedef (oyuncu karakteri)
    [SerializeField] private Transform oyuncuGovde;  // Kamera ile oyuncunun gövdesini döndürmek için

    [SerializeField] private Vector3 hedefMesafe;  // Kamera ile hedef arasýndaki mesafe offseti
    [SerializeField] private float hedefTakipHizi =10f;

    [Range(0, 100)]
    [SerializeField]
    private float MauseSensitivity =2f;  // Fare Hassasiyeti
    private float mouseX, mouseY;  // Mouse eksen deðerleri

    PlayerControl playerControl;  // Karakter kontrol scriptine eriþim

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Fare imlecini kilitler yani görünmez yapar
        playerControl = Object.FindObjectOfType<PlayerControl>();
    }

    private void LateUpdate()  
    {
        if (playerControl.hayattaMi)  // Oyuncu hayatta ise 
        {
            PozisyonAyarla();
            RotasyonAyarla();
        }
    }

    void PozisyonAyarla()
    {
        // Kamera pozisyonunu oyuncunun pozisyonuna ve offsetine göre ayarlama
        transform.position = Vector3.Lerp(transform.position, hedefOyuncu.position + hedefMesafe, Time.deltaTime * hedefTakipHizi);
    }

    void RotasyonAyarla()
    {
        mouseX += Input.GetAxis("Mouse X") * MauseSensitivity; // Fare hareketlerini toplama ve hassasiyetle çarpma
        mouseY += Input.GetAxis("Mouse Y") * MauseSensitivity;

        mouseY = Mathf.Clamp(mouseY, -50, 50); // Yukarý aþaðý bakma limitleri
     
        transform.eulerAngles = new Vector3(-mouseY, mouseX, 0);   // Kamera yönünü fare hareketine göre ayarlama
        hedefOyuncu.transform.eulerAngles = new Vector3(0, mouseX, 0); // Karakterin yönünü fare hareketine göre ayarlama       

        // Kamera hareketi ile karakterin gövdesini hareket ettirmek için
        Vector3 geciciRotasyon = transform.eulerAngles; // Kameranýn rotasyonunu deðiþkene atadýk
        geciciRotasyon.z = 0;
        geciciRotasyon.y = transform.localEulerAngles.y;      // Y ekseninde kamera ile ayný rotasyon olacak
        geciciRotasyon.x = transform.localEulerAngles.x + 10; // X ekseninde 10 birim döndürecek
        
        // Karakter gövdesine bu rotasyonu aktarma
        oyuncuGovde.transform.eulerAngles = geciciRotasyon;
    }
}
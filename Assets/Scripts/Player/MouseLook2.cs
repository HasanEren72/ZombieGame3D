using UnityEngine;

public class MouseLook2 : MonoBehaviour
{
    [SerializeField] private Transform hedefOyuncu;  // Kameran�n takip edece�i hedef (oyuncu karakteri)
    [SerializeField] private Transform oyuncuGovde;  // Kamera ile oyuncunun g�vdesini d�nd�rmek i�in

    [SerializeField] private Vector3 hedefMesafe;  // Kamera ile hedef aras�ndaki mesafe offseti
    [SerializeField] private float hedefTakipHizi =10f;

    [Range(0, 100)]
    [SerializeField]
    private float MauseSensitivity =2f;  // Fare Hassasiyeti
    private float mouseX, mouseY;  // Mouse eksen de�erleri

    PlayerControl playerControl;  // Karakter kontrol scriptine eri�im

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Fare imlecini kilitler yani g�r�nmez yapar
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
        // Kamera pozisyonunu oyuncunun pozisyonuna ve offsetine g�re ayarlama
        transform.position = Vector3.Lerp(transform.position, hedefOyuncu.position + hedefMesafe, Time.deltaTime * hedefTakipHizi);
    }

    void RotasyonAyarla()
    {
        mouseX += Input.GetAxis("Mouse X") * MauseSensitivity; // Fare hareketlerini toplama ve hassasiyetle �arpma
        mouseY += Input.GetAxis("Mouse Y") * MauseSensitivity;

        mouseY = Mathf.Clamp(mouseY, -50, 50); // Yukar� a�a�� bakma limitleri
     
        transform.eulerAngles = new Vector3(-mouseY, mouseX, 0);   // Kamera y�n�n� fare hareketine g�re ayarlama
        hedefOyuncu.transform.eulerAngles = new Vector3(0, mouseX, 0); // Karakterin y�n�n� fare hareketine g�re ayarlama       

        // Kamera hareketi ile karakterin g�vdesini hareket ettirmek i�in
        Vector3 geciciRotasyon = transform.eulerAngles; // Kameran�n rotasyonunu de�i�kene atad�k
        geciciRotasyon.z = 0;
        geciciRotasyon.y = transform.localEulerAngles.y;      // Y ekseninde kamera ile ayn� rotasyon olacak
        geciciRotasyon.x = transform.localEulerAngles.x + 10; // X ekseninde 10 birim d�nd�recek
        
        // Karakter g�vdesine bu rotasyonu aktarma
        oyuncuGovde.transform.eulerAngles = geciciRotasyon;
    }
}
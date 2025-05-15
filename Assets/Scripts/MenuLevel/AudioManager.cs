using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [SerializeField] private AudioClip clickSesi;

    [Range(0,1)]
    [SerializeField] private float SoundVolume = 0.7f;
    [SerializeField] private Button[] butonlar;

    private void Awake()
    {
        if (Instance ==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ButonlaraClickSesiEkle() //sahnedeki t�m butonlara click sesi ekler
    {
        butonlar = Resources.FindObjectsOfTypeAll<Button>();//sahenedeki aktif ve pasif t�m butonlar� atar

        foreach (var buton in butonlar) //t�m butonlara  clickSesiCal() fonk. ekledik
        {
            buton.onClick.AddListener(() => PlaySFX(clickSesi));
        }
    }

    // M�zik �al
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = SoundVolume;
        musicSource.Play();
    }

    // Efekt �al
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    // M�zik Durdur
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Efekt Sesini Durdur
    public void StopSFX()
    {    
       sfxSource.Stop();
    }

    public void ToggleMusic()
    {      
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        else
        {
            musicSource.Play();
        }
    } 
}

using System.Collections;
using TMPro;
using UnityEngine;

public class UI_Control : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject Giris_panel;
    [SerializeField] private GameObject kayit_panel;
    [SerializeField] private TextMeshProUGUI  hataYazisi_TMP;

    [SerializeField] private AudioClip loginSes;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(loginSes);
        AudioManager.Instance.ButonlaraClickSesiEkle();
    }

    public void Kayitol_B()
    {
        kayit_panel.SetActive(true);
        Giris_panel.SetActive(false);
    }
    public void Geri_B()
    {
        kayit_panel.SetActive(false);
        Giris_panel.SetActive(true);       
    }

    public IEnumerator hataPanel(string hataText)
    {
        hataYazisi_TMP.SetText(hataText);
       
        yield return new WaitForSeconds(1.5f);
       // hataAnimator.SetBool("HataDurumu", false);
    }

    public void SesAcKapa()
    {
      AudioManager.Instance.ToggleMusic();
    }

    public void exit()
    {
        Application.Quit();
    }
}

using UnityEngine;

public class SkyBoxControl : MonoBehaviour
{
    [SerializeField] private GameObject yagmurEfekti; 
    [SerializeField] private GameObject KarEfekti;  

    [SerializeField] private AudioClip yagmurSesi;  
    [SerializeField] private AudioClip BackGroundSesi;  

    [SerializeField] private Material matGunesli , matYagmurlu , matKarli;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(BackGroundSesi);
    }
    public void HavaDurumuDegistir(int index)
    {
        if (index == 0) //default güneþli
        {        
            RenderSettings.skybox = matGunesli;
            AudioManager.Instance.PlayMusic(BackGroundSesi);
        }
        else if (index == 1)  //güneþli
        {
            RenderSettings.skybox = matGunesli;
            yagmurEfekti.SetActive(false);
            KarEfekti.SetActive(false);
            AudioManager.Instance.PlayMusic(BackGroundSesi);
        }
        else if (index == 2) //yaðmurlu
        {
            RenderSettings.skybox = matYagmurlu;
            yagmurEfekti.SetActive(true);
            KarEfekti.SetActive(false);
            AudioManager.Instance.PlayMusic(yagmurSesi);        
        }
        else if(index == 3) //karlý
        {
            RenderSettings.skybox = matKarli;
            yagmurEfekti.SetActive(false);
            KarEfekti.SetActive(true);
            AudioManager.Instance.PlayMusic(BackGroundSesi);
        }
    }
}

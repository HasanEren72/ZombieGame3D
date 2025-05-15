using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
    }

    public void AnimsayonControl()
    {
        float yatay = Input.GetAxis("Horizontal");
        float dikey = Input.GetAxis("Vertical");

        playerAnimator.SetFloat("Horizontal", yatay); // Animasyon karýþýmý için blendtree de kullanýlýr.
        playerAnimator.SetFloat("Vertical", dikey);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            playerAnimator.SetBool("crouch", true);     //çömelme             
        }
        else
        {
            playerAnimator.SetBool("crouch", false);
        }
    }

    public void PlaySetAnim(string animasyonAdi)
    {
        playerAnimator.SetBool(animasyonAdi, true);
    }

    public void StopSetAnim(string animasyonAdi)
    {
        playerAnimator.SetBool(animasyonAdi, false);
    }
}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ToggleScript : MonoBehaviour
{
    [SerializeField] RectTransform HandleRectTransform;
    [SerializeField] Sprite AktiveSprite;
    [SerializeField] GameObject BackGroundimage;

    Image BarImage;
    Sprite BackGroundSpriteDefault;

    Toggle toggle;

    Vector2 handlePosistion;

    private void Awake()
    {
        BackGroundimage.GetComponent<RectTransform>().localScale = Vector3.zero;

        toggle = GetComponent<Toggle>();
        handlePosistion = HandleRectTransform.anchoredPosition;
        toggle.onValueChanged.AddListener(OnSwitch);
        
        BarImage = HandleRectTransform.parent.GetComponent<Image>(); //BarImage Handle ýn üstünde olduðu için "Parent" kullandýk.

        BackGroundSpriteDefault = BarImage.sprite;       
    }

    void OnSwitch(bool on)
    {      
        // HandleRectTransform.anchoredPosition = on ? handlePosistion * -1 : handlePosistion;

        HandleRectTransform.DOAnchorPos ( on ? handlePosistion * -1 : handlePosistion  , 0.4f ).SetEase(Ease.InOutBack);
      
        BarImage.sprite = on ? AktiveSprite : BackGroundSpriteDefault;

        if (on)
        {
            BackGroundimage.GetComponent<RectTransform>().DOScale(1, 0.5f).SetEase(Ease.OutBack);
        }
        else
        {
            BackGroundimage.GetComponent<RectTransform>().DOScale(0, 0.5f).SetEase(Ease.InBack);
        }
    }

    void OnDestroy() //silinirse fonksiyonu kaldýrýr bellek sýzýntýlarýný önler.
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}

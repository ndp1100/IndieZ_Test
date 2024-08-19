using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Domain;
using Game.Level.Gun;
using Game.UI.Hud;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GunHudView : BaseHudWithModel<GunModel>
{
    [SerializeField] private Button _fireButton;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private Image _reloadImage;

    public TextMeshProUGUI AmmoText => _ammoText;
    public Image ReloadImage => _reloadImage;
    public Button FireButton => _fireButton;


    public void SetAmmoText(int currentClip, int clipSize)
    {
        _ammoText.text = $"{currentClip}/{clipSize}";
    }

    public void ShowReloadImage(bool isShow)
    {
        _reloadImage.gameObject.SetActive(isShow);
    }

    public void SetReloadImage(float fillAmount)
    {
        if (_reloadImage.gameObject.activeSelf == false)
            _reloadImage.gameObject.SetActive(true);

        _reloadImage.fillAmount = fillAmount;
    }

    protected override void OnEnable()
    {
        _reloadImage.fillAmount = 0;
        _reloadImage.gameObject.SetActive(false);
        _fireButton.interactable = false;
    }

    protected override void OnDisable()
    {

    }

    protected override void OnModelChanged(GunModel model)
    {
        SetAmmoText(model.CurrentClip, model.ClipSize);
        _fireButton.interactable = model.IsCanShoot;
    }
}

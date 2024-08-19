using DG.Tweening;
using Game.Level.Modules;
using Injection;
using TMPro;
using UnityEngine;

public class DmgTextView : MonoBehaviour
{
    [Inject] private BattleObjectModule _battleObjectModule;

    [SerializeField] private TextMeshProUGUI _dmgText;
    [Inject] private LevelView _levelView;


    //show damage text with animation and destroy it after
    public void ShowDmgText(Transform uiPivoTransform, string text, Color color, float aliveTime = 0.25f)
    {
        _dmgText.text = text;
        _dmgText.color = color;

        //convert world position to screen position
        var screenPos = _levelView.MainCamera.WorldToScreenPoint(uiPivoTransform.position);
        transform.position = screenPos;

        //animation with dotween
        transform.localScale = Vector3.one * 0.5f;
        transform.DOScale(Vector3.one, aliveTime).SetEase(Ease.OutBack);

        //floating up animation
        transform.DOMoveY(transform.position.y + 50, aliveTime).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            _battleObjectModule.ReleaseDmgText(this);
        });
    }
}
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class JackhummerIndicator : AbstractIndicator
{
    [SerializeField] private Image _bg;
    [SerializeField] private CanvasGroup _canvasGroup;

    private Jackhammer _jackhammer;
    private Tween _tween;
    private bool _isOverheat;

    public void Init(Jackhammer jackhammer, RectTransform canvas)
    {
        _jackhammer = jackhammer;

        Init(_jackhammer.FloatParameter, _jackhammer.IndicatorPoint, canvas);

        if (FloatParameter.IsAtMaxValue())
            View.gameObject.SetActive(false);
    }

    protected override void OnValueChanged(float arg1, float arg2)
    {
        if (_jackhammer.IsOverheat)
            return;

        ProgressFill.fillAmount = FloatParameter.GetPercentage();
        ProgressFill.color = FloatParameter.IsCritical() ? Color.yellow : Color.green;
    }

    protected override void Update()
    {
        base.Update();
        View.gameObject.SetActive(_jackhammer.HasVisible() && FloatParameter.IsAtMaxValue() == false);

        HandleOverheat();
    }

    private void HandleOverheat()
    {
        if (_isOverheat == _jackhammer.IsOverheat)
            return;

        _isOverheat = _jackhammer.IsOverheat;

        if (_isOverheat)
        {
            _bg.color = Color.red;
            _tween.Kill();
            _tween = _canvasGroup.DOFade(0, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
        else
        {
            _tween.Kill();
            _tween = null;
            _canvasGroup.alpha = 1.0f;
            _bg.color = Color.black;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _tween.Kill();
    }
}
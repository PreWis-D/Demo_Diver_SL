using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractIndicator : AbstractIndicator
{
    [SerializeField] private Image _inputIcon;

    private Tween _tween;
    private RectTransform _canvas;

    private void OnEnable()
    {
        View.gameObject.SetActive(false);
    }

    public override void Init(FloatParameter floatParameter, Transform seekPoint, RectTransform canvas)
    {
        base.Init(floatParameter, seekPoint, canvas);

        _canvas = canvas;
    }

    public void Show()
    {
        View.gameObject.SetActive(true);
        ProgressFill.fillAmount = FloatParameter.GetPercentage();
        //_tween.Kill();
        //_tween = View.transform.DOScale(Vector3.one, 0.25f).From(Vector3.zero).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        View.gameObject.SetActive(false);
        //_tween.Kill();
        //_tween = View.transform.DOScale(Vector3.zero, 0.25f).From(Vector3.one).SetEase(Ease.InBack);
    }

    public void UpdateInteractable(IInteractable interactable, bool isFounded)
    {
        if (isFounded)
        {
            Init(interactable.FloatParameter, SeekPoint, _canvas);
            _inputIcon.sprite = SL.Get<SpritesService>().GetInputSprite(interactable.InputType);
            Show();
        }
        else
        {
            Hide();
        }
    }

    protected override void OnValueChanged(float arg1, float arg2)
    {
        ProgressFill.fillAmount = FloatParameter.GetPercentage();
        View.gameObject.SetActive(FloatParameter.IsAtMinValue() == false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _tween.Kill();
    }
}
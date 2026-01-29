using DG.Tweening;
using UnityEngine;

public class AimIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform _view;
    [SerializeField] private float _sensitivity = 1f;

    private InputProcessingService _input;
    private RectTransform _rectTransform;
    private Camera _camera;
    private Tween _tween;
    private RectTransform _canvas;

    private Vector2 _canvasSize;

    private float _smoothSpeed = 10f;
    private bool _isActive;

    public void Init(RectTransform canvas)
    {
        _canvas = canvas;
        _rectTransform = GetComponent<RectTransform>();
        _canvasSize = canvas.sizeDelta;
        _input = SL.Get<InputProcessingService>();
        _camera = SL.Get<CamerasService>().GetMainCamera();
        _view.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (_input.IsAim)
            Animate(Vector3.one, Vector3.zero, Ease.OutBack, true);
        else
            Animate(Vector3.zero, Vector3.one, Ease.InBack, false);
    }

    private void LateUpdate()
    {
        if (_input.IsAim)
            HandlePosition();
    }

    private void Animate(Vector3 target, Vector3 start, Ease ease, bool isActive)
    {
        if (_isActive == isActive)
            return;

        _isActive = isActive;

        _tween.Kill();
        _tween = _view.transform.DOScale(target, 0.15f).From(start).SetEase(ease);
    }

    private void HandlePosition()
    {
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas,
            _input.AimPosition,
            _camera,
            out pos
        );

        Vector2 normalizedPosition = new Vector2(
            _input.AimPosition.x / Screen.width,
            _input.AimPosition.y / Screen.height
        );

        Vector2 targetPosition = new Vector2(
            (normalizedPosition.x - 0.5f) * _canvasSize.x * _sensitivity,
            (normalizedPosition.y - 0.5f) * _canvasSize.y * _sensitivity
        );

        _rectTransform.localPosition = targetPosition;
    }
}
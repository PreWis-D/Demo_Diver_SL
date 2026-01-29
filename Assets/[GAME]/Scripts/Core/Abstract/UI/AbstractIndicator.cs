using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractIndicator : MonoBehaviour
{
    [SerializeField] protected RectTransform View;
    [SerializeField] protected Image ProgressFill;

    private RectTransform _rectTransform;
    private Camera _camera;
    private Vector2 _canvasSize;
    private float _smoothSpeed = 10f;
    private Vector2 _targetPosition;

    private Coroutine _coroutine;
    private WaitForEndOfFrame _waitForEndOfFrame;
    protected FloatParameter FloatParameter;

    public Transform SeekPoint { get; protected set; }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public virtual void Init(FloatParameter floatParameter, Transform seekPoint, RectTransform canvas)
    {
        if (FloatParameter != null)
            FloatParameter.ValueChanged -= OnValueChanged;

        FloatParameter = floatParameter;
        SeekPoint = seekPoint;
        _camera = SL.Get<CamerasService>().GetMainCamera();
        _canvasSize = canvas.sizeDelta;

        FloatParameter.ValueChanged += OnValueChanged;
    }

    protected virtual void Update()
    {
        if (_coroutine == null)
            _coroutine = StartCoroutine(MoveAtEndOfFrame());
    }

    private IEnumerator MoveAtEndOfFrame()
    {
        _waitForEndOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            yield return _waitForEndOfFrame;
            if (SeekPoint != null)
            {
                _targetPosition = CoordinateConverter.WorldToViewportPointConvert(SeekPoint, _canvasSize);
                _rectTransform.localPosition = _targetPosition;
            }
        }
    }

    protected abstract void OnValueChanged(float arg1, float arg2);

    protected virtual void OnDestroy()
    {
        StopCoroutine();
    }

    private void StopCoroutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}
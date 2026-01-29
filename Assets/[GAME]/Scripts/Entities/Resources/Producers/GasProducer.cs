using UnityEngine;
using UnityEngine.UI;

public class GasProducer : AbstractResourceProducer
{
    [SerializeField] private float _startGasAmount;
    [SerializeField] private Transform _gasFX;
    [SerializeField] private Transform _progressTransform;
    [SerializeField] private Image _fill;
    [field: SerializeField] public Transform ConnectPlayerPoint { get; private set; }
    [field: SerializeField] public Transform ConnectToolPoint { get; private set; }

    public FloatParameter FloatParameter { get; private set; }

    public override void Init()
    {
        FloatParameter = new FloatParameter(_startGasAmount, _startGasAmount);
        FloatParameter.ValueChanged += OnValueChanged;

        UpdateFill();
        Interrupt();
    }

    public void ChangeVisibleGasFX(bool isVisible)
    {
        if (FloatParameter.IsAtMinValue())
        { 
            _gasFX.gameObject.SetActive(false);
            return;
        }

        _gasFX.gameObject.SetActive(isVisible);
    }

    public void Interact()
    {
        _progressTransform.gameObject.SetActive(true);
        FloatParameter.DecreaseValue(Time.deltaTime);
    }

    public void Interrupt()
    {
        _progressTransform.gameObject.SetActive(false);
    }

    private void OnValueChanged(float arg1, float arg2)
    {
        UpdateFill();

        if (FloatParameter.IsAtMinValue())
            _gasFX.gameObject.SetActive(false);
    }

    private void UpdateFill()
    {
        _fill.fillAmount = FloatParameter.GetPercentage();
    }

    private void OnDestroy()
    {
        FloatParameter.ValueChanged -= OnValueChanged;
    }
}
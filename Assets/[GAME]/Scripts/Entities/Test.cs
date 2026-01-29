using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class Test : MonoBehaviour, IDamagable
{
    [SerializeField] private MeshRenderer _renderer;

    private FloatParameter _floatParameter;

    public bool IsDied { get; private set; }

    private void Awake()
    {
        _floatParameter = new FloatParameter(3, 3);
        _floatParameter.ValueChanged += OnValueChanged;
    }

    public void TakeDamage(float damage)
    {
        _floatParameter.DecreaseValue(damage);
    }

    public void Died()
    {
        IsDied = true;
        _renderer.material.color = Color.black;
    }

    private void OnValueChanged(float arg1, float arg2)
    {
        if (_floatParameter.IsAtMinValue())
            Died();
        else
            Blink().Forget();
    }

    private async UniTask Blink()
    {
        _renderer.material.color = Color.red;
        await UniTask.Delay(100);
        _renderer.material.color = Color.white;
    }

    private void OnDestroy()
    {
        _floatParameter.ValueChanged -= OnValueChanged;
    }
}
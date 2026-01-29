using System;

public abstract class ObservableParameter<T> where T : IComparable
{
    protected T _maxValue;
    protected T _currentValue;
    protected float _criticalThreshold = 0.2f;

    public event Action ValueMin;
    public event Action ValueMax;
    public event Action ValueCritical;
    public event Action<T, T> ValueChanged;

    public ObservableParameter(T maxValue, T currentValue, float criticalThreshold = 0.2f)
    {
        _maxValue = maxValue;
        _currentValue = currentValue;
        _criticalThreshold = criticalThreshold;

        if (_currentValue.CompareTo(default(T)) <= 0)
            _currentValue = _maxValue;
    }

    public T CurrentValue
    {
        get => _currentValue;
        set
        {
            T previousValue = _currentValue;
            _currentValue = ClampValue(value);

            if (!_currentValue.Equals(previousValue))
            {
                ValueChanged?.Invoke(_currentValue, _maxValue);
                CheckSpecialConditions(previousValue);
            }
        }
    }

    public T MaxValue
    {
        get => _maxValue;
        set
        {
            if (value.CompareTo(default(T)) <= 0) return;

            T previousMax = _maxValue;
            _maxValue = value;
            _currentValue = ClampValue(_currentValue);

            if (!_maxValue.Equals(previousMax))
            {
                ValueChanged?.Invoke(_currentValue, _maxValue);
            }
        }
    }

    public float CriticalThreshold => _criticalThreshold;

    public abstract T ClampValue(T value);

    protected virtual void CheckSpecialConditions(T previousValue)
    {
        if (IsAtMinValue())
        {
            ValueMin?.Invoke();
        }
        else if (IsAtMaxValue())
        {
            ValueMax?.Invoke();
        }
        else if (IsCritical())
        {
            ValueCritical?.Invoke();
        }
    }

    public abstract bool IsAtMinValue();
    public abstract bool IsAtMaxValue();
    public abstract bool IsCritical();
    public abstract float GetPercentage();
}

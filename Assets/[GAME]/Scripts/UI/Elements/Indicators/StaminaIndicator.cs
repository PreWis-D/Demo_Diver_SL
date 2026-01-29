using UnityEngine;
using UnityEngine.UI;

public class StaminaIndicator : AbstractIndicator
{
    public override void Init(FloatParameter floatParameter, Transform seekPoint, RectTransform canvas)
    {
        base.Init(floatParameter, seekPoint, canvas);

        if (FloatParameter.IsAtMaxValue())
            View.gameObject.SetActive(false);
    }

    protected override void OnValueChanged(float arg1, float arg2)
    {
        View.gameObject.SetActive(FloatParameter.IsAtMaxValue() == false);
        ProgressFill.fillAmount = FloatParameter.GetPercentage();
        ProgressFill.color = FloatParameter.IsCritical() ? Color.red : Color.green;
    }
}
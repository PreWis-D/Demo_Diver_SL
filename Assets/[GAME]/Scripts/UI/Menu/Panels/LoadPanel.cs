using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadPanel : AbstractPanel
{
    [SerializeField] private Image _progressbar;
    [SerializeField] private TMP_Text _progressText;

    private FloatParameter _progress;

    public override PanelType Type => PanelType.Load;

    public override void Init()
    {
        base.Init();

        _progress = SL.Get<SceneLoadService>().ProggressParam;
        _progress.ValueChanged += OnProgressChanged;

        OnProgressChanged(_progress.CurrentValue, _progress.MaxValue);
    }

    private void OnProgressChanged(float current, float max)
    {
        _progressText.SetText($"Loading {(int)current}%");
        _progressbar.fillAmount = current / max;
    }

    private void OnDestroy()
    {
        _progress.ValueChanged -= OnProgressChanged;
    }
}